namespace Api.Model;

public class Transacao(
    string descricao,
    int valor,
    string tipo,
    int clienteId)
{
    public Transacao() : this(default, default, default, default)
    {
    }
    
    public int Id { get; set; }
    public int Valor { get; set; } = valor;
    public string Descricao { get; set; } = descricao;
    public string Tipo { get; set; } = tipo;
    public DateTime DataTransacao { get; set; } = DateTime.UtcNow;
    public int ClienteId { get; set; } = clienteId;
    public Cliente Cliente { get; set; } = null!;
}