using Api.Contratos;

namespace Api;

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(ExtratoResponse))]
[JsonSerializable(typeof(ExtratoSaldoClienteResponse))]
[JsonSerializable(typeof(ExtratoTransacaoClienteResponse))]
[JsonSerializable(typeof(IList<ExtratoTransacaoClienteResponse>))]
[JsonSerializable(typeof(TransacaoRequest))]
[JsonSerializable(typeof(TransacaoResponse))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(int))]
public partial class SourceGenerationContext : JsonSerializerContext { }
