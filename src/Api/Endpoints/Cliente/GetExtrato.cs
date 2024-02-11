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
        [FromServices] RinhaRepository repository,
        CancellationToken ct)
    {
        var cliente = await repository.GetCliente(id);
        if (cliente is null)
            return Results.NotFound("Cliente não encontrado!");

        var transacoes = await repository.GetTransacoes(id);
        
        var transacoesCliente = new List<ExtratoResponse.TransacaoCliente>();
        
        foreach (var transacao in transacoes)
        {
            transacoesCliente.Add(new ExtratoResponse.TransacaoCliente()
            {
                Descricao = transacao.Descricao,
                RealizadaEm = transacao.DataTransacao,
                Tipo = 'D',
                Valor = transacao.Valor
            });
        }
            
        var result = new ExtratoResponse
        {
            Saldo = new()
            {
                Total = cliente.Saldo,
                DataExtrato = DateTime.UtcNow,
                Limite = cliente.Limite
            },
            UltimasTransacoes = transacoesCliente
        };
        return Results.Ok(result);
    }
}