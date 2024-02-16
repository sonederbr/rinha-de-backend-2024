using Api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Repository.Configuration;

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

        builder.Property(p => p.Tipo)
            .HasColumnName("tipo")
            .HasMaxLength(1)
            .IsRequired();
        
        builder.Property(p => p.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(p => p.DataTransacao)
            .HasColumnName("realizada_em")
            .IsRequired();
        
        builder.Property(p => p.ClienteId)
            .HasColumnName("idcliente")
            .IsRequired();
            
        builder
            .HasOne(e => e.Cliente)
            .WithMany(e => e.Transacoes)
            .HasForeignKey(e => e.ClienteId)
            .IsRequired();

        builder.ToTable("transacao");
    }
}