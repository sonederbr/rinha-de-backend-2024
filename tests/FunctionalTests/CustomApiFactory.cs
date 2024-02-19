using Api.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace FunctionalTests;

public class CustomApiFactory : WebApplicationFactory<IApiMarker>
{
    public HttpClient Client { get; }
    
    public NpgsqlConnection Connection { get; private set; }

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
        var connection = scopedServices.GetRequiredService<NpgsqlConnection>();
        
        Connection = connection;

        PopulateTestData();
        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
         builder.UseEnvironment("Test");
    }
    
    private void PopulateTestData()
    {
        var clientes = new List<Cliente>
        {
            new Cliente(1, 100000, 0),
            new Cliente(2, 80000, 0),
            new Cliente(3, 1000000, 0),
            new Cliente(4, 10000000, 0),
            new Cliente(5, 500000, 0)
        };
        
        using (Connection)
        {
            Connection.Open();
            using (var cmd1 = Connection.CreateCommand())
            {
                cmd1.CommandText = "TRUNCATE TABLE cliente;";
                cmd1.ExecuteNonQuery();
            }
            
            using (var cmd2 = Connection.CreateCommand())
            {
                cmd2.CommandText = "TRUNCATE TABLE transacao;";
                cmd2.ExecuteNonQuery();
            }
            
            foreach (var cliente in clientes)
            {
                using var cmd = Connection.CreateCommand();
                cmd.CommandText = $"INSERT INTO cliente (id, limite, saldo) VALUES ($1, $2, $3) ";
                cmd.Parameters.AddWithValue(cliente.Id);
                cmd.Parameters.AddWithValue(cliente.Limite);
                cmd.Parameters.AddWithValue(cliente.Saldo);
                
                var result = cmd.ExecuteNonQuery();
            }
        }
    }
}