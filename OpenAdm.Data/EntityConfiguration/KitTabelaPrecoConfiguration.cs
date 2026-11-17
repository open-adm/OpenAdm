using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenAdm.Domain.Entities;

namespace OpenAdm.Data.EntityConfiguration;

public class KitTabelaPrecoConfiguration : IEntityTypeConfiguration<KitTabelaPreco>
{
    public void Configure(EntityTypeBuilder<KitTabelaPreco> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Ativo)
            .IsRequired()
            .HasDefaultValue(true);
        builder.Property(x => x.PrecoPessoaFisica)
            .HasPrecision(12, 2);
        builder.Property(x => x.PrecoPessoaJuridica)
            .HasPrecision(12, 2);

        builder.HasIndex(x => new { x.KitId, x.TabelaPrecoId })
            .HasFilter("\"Ativo\" = true")
            .IsUnique();

        builder.HasOne(x => x.TabelaPreco)
            .WithMany()
            .HasForeignKey(x => x.TabelaPrecoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
