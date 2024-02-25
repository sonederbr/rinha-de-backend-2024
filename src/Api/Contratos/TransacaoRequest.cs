namespace Api.Contratos;

public class TransacaoRequest
{
    private static readonly string[] TiposValidos = new[] { "c", "d" };

    [JsonPropertyName("valor")]
    public int Valor { get; set; }

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; }

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; }

    public bool EhValido()
    {
        return TiposValidos.Contains(Tipo) 
               && !string.IsNullOrWhiteSpace(Descricao) && Descricao.Length <= 10 &&
               Valor > 0;
    }
}