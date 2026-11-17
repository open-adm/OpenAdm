using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.PaginateDto;

namespace OpenAdm.Infra.Paginacao;

public sealed class PaginacaoKitDto : FilterModel<Kit>
{
    public bool? Ativo { get; set; }

    public override Expression<Func<Kit, bool>> GetWhereBySearch()
    {
        var nome = Search?.Trim();
        return kit =>
            (Ativo.HasValue
                ? kit.Ativo == Ativo.Value
                : ListarInativo || kit.Ativo)
            && (string.IsNullOrWhiteSpace(nome) ||
                EF.Functions.ILike(EF.Functions.Unaccent(kit.Nome), $"%{nome}%"));
    }
}
