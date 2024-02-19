namespace Api.Endpoints.Cliente.Dtos;

public class TransacaoRequest
{
    private static readonly string[] TiposValidos = new[] { "c", "d" };

    [JsonPropertyName("valor")]
    public int Valor { get; set; }

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; }

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; }

    public bool EhValido(int id)
    {
        return id > 0 && 
               TiposValidos.Contains(Tipo) 
               && !string.IsNullOrWhiteSpace(Descricao) && Descricao.Length <= 0 &&
               Valor > 0;
    }
}