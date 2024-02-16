namespace Api.Endpoints.Cliente.Dtos;

public class TransacaoRequest
{
    [JsonPropertyName("valor")]
    public int Valor { get; set; }

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; }

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; }
}