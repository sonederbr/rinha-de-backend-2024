using Api.Endpoints.Cliente;
using Api.Model;
using Api.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppSettingsEnvironment();

builder.Services.AddDbContext<RinhaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RinhaDbContext")));

builder.Services.AddScoped(typeof(ClienteRepository));

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