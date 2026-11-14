using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenAdm.Domain.Entities;

namespace OpenAdm.Data.EntityConfiguration;

public class KitItemConfiguration : IEntityTypeConfiguration<KitItem>
{
    public void Configure(EntityTypeBuilder<KitItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Ativo)
            .IsRequired()
            .HasDefaultValue(true);
        builder.Property(x => x.Quantidade)
            .IsRequired();

        builder.HasIndex(x => new
            {
                x.KitId,
                x.ProdutoId,
                x.TamanhoId,
                x.PesoId
            })
            .HasFilter("\"Ativo\" = true")
            .IsUnique()
            .AreNullsDistinct(false);

        builder.HasOne(x => x.Produto)
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Tamanho)
            .WithMany()
            .HasForeignKey(x => x.TamanhoId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Peso)
            .WithMany()
            .HasForeignKey(x => x.PesoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}