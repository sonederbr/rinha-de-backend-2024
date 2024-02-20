using Api.Endpoints.Cliente.Dtos;
using Api.Model;
using Api.Repository;
using Microsoft.Extensions.Caching.Memory;
namespace Api.Endpoints.Cliente;

public static class PostTransacao
{
    public static void AddCriarTransacaoPorClienteEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/clientes/{id}/transacoes", CriarTransacaoPorClienteAsync)
            .Produces<TransacaoResponse>()
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .AllowAnonymous()
            .WithName("CriarTransacao")
            .WithTags("clientes")
            .WithOpenApi();
    }

    private static async Task<IResult> CriarTransacaoPorClienteAsync(
        [FromRoute] int id,
        [FromBody] TransacaoRequest req,
        [FromServices] ClienteRepository repository,
        [FromServices] IMemoryCache cache,
        CancellationToken ct)
    {
        if (!req.EhValido(id))
            return Results.BadRequest("Requisicao inválida!");

        if (cache.Get(id) is null)
            return Results.NotFound("Cliente não encontrado!");

        var result = await repository.CrebitarAsync(new Crebitar(
            IdCliente: id,
            Valor: req.Valor,
            Tipo: req.Tipo,
            Descricao: req.Descricao), ct);

        return !result.HasValue
            ? Results.UnprocessableEntity()
            : Results.Ok(new TransacaoResponse(result.Value.Saldo, result.Value.Limite));
    }
}