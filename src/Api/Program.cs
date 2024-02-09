using Api.Endpoints.Cliente;
using Api.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppSettingsEnvironment();
builder.Services.AddDbContext<DataContext>();
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMySwagger(builder);
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
app.UseMySwagger();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();

app.AddCriarTransacaoPorClienteEndpoint(); // POST /clientes/[id]/transacoes
app.AddExtratoPorClienteEndpoint(); // GET /clientes/[id]/extrato 

app.Run();