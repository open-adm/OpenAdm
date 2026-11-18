using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenAdm.Domain.Entities;

namespace OpenAdm.Data.EntityConfiguration;

public sealed class AvisoUsuarioConfiguration : IEntityTypeConfiguration<AvisoUsuario>
{
    public void Configure(EntityTypeBuilder<AvisoUsuario> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Mensagem).IsRequired().HasMaxLength(AvisoUsuario.MensagemMaxLength);
        builder.Property(x => x.Ativo).HasDefaultValue(true);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UsuarioId, x.DataDeCriacao, x.Id });
    }
}
