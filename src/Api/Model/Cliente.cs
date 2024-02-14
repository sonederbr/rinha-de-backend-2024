namespace Api.Model;

public class Cliente(int limite, int saldo)
{
    protected Cliente() : this(default, default)
    {
    }
    
    public int Id { get; set; }
    public int Limite { get; set; } = limite;
    public int Saldo { get; set; } = saldo;

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}