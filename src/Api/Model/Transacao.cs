namespace Api.Model;

public class Transacao
{
    public int Id { get; set; }
    public int Valor { get; set; }
    public string Descricao { get; set; }
    public DateTime DataTransacao { get; set; }
    public int ClienteId { get; set; }
}