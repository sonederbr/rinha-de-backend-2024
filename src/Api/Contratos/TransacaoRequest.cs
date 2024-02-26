namespace Api.Contratos;
//
// public class TransacaoRequest
// {
//     private static readonly string[] TiposValidos = new[] { "c", "d" };
//
//     [JsonPropertyName("valor")]
//     public int Valor { get; set; }
//
//     [JsonPropertyName("tipo")]
//     public string Tipo { get; set; }
//
//     [JsonPropertyName("descricao")]
//     public string Descricao { get; set; }
//
//     public bool EhValido()
//     {
//         return TiposValidos.Contains(Tipo) 
//                && !string.IsNullOrWhiteSpace(Descricao) && Descricao.Length <= 10 &&
//                Valor > 0;
//     }
// }

public class TransacaoRequest
{
    private readonly static string[] TiposValidos = ["c", "d"];
    public int Valor { get; set; }
    public string? Tipo { get; set; }
    public string? Descricao { get; set; }
    public bool EhValido()
    {
        return TiposValidos.Contains(Tipo)
               && !string.IsNullOrEmpty(Descricao)
               && Descricao.Length <= 10
               && Valor > 0;
    }
}