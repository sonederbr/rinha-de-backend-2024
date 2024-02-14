using Api.Endpoints.Cliente.Dtos;
using Api.Repository;

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
        [FromServices] ClienteRepository repository,
        CancellationToken ct)
    {
        var cliente = await repository.ObterClienteTransacao(id, ct);
        if (cliente is null)
            return Results.NotFound("Cliente não encontrado!");
        
        var result = new ExtratoResponse
        {
            Saldo = new()
            {
                Total = cliente.Saldo,
                DataExtrato = DateTime.UtcNow,
                Limite = cliente.Limite
            },
            UltimasTransacoes = cliente.Transacoes.Select(
                t => new ExtratoResponse.TransacaoCliente()
                {
                    Descricao = t.Descricao,
                    RealizadaEm = t.DataTransacao, 
                    Tipo = t.Tipo, 
                    Valor = t.Valor
                }).ToList()
        };
        return Results.Ok(result);
    }
}