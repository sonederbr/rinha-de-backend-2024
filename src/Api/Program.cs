using Api;
using Api.Contratos;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppSettingsEnvironment();

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("RinhaDbContext")!);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, SourceGenerationContext.Default);
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

var dbFuncs = new Dictionary<string, string>
{
    { "c", "credita" },
    { "d", "debita" }
};

app.MapGet("/clientes/{id}/extrato", async (
    int id, 
    NpgsqlConnection connection,
    CancellationToken ct) =>
{
    if (id is < 1 or > 5)
        return Results.NotFound();

    await using (connection)
    {
        await connection.OpenAsync(ct);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM obter_extrato_cliente($1);";
        cmd.Parameters.AddWithValue(id);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);

        var response = new ExtratoResponse
        {
            ExtratoSaldo = new()
            {
                Total = reader.GetInt32(0),
                Limite = reader.GetInt32(1),
                DataExtrato = reader.GetDateTime(2)
            }
        };

        while (await reader.ReadAsync(ct))
            response.AdicionarTransacao(new ExtratoTransacaoClienteResponse()
            {
                Valor = reader.GetInt32(3),
                Tipo = reader.GetString(4),
                Descricao = reader.GetString(5),
                RealizadaEm = reader.GetDateTime(6),
            });

        return Results.Ok(response);
    }
});

app.MapPost("/clientes/{id}/transacoes", async (
    int id,
    TransacaoRequest req,
    NpgsqlConnection connection,
    CancellationToken ct) =>
{
    if (id is < 1 or > 5)
        return Results.NotFound();

    if (!req.EhValido())
        return Results.UnprocessableEntity();

    await using (connection)
    {
        await connection.OpenAsync(ct);
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT saldo_atualizado, limite_atual, linhas_afetadas FROM {dbFuncs[req.Tipo]}_saldo_cliente_e_insere_transacao($1, $2, $3);";
        cmd.Parameters.AddWithValue(id);
        cmd.Parameters.AddWithValue(req.Valor);
        cmd.Parameters.AddWithValue(req.Descricao); 
        using var reader = await cmd.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);

        if (req.Tipo.Equals("c"))
            return Results.Ok(new TransacaoResponse(saldo: reader.GetInt32(0), limite: reader.GetInt32(1)));
        
        return reader.GetInt16(2) == 0
            ? Results.UnprocessableEntity()
            : Results.Ok(new TransacaoResponse(saldo: reader.GetInt32(0), limite: reader.GetInt32(1)));
    }
});

app.Run();