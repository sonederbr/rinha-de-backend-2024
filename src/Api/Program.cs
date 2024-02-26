using Api;
using Api.Contratos;
using Npgsql;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppSettingsEnvironment();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, SourceGenerationContext.Default);
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("RinhaDbContext")!);

var app = builder.Build();

app.UseMetricServer();
app.UseHttpMetrics();

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
        await connection.OpenAsync();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText =  "SELECT * FROM obter_extrato_cliente($1);";
        cmd.Parameters.AddWithValue(id);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        ExtratoSaldoClienteResponse extrato = new(reader.GetInt32(0), reader.GetDateTime(2), reader.GetInt32(1));
        var transacoes = new List<ExtratoTransacaoClienteResponse>();
        while (await reader.ReadAsync())
            transacoes.Add(
                new(reader.GetInt32(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetDateTime(6)));

        return Results.Ok(new ExtratoResponse(extrato, transacoes));
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
        await connection.OpenAsync();
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT saldo_atualizado, limite_atual, linhas_afetadas FROM {dbFuncs[req.Tipo]}_saldo_cliente_e_insere_transacao($1, $2, $3);";
        cmd.Parameters.AddWithValue(id);
        cmd.Parameters.AddWithValue(req.Valor);
        cmd.Parameters.AddWithValue(req.Descricao);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return reader.GetInt16(2) == 0 
            ? Results.UnprocessableEntity() 
            : Results.Ok(new TransacaoResponse(reader.GetInt32(0), reader.GetInt32(1)));
    }
});

app.MapGet("/admin/db-reset", async (NpgsqlConnection conn) =>
{
    await using (conn)
    {
        await conn.OpenAsync();
        await using var cmd = conn.CreateBatch();
        cmd.BatchCommands.Add(new NpgsqlBatchCommand("update saldo_cliente set saldo = 0"));
        cmd.BatchCommands.Add(new NpgsqlBatchCommand("truncate table transacao_cliente"));
        await using var reader = await cmd.ExecuteReaderAsync();
        return Results.Ok("db reset!");
    }
});

app.Run();