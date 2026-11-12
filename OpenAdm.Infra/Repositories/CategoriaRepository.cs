using OpenAdm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OpenAdm.Data.Context;
using OpenAdm.Domain.Interfaces;

namespace OpenAdm.Infra.Repositories;

public class CategoriaRepository(ParceiroContext parceiroContext)
    : GenericRepository<Categoria>(parceiroContext), ICategoriaRepository
{
    public async Task<Categoria?> GetCategoriaAsync(Guid id)
    {
        return await ParceiroContext
            .Categorias
            .AsNoTracking()
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

    public async Task<IList<Categoria>> GetCategoriasDropDownAsync()
    {
        return await ParceiroContext
            .Categorias
            .AsNoTracking()
            .ToListAsync();
    }
}
