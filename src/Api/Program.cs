using Api.Endpoints.Cliente;
using Api.Model;
using Api.Repository;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppSettingsEnvironment();

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("RinhaDbContext")!);

builder.Services.AddScoped(typeof(ClienteRepository));

builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddMySwagger(builder);
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddMemoryCache();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
    app.UseMySwagger();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();

app.AddCriarTransacaoPorClienteEndpoint(); // POST /clientes/[id]/transacoes
app.AddExtratoPorClienteEndpoint(); // GET /clientes/[id]/extrato 

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var repository = serviceScope.ServiceProvider.GetService<ClienteRepository>();
    var cache = serviceScope.ServiceProvider.GetService<IMemoryCache>();
    var entryOptions = new MemoryCacheEntryOptions { Size = 5 }.SetPriority(CacheItemPriority.NeverRemove);
    // foreach (var cliente in await repository!.ObterTodosClientesAsync())
    //     cache!.Set(cliente.Id, cliente, entryOptions);
    Console.WriteLine("Repository is null >>>>>>>" + repository == null);
    Console.WriteLine("Cache is null >>>>>>>" + cache == null);

    cache.Set(1, new Cliente(1, 100000, 0));
    cache.Set(2, new Cliente(2, 80000, 0));
    cache.Set(3, new Cliente(3, 1000000, 0));
    cache.Set(4, new Cliente(4, 10000000, 0));
    cache.Set(5, new Cliente(5, 500000, 0));
}

app.Run();
