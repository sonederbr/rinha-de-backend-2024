using Api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Repository.Configuration;

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

        builder
            .HasMany(e => e.Transacoes)
            .WithOne(e => e.Cliente)
            .HasForeignKey(e => e.ClienteId)
            .IsRequired();
        
        builder.ToTable("cliente");
    }
}