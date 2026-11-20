using Microsoft.EntityFrameworkCore;
using OpenAdm.Domain.PaginateDto;

namespace OpenAdm.Infra.Extensions.IQueryable;

public static class CustomFilter
{
    public static async Task<(int TotalPaginas, int TotalDeRegistros, IList<TResult> Values)> ProjetarEPaginarAsync<TEntity, TResult>(
        this IQueryable<TEntity> query,
        FilterModel<TEntity> filterModel,
        System.Linq.Expressions.Expression<Func<TEntity, TResult>> select)
        where TEntity : class
        where TResult : class
    {
        filterModel.ValidarEObterPropriedadeDeOrdenacao();
        var coluna = filterModel.OrderBy[..1].ToUpper() + filterModel.OrderBy[1..];
        var orderBy = filterModel.OrderByCustom() ?? (x => EF.Property<TEntity>(x!, coluna)!);
        var thenOrderBy = filterModel.ThenOrderByCustom();

        IOrderedQueryable<TEntity> orderedQuery;
        if (filterModel.Asc)
        {
            orderedQuery = thenOrderBy == null
                ? query.OrderBy(orderBy)
                : query.OrderBy(orderBy).ThenBy(thenOrderBy);
        }
        else
        {
            orderedQuery = thenOrderBy == null
                ? query.OrderByDescending(orderBy)
                : query.OrderByDescending(orderBy).ThenByDescending(thenOrderBy);
        }

        var totalDeRegistros = await query.CountAsync();
        var totalPaginas = CalcularTotalDePaginas(totalDeRegistros, filterModel.Take);
        var values = await orderedQuery
            .Paginate(filterModel.Skip, filterModel.Take)
            .Select(select)
            .ToListAsync();

        return (totalPaginas, totalDeRegistros, values);
    }

    public static async Task<(int TotalPaginas, IList<TEntity> Values)> CustomFilterAsync<TEntity>(
        this IQueryable<TEntity> querable, FilterModel<TEntity> filterModel)
    {
        var totalDeRegistros = await querable.CountAsync();
        return await querable.CustomFilterAsync(filterModel, totalDeRegistros);
    }

    public static async Task<(int TotalPaginas, IList<TEntity> Values)> CustomFilterAsync<TEntity>(
        this IQueryable<TEntity> querable,
        FilterModel<TEntity> filterModel,
        int totalDeRegistros)
    {
        var totalPaginas = CalcularTotalDePaginas(totalDeRegistros, filterModel.Take);

        var coluna = filterModel.OrderBy[..1].ToUpper() + filterModel.OrderBy[1..];
        var orderBy = filterModel.OrderByCustom();
        var thenOrderBy = filterModel.ThenOrderByCustom();

        orderBy ??= x => EF.Property<TEntity>(x!, coluna)!;

        if (thenOrderBy != null)
        {
            querable = filterModel.Asc ? querable.OrderBy(orderBy).ThenBy(thenOrderBy) : querable.OrderByDescending(orderBy).ThenByDescending(thenOrderBy);
        }
        else
        {
            querable = filterModel.Asc ? querable.OrderBy(orderBy) : querable.OrderByDescending(orderBy);
        }

        var values = await querable
            .Paginate(filterModel.Skip, filterModel.Take)
            .ToListAsync();

        return (totalPaginas, values);
    }

    private static int CalcularTotalDePaginas(int totalDeRegistros, int take)
        => (int)Math.Ceiling((decimal)totalDeRegistros / take);

    public static async Task<int> CountCustomAsync<TEntity>(this IQueryable<TEntity> querable, int take)
    {
        if (take <= 0)
            throw new ArgumentOutOfRangeException(nameof(take), "Take deve ser maior que zero.");

        var count = await querable.CountAsync();
        return CalcularTotalDePaginas(count, take);
    }
}
