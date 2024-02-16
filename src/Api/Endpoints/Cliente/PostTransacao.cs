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
        if(id <= 0) 
            return Results.BadRequest("Id do cliente não informado!");
        
        if(req.Valor <= 0) 
            return Results.BadRequest("Valor deve ser um número inteiro positivo!");
        
        if(string.IsNullOrWhiteSpace(req.Tipo) || !req.Tipo.Equals("c", StringComparison.InvariantCultureIgnoreCase) && !req.Tipo.Equals("d", StringComparison.InvariantCultureIgnoreCase)) 
            return Results.BadRequest("Tipo deve ser c para crédito e d para débito!");
        
        if(string.IsNullOrWhiteSpace(req.Descricao) || req.Descricao?.Length < 1 || req.Descricao?.Length > 10)
            return Results.BadRequest("Descricao deve ser uma string de 1 a 10 caracteres!");

        var cliente = await repository.ObterCliente(id, ct);

        if (cliente is null)
            return Results.NotFound("Cliente nao encontrado!");

        switch (req.Tipo)
        {
            case "c":
                cliente.Saldo += req.Valor;
                break;
            case "d":
                cliente.Saldo -= req.Valor;
                break;
            default:
                return Results.BadRequest("tipo deve ser c para crédito e d para débito!");
        }

        if ((req.Tipo.Equals("d")))
        {
            if (Math.Abs(cliente.Saldo) > cliente.Limite)
                return Results.StatusCode(422);
        }
        
        cliente.Transacoes.Add(new Transacao(req.Descricao, req.Valor, req.Tipo,  id));

        await repository.Atualizar(cliente, ct);
        
        var ret = await repository.SalvarAlteracoesAsync(ct);
        if (ret < 1)
            return Results.StatusCode(503);

        var result = new TransacaoResponse
        {
            Saldo = cliente.Saldo,
            Limite = cliente.Limite
        };

        return Results.Ok(result);
    }
}