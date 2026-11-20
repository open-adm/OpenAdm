using Microsoft.EntityFrameworkCore;
using OpenAdm.Data.Context;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Interfaces;

namespace OpenAdm.Infra.Repositories;

public class AcessoEcommerceRepository : IAcessoEcommerceRepository
{
    private readonly AppDbContext _appDbContext;

    public AcessoEcommerceRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task AddAsync(AcessoEcommerce acessoEcommerce)
    {
        await _appDbContext.AddAsync(acessoEcommerce);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task EditAsync(AcessoEcommerce acessoEcommerce)
    {
        await _appDbContext
            .AcessosEcommerce
            .Where(x => x.Id == acessoEcommerce.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.Quantidade, acessoEcommerce.Quantidade));
    }

    public Task<AcessoEcommerce?> ObterPorDataAsync(int ano, int mes, Guid parceiroId)
    {
        return ObterAcessosDoMes(ano, mes, parceiroId).FirstOrDefaultAsync();
    }

    public Task<long> ObterQuantidadePorDataAsync(int ano, int mes, Guid parceiroId)
    {
        return ObterAcessosDoMes(ano, mes, parceiroId)
            .Select(x => x.Quantidade)
            .FirstOrDefaultAsync();
    }

    private IQueryable<AcessoEcommerce> ObterAcessosDoMes(int ano, int mes, Guid parceiroId)
    {
        var inicio = new DateTime(ano, mes, 1);
        var fim = inicio.AddMonths(1);

        return _appDbContext.AcessosEcommerce
            .Where(x => x.ParceiroId == parceiroId && x.DataDeCriacao >= inicio && x.DataDeCriacao < fim);
    }
}
