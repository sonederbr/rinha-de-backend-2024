using Api.Endpoints.Cliente.Dtos;

namespace Api.Endpoints.Cliente;

public static class GetExtrato
{
    public static void AddExtratoPorClienteEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/clientes/{id}/extrato", ObterExtratoPorClienteAsync)
            .Produces<ExtratoResponse>()
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .AllowAnonymous()
            .WithName("ObterExtrato")
            .WithTags("clientes")
            .WithOpenApi();
    }

    private static async Task<IResult> ObterExtratoPorClienteAsync(
        [FromRoute] int id,
        CancellationToken ct)
    {
        var result = new ExtratoResponse
        {
            Saldo = new()
            {
                Total = -9098,
                DataExtrato = DateTime.Parse("2024-01-17T02:34:41.217753Z"),
                Limite = 100000
            },
            UltimasTransacoes =
            [
                new()
                {
                    Valor = 10,
                    Tipo = 'c',
                    Descricao = "descricao",
                    RealizadaEm = DateTime.Parse("2024-01-17T02:34:38.543030Z")
                },

                new()
                {
                    Valor = 90000,
                    Tipo = 'd',
                    Descricao = "descricao",
                    RealizadaEm = DateTime.Parse("2024-01-17T02:34:38.543030Z")
                }
            ]
        };
        return Results.Ok(result);
    }
}