using Api.Endpoints.Cliente.Dtos;
using Api.Model;
using Api.Repository;

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
        CancellationToken ct)
    {
        if (!req.EhValido(id))
            return Results.BadRequest("Requisicao inválida!");

        var cliente = await repository.ObterClienteAsync(id, ct);

        if (cliente is null)
            return Results.NotFound("Cliente nao encontrado!");

        switch (req.Tipo)
        {
            case "d":
                cliente.Saldo -= req.Valor;
                if (Math.Abs(cliente.Saldo) > cliente.Limite)
                    return Results.UnprocessableEntity();
                break;
            default:
                cliente.Saldo += req.Valor;
                break;
        }
        
        // cliente.Transacoes.Add(new Transacao(req.Descricao, req.Valor, req.Tipo,  id));
        //
        // await repository.Atualizar(cliente, ct);
        //
        // var ret = await repository.SalvarAlteracoesAsync(ct);
        // if (ret < 1)
        //     return Results.StatusCode(503);

        var result = new TransacaoResponse
        {
            Saldo = cliente.Saldo,
            Limite = cliente.Limite
        };

        return Results.Ok(result);
    }
}