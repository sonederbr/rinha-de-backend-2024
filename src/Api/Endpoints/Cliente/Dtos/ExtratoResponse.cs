namespace Api.Endpoints.Cliente.Dtos;

public class ExtratoResponse
{
    [JsonPropertyName("saldo")]
    public SaldoCliente Saldo { get; set; }

    [JsonPropertyName("ultimas_transacoes")]
    public List<TransacaoCliente> UltimasTransacoes { get; set; }
    
    public class SaldoCliente
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("data_extrato")]
        public DateTime DataExtrato { get; set; }

        [JsonPropertyName("limite")]
        public int Limite { get; set; }
    }

    public class TransacaoCliente
    {
        [JsonPropertyName("valor")]
        public int Valor { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }

        [JsonPropertyName("realizada_em")]
        public DateTime RealizadaEm { get; set; }
    }
}
