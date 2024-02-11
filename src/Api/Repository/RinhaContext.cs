using Api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Repository;

public class RinhaContext(DbContextOptions<RinhaContext> options) : DbContext(options)
{
    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    // {
    //     // connect to postgres with connection string from app settings
    //     // https://stackoverflow.com/questions/69941444/how-to-have-docker-compose-init-a-sql-server-database
    //     options.UseNpgsql(Configuration.GetConnectionString("ConnectionString"));
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();
    }
    
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
}

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(p => p.Saldo)
            .HasColumnName("saldo")
            .IsRequired();

        builder.Property(p => p.Limite)
            .HasColumnName("limite")
            .IsRequired();

        builder.ToTable("cliente");
    }
}

public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(p => p.Valor)
            .HasColumnName("valor")
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasColumnName("descricao")
            .IsRequired();
        
        builder.Property(p => p.DataTransacao)
            .HasColumnName("realizada_em")
            .IsRequired();
        
        builder.Property(p => p.ClienteId)
            .HasColumnName("idcliente")
            .IsRequired();


        builder.ToTable("transacao");
    }
}

public static class ModelBuilderExtensions
{
    public static void ApplyAllConfigurationsFromCurrentAssembly(
        this ModelBuilder modelBuilder,
        Assembly? assembly = null,
        string configNamespace = "")
    {
        if (assembly == (Assembly) null)
            assembly = Assembly.GetCallingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
}