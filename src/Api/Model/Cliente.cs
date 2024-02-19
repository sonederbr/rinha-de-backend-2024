namespace Api.Model;

public class Cliente(int id, int limite, int saldo)
{
    public int Id { get; set; } = id;
    public int Limite { get; set; } = limite;
    public int Saldo { get; set; } = saldo;
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}