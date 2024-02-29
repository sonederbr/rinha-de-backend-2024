using Api;
using Api.Endpoints;
using Api.Middlewares;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppSettingsEnvironment();

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("RinhaDbContext")!, ServiceLifetime.Scoped);

builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, SourceGenerationContext.Default);
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddMySwagger(builder);
}

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
    app.UseMySwagger();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.AddObterExtratoPorClienteEndpoint();    // GET  /clientes/[id]/extrato 
app.AddCriarTransacaoPorClienteEndpoint();  // POST /clientes/[id]/transacoes
app.AddExecutarManutencaoBdEndpoint();      // POST /manutencao/reseta-bd

app.Run();
