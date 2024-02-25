using Api.Endpoints.Cliente.Dtos;
using Ardalis.HttpClientTestExtensions;
using FluentAssertions;

namespace FunctionalTests;

public class ObterExtratoEndpoint : IClassFixture<CustomApiFactory>
{
    private readonly HttpClient _client;
    
    public ObterExtratoEndpoint(CustomApiFactory factory)
    {
        _client = factory.Client;
    }

    [Fact]
    public async Task ObterExtratoPorClienteAsyncEndpointComSucesso()
    {
        // act
        var response = await _client.GetAndDeserializeAsync<ExtratoResponse>("/clientes/1/extrato");

        // assert
        response.ExtratoSaldo.Should().NotBeNull();
        response.ExtratoSaldo.Total.Should().Be(0);
        response.ExtratoSaldo.Limite.Should().Be(100000);
        response.UltimasTransacoes.Count.Should().Be(0);
    }
}