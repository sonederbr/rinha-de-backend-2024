using Api.Endpoints.Cliente.Dtos;
using Api.Repository;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;

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
            .WithName("ObterExtratoAsync")
            .WithTags("clientes")
            .WithOpenApi();
    }

    private static async Task<IResult> ObterExtratoPorClienteAsync(
        [FromRoute] int id,
        [FromServices] ClienteRepository repository,
        [FromServices] IMemoryCache cache,
        CancellationToken ct)
    {
        if (cache.Get(id) is null)
            return Results.NotFound("Cliente não encontrado!");

        var cliente = await repository.ObterExtratoAsync(id);     
        var result = new ExtratoResponse
        {
            Saldo = new()
            {
                Total = cliente.Value.Saldo,
                DataExtrato = DateTime.UtcNow,
                Limite = cliente.Value.Limite
            },
            UltimasTransacoes = cliente.Value.Transacoes.Select(
                t => new ExtratoResponse.TransacaoCliente()
                {
                    Valor = t.Valor,
                    Tipo = t.Tipo, 
                    Descricao = t.Descricao,
                    RealizadaEm = t.DataTransacao, 
                }).ToList()
        };
        return Results.Ok(result);
    }
}