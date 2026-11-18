using OpenAdm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using OpenAdm.Domain.Extensions;
using OpenAdm.Domain.PaginateDto;

namespace OpenAdm.Infra.Paginacao;

public class PaginacaoMovimentacaoDeProdutoDto : FilterModel<MovimentacaoDeProduto>
{
    public override Expression<Func<MovimentacaoDeProduto, bool>>? GetWhereBySearch()
    {
        if (string.IsNullOrWhiteSpace(Search))
            return null;

        var search = Search.RemoverAcentos().Trim();
        
        return x => EF.Functions.ILike(EF.Functions.Unaccent(x.Produto.Descricao), $"%{search}%");
    }
}
