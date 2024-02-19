using Api.Model;
using Api.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FunctionalTests;

public class CustomApiFactory : WebApplicationFactory<IApiMarker>
{
    public HttpClient Client { get; }
    
    public RinhaDbContext DbContext { get; private set; }

    public CustomApiFactory()
    {
        Client = CreateClient();
    }

    /// <summary>
    /// Overriding CreateHost to avoid creating a separate ServiceProvider
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = builder.Build();
        host.Start();

        var serviceProvider = host.Services;
        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<RinhaDbContext>();
        
        DbContext = db;
        
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        PopulateTestData(db);
        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<RinhaDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                var inMemoryCollectionName = Guid.NewGuid().ToString();
                
                services.AddDbContext<RinhaDbContext>(options =>
                {
                    options.UseSqlite("Data Source=studyway-test.sqlite");
                });
            });
        
         builder.UseEnvironment("Test");
    }
    
    private void PopulateTestData(RinhaDbContext dbContext)
    {
        dbContext.RemoveRange(dbContext.Clientes);
        dbContext.RemoveRange(dbContext.Transacoes);

        dbContext.SaveChanges();
            
        dbContext.AddRange(new List<Cliente>
        {
            new Cliente(1, 100000, 0) { Transacoes = new List<Transacao>()
            {
                 new Transacao("Descicao test 1", 1000, "c", 1),
                 new Transacao("Descicao test 2", 1000, "d", 1)
            }},
            new Cliente(2, 80000, 0),
            new Cliente(3, 1000000, 0),
            new Cliente(4, 10000000, 0),
            new Cliente(5, 500000, 0)
        });

        dbContext.SaveChanges();
    }
}