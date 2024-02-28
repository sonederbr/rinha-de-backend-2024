using Api.Contratos;
using Npgsql;

namespace Api.Endpoints;

public static class ObterExtratoPorCliente
{
    public static void AddObterExtratoPorClienteEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/clientes/{id}/extrato", ObterExtratoPorClienteAsync)
            .Produces<ExtratoResponse>()
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("ObterExtratoPorCliente")
            .WithTags("clientes")
            .WithOpenApi();
    }

    private static async Task<IResult> ObterExtratoPorClienteAsync(
        [FromRoute] int id,
        [FromServices] NpgsqlConnection connection,
        CancellationToken ct)
    {
        if (id is < 1 or > 5)
            return Results.NotFound();

        await using (connection)
        {
            await connection.OpenAsync(ct);

            await using var cmd = connection.CreateCommand();
            cmd.CommandText =  "SELECT * FROM obter_extrato_cliente($1);";
            cmd.Parameters.AddWithValue(id);

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            await reader.ReadAsync(ct);

            ExtratoSaldoClienteResponse extrato = new(reader.GetInt32(1), reader.GetDateTime(4), reader.GetInt32(0));
            var transacoes = new List<ExtratoTransacaoClienteResponse>();
            while (await reader.ReadAsync(ct))
                transacoes.Add(
                    new(reader.GetInt32(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetDateTime(4)));

            return Results.Ok(new ExtratoResponse(extrato, transacoes));
        }
    }
}