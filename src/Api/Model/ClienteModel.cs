namespace Api.Model;

public readonly record struct Cliente(int Id, int Limite, int Saldo)
{
    public ICollection<Transacao> Transacoes { get; } = new List<Transacao>();
    public void Add(Transacao transacao) => Transacoes.Add(transacao);
}

public readonly record struct Transacao(string Descricao, int Valor, string Tipo)
{
    public DateTime DataTransacao { get; } = DateTime.UtcNow;
}

public readonly record struct ClienteModel(int Limite, int Saldo);

public readonly record struct Crebitar(int IdCliente, int Valor, string Tipo, string Descricao);