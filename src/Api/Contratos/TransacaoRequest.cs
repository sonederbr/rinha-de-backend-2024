namespace Api.Contratos;

public class TransacaoRequest
{
    private static readonly string[] TiposValidos = ["c", "d"];
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