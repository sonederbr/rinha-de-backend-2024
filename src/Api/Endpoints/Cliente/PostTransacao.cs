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
        [FromServices] RinhaRepository repository,
        CancellationToken ct)
    {
        var transacao = new Transacao()
        {
            Valor = req.Valor,
            Descricao = req.Descricao,
            DataTransacao = DateTime.UtcNow,
            ClienteId = id
        };
        
        var cliente = await repository.GetCliente(id);
        
        if (req.Tipo == 'c')
            cliente.Saldo += req.Valor;
        else
            cliente.Saldo -= req.Valor;

        if (cliente.Saldo * -1 > cliente.Limite)
            return Results.StatusCode(422);

        if(await repository.AtualizarCliente(cliente) < 1)
            return Results.Problem(statusCode: 500);
        
        if(await repository.NovaTransacao(transacao) < 1)
            return Results.Problem(statusCode: 501);
        
        cliente = await repository.GetCliente(id);
        
        var result = new TransacaoResponse
        {
            Saldo = cliente.Saldo,
            Limite = cliente.Limite
        };

        return Results.Ok(result);
    }
}