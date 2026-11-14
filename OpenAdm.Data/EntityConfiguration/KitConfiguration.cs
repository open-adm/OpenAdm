using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenAdm.Data.Constants;
using OpenAdm.Domain.Entities;

namespace OpenAdm.Data.EntityConfiguration;

public class KitConfiguration : IEntityTypeConfiguration<Kit>
{
    public void Configure(EntityTypeBuilder<Kit> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DataDeCriacao)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()");
        builder.Property(x => x.DataDeAtualizacao)
            .IsRequired()
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("now()");
        builder.Property(x => x.Numero)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(KitMaxLength.Nome);
        builder.Property(x => x.Descricao)
            .HasMaxLength(KitMaxLength.Descricao);
        builder.Property(x => x.Imagem)
            .HasMaxLength(KitMaxLength.Imagem);
        builder.Property(x => x.Ativo)
            .IsRequired()
            .HasDefaultValue(true);
        builder.Property(x => x.VenderPessoaFisica)
            .IsRequired();
        builder.Property(x => x.VenderPessoaJuridica)
            .IsRequired();

        builder.HasMany(x => x.Itens)
            .WithOne(x => x.Kit)
            .HasForeignKey(x => x.KitId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.Precos)
            .WithOne(x => x.Kit)
            .HasForeignKey(x => x.KitId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
