using Api.Contratos;
using Npgsql;

namespace Api.Endpoints;

public static class PostTransacao
{
    public static void AddCriarTransacaoPorClienteEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/clientes/{id}/transacoes", CriarTransacaoPorClienteAsync)
            .Produces<TransacaoResponse>()
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("CriarTransacaoPorCliente")
            .WithTags("clientes")
            .WithOpenApi();
    }

    private static async Task<IResult> CriarTransacaoPorClienteAsync(
        [FromRoute] int id,
        [FromBody] TransacaoRequest request,
        [FromServices] NpgsqlConnection connection,
        CancellationToken ct)
    {
        if (id is < 1 or > 5)
            return Results.NotFound();

        if (!request.EhValido())
            return Results.UnprocessableEntity();
        
        await using (connection)
        {
            await connection.OpenAsync(ct);
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT saldo_atualizado, limite_atual, linhas_afetadas FROM atualiza_saldo_cliente_and_insere_transacao($1, $2, $3, $4);";
            cmd.Parameters.AddWithValue(id);
            cmd.Parameters.AddWithValue(request.Valor);
            cmd.Parameters.AddWithValue(request.Descricao!);
            cmd.Parameters.AddWithValue(request.Tipo!);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            await reader.ReadAsync(ct);

            var atualizouSaldo = reader.GetInt16(2) > 0;
            var saldo = reader.GetInt32(0);
            var limite = reader.GetInt32(1);
            
            return atualizouSaldo 
                ? Results.Ok(new TransacaoResponse(saldo, limite))
                : Results.UnprocessableEntity();
        }
    }
}