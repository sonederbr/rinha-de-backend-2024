namespace Api.Model;

public class Transacao(
    string descricao,
    int valor,
    string tipo)
{
    public int Valor { get; set; } = valor;
    public string Descricao { get; set; } = descricao;
    public string Tipo { get; set; } = tipo;
    public DateTime DataTransacao { get; set; } = DateTime.UtcNow;
}