using Api.Contratos;
using Npgsql;

namespace Api.Endpoints;

public static class PostManutencao
{
    public static void AddExecutarManutencaoBdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/manutencao/reseta-bd", ResetaBancoDeDadosAsync)
            .Produces<ExtratoResponse>()
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("ResetaBancoDeDados")
            .WithTags("manutencao")
            .WithOpenApi();
    }

    private static async Task<IResult> ResetaBancoDeDadosAsync(
        [FromServices] NpgsqlConnection connection,
        CancellationToken ct)
    {
        await using (connection)
        {
            await connection.OpenAsync(ct);
            await using var cmd = connection.CreateBatch();
            cmd.BatchCommands.Add(new NpgsqlBatchCommand("update saldo_cliente set saldo = 0"));
            cmd.BatchCommands.Add(new NpgsqlBatchCommand("truncate table transacao_cliente"));
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            return Results.Ok("Banco de dados resetado com sucesso!");
        }
    }
}