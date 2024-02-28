namespace Api.Contratos;

public record ExtratoResponse(ExtratoSaldoClienteResponse Saldo, IList<ExtratoTransacaoClienteResponse> UltimasTransacoes);

public record ExtratoSaldoClienteResponse(int Total, DateTime DataExtrato, int Limite);

public record ExtratoTransacaoClienteResponse(int Valor, string Tipo, string Descricao, DateTime RealizadaEm);
