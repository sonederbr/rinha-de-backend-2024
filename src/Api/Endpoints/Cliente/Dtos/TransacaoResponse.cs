namespace Api.Endpoints.Cliente.Dtos;

public class TransacaoResponse
{
    [JsonPropertyName("limite")]
    public int Limite { get; set; }

    [JsonPropertyName("valor")]
    public int Saldo { get; set; }
}