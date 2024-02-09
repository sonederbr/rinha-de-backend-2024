using Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Repository;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to postgres with connection string from app settings
        // https://stackoverflow.com/questions/69941444/how-to-have-docker-compose-init-a-sql-server-database
        options.UseNpgsql(Configuration.GetConnectionString("ConnectionString"));
    }

    public DbSet<Cliente> Clientes { get; set; }
}