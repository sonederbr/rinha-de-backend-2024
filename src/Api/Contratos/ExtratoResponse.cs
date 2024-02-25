namespace Api.Contratos;

public class ExtratoResponse
{
    [JsonPropertyName("saldo")]
    public ExtratoSaldoClienteResponse ExtratoSaldo { get; set; }

    [JsonPropertyName("ultimas_transacoes")]
    public IList<ExtratoTransacaoClienteResponse> UltimasTransacoes { get; } = new List<ExtratoTransacaoClienteResponse>();
    
    public void AdicionarTransacao(ExtratoTransacaoClienteResponse transacao) => UltimasTransacoes.Add(transacao);
}

public class ExtratoSaldoClienteResponse
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("limite")]
    public int Limite { get; set; }
    
    [JsonPropertyName("data_extrato")]
    public DateTime DataExtrato { get; set; }
}

public class ExtratoTransacaoClienteResponse
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
