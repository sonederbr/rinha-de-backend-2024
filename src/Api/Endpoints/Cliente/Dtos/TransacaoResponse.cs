namespace Api.Endpoints.Cliente.Dtos;

public class TransacaoResponse(int saldo, int limite)
{
    [JsonPropertyName("limite")]
    public int Limite { get; set; } = saldo;

    [JsonPropertyName("valor")] 
    public int Saldo { get; set; } = limite;
}