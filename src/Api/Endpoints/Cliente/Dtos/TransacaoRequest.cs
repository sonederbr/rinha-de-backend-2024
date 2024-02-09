namespace Api.Endpoints.Cliente.Dtos;

public class TransacaoRequest
{
    public int Valor { get; set; }
    public char Tipo { get; set; }
    public string Descricao { get; set; }
}