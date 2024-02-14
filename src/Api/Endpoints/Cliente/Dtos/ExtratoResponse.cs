namespace Api.Endpoints.Cliente.Dtos;

public class ExtratoResponse
{
    public SaldoCliente Saldo { get; set; }
    public List<TransacaoCliente> UltimasTransacoes { get; set; }
    
    public class SaldoCliente
    {
        public int Total { get; set; }
        public DateTime DataExtrato { get; set; }
        public int Limite { get; set; }
    }

    public class TransacaoCliente
    {
        public int Valor { get; set; }
        public char Tipo { get; set; }
        public string? Descricao { get; set; }
        public DateTime RealizadaEm { get; set; }
    }
}
