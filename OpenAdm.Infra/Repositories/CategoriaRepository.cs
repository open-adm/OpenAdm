using OpenAdm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OpenAdm.Data.Context;
using OpenAdm.Domain.Interfaces;
using OpenAdm.Domain.Model;
using OpenAdm.Domain.PaginateDto;
using System.Linq.Expressions;

namespace OpenAdm.Infra.Repositories;

public class CategoriaRepository(ParceiroContext parceiroContext)
    : GenericRepository<Categoria>(parceiroContext), ICategoriaRepository
{
    public async Task<int> GetMaxOrdemExibicaoAsync()
        => await ParceiroContext.Categorias.MaxAsync(x => (int?)x.OrdemExibixao) ?? 0;

    public async Task<Categoria?> GetCategoriaAsync(Guid id)
    {
        return await ParceiroContext
            .Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<Categoria?> GetCategoriaParaAtualizarOrdemAsync(Guid id)
    {
        return ParceiroContext
            .Categorias
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IList<Categoria>> GetCategoriasAsync()
    {
        var categorias = await ParceiroContext
            .Categorias
            .AsQueryable()
            .AsNoTracking()
            .OrderBy(c => c.OrdemExibixao)
            .Include(x => x.Produtos.Where(y => y.Ativo))
            .Where(x => !x.InativoEcommerce && x.Produtos.Any(y => y.Ativo))
            .ToListAsync();

        foreach (var categoria in categorias)
        {
            categoria.Produtos = [.. categoria
                .Produtos
                .Select(x =>
                    new Produto(
                        x.Id,
                        x.DataDeCriacao,
                        x.DataDeAtualizacao,
                        x.Numero,
                        x.Descricao,
                        x.EspecificacaoTecnica,
                        x.CategoriaId,
                        x.Referencia,
                        x.UrlFoto,
                        x.NomeFoto,
                        x.VendaSomenteComEstoqueDisponivel,
                        x.Ativo,
                        x.OrdemExibixao))
                .OrderBy(x => x.Numero)
                .Take(3)];
        }

        return categorias;
    }

    public async Task<IList<OrdemExibicaoModel>> GetCategoriasParaOrdemExibicaoAsync()
    {
        return await ParceiroContext
            .Categorias
            .AsNoTracking()
            .OrderBy(x => x.OrdemExibixao)
            .ThenBy(x => x.Numero)
            .Select(x => new OrdemExibicaoModel
            {
                Id = x.Id,
                Descricao = x.Descricao,
                Foto = x.Foto,
                OrdemExibicao = x.OrdemExibixao
            })
            .ToListAsync();
    }

    public Task<IList<DropDownItemModel>> BuscarDropDownAsync(DropDownFiltro filtro)
    {
        var search = filtro.Search?.Trim();
        Expression<Func<Categoria, bool>>? where = string.IsNullOrWhiteSpace(search)
            ? null
            : x => EF.Functions.ILike(EF.Functions.Unaccent(x.Descricao), $"%{search}%");

        return BuscarDropDownAsync(
            filtro,
            where,
            x => x.Descricao,
            x => x.Id,
            x => new DropDownItemModel { Id = x.Id, Descricao = x.Descricao });
    }
}
