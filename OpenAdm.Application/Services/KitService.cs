using OpenAdm.Application.Dtos.Kits;
using OpenAdm.Application.HttpClient.Interfaces;
using OpenAdm.Application.Interfaces;
using OpenAdm.Application.Models;
using OpenAdm.Application.Models.Kits;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Interfaces;
using OpenAdm.Domain.Model;
using OpenAdm.Domain.PaginateDto;

namespace OpenAdm.Application.Services;

public sealed class KitService(
    IKitRepository kitRepository,
    IUploadImageBlobClient uploadImageBlobClient) : IKitService
{
    public async Task<ResultPartner<KitViewModel>> CriarAsync(KitDto dto)
    {
        var erro = await ValidarAsync(dto);
        if (erro != null)
            return (ResultPartner<KitViewModel>)erro;

        var (imagem, nomeImagem) = await ObterImagemAsync(dto.NovaFoto);
        var kit = Kit.Novo(
            dto.Nome.Trim(),
            dto.Descricao,
            imagem,
            nomeImagem,
            dto.VenderPessoaFisica,
            dto.VenderPessoaJuridica);

        foreach (var itemDto in dto.Itens)
        {
            var item = KitItem.Novo(
                kit.Id,
                itemDto.ProdutoId,
                itemDto.TamanhoId,
                itemDto.PesoId,
                itemDto.Quantidade);

            kit.Itens.Add(item);
        }

        foreach (var precoDto in dto.Precos)
        {
            var preco = KitTabelaPreco.Novo(
                kit.Id,
                precoDto.TabelaPrecoId,
                precoDto.PrecoPessoaFisica,
                precoDto.PrecoPessoaJuridica);

            kit.Precos.Add(preco);
        }

        await kitRepository.AddAsync(kit);
        return (ResultPartner<KitViewModel>)(KitViewModel)kit;
    }

    public async Task<ResultPartner<KitViewModel>> AtualizarAsync(Guid id, KitDto dto)
    {
        var erro = await ValidarAsync(dto, id);
        if (erro != null)
            return (ResultPartner<KitViewModel>)erro;

        var kit = await kitRepository.ObterAsync(id, rastrear: true);
        if (kit == null)
            return (ResultPartner<KitViewModel>)"Não foi possível localizar o kit";

        var imagem = kit.Imagem;
        var nomeImagem = kit.NomeImagem;

        if (!string.IsNullOrWhiteSpace(dto.NovaFoto))
        {
            var (novaImagem, nomeNovoBlob) = await ObterImagemAsync(dto.NovaFoto);
            var imagemAlterada = kit.Imagem != novaImagem;

            if (imagemAlterada && !string.IsNullOrWhiteSpace(kit.NomeImagem))
            {
                var imagemAnteriorExcluida = await uploadImageBlobClient
                    .DeleteImageAsync(kit.NomeImagem);

                if (!imagemAnteriorExcluida)
                {
                    if (!string.IsNullOrWhiteSpace(nomeNovoBlob))
                    {
                        await uploadImageBlobClient.DeleteImageAsync(nomeNovoBlob);
                    }

                    return (ResultPartner<KitViewModel>)
                        "Não foi possível excluir a imagem anterior";
                }
            }

            imagem = novaImagem;
            nomeImagem = imagemAlterada ? nomeNovoBlob : kit.NomeImagem;
        }

        kit.Atualizar(
            dto.Nome.Trim(),
            dto.Descricao,
            imagem,
            nomeImagem,
            kit.Ativo,
            dto.VenderPessoaFisica,
            dto.VenderPessoaJuridica);

        await AtualizarItensAsync(kit, dto.Itens);
        await AtualizarPrecosAsync(kit, dto.Precos);

        await kitRepository.SaveChangesAsync();

        return (ResultPartner<KitViewModel>)(KitViewModel)kit;
    }

    public async Task<ResultPartner<KitViewModel>> ObterAsync(Guid id)
    {
        var kit = await kitRepository.ObterAsync(id);
        return kit == null
            ? (ResultPartner<KitViewModel>)"Não foi possível localizar o kit"
            : (ResultPartner<KitViewModel>)(KitViewModel)kit;
    }

    public async Task<ResultPartner<PaginacaoViewModel<KitViewModel>>> PaginarAsync(
        FilterModel<Kit> filtro)
    {
        var pagina = await kitRepository.PaginacaoAsync(filtro);
        var resultado = new PaginacaoViewModel<KitViewModel>
        {
            TotalDeRegistros = pagina.TotalDeRegistros,
            TotalPaginas = pagina.TotalPaginas,
            Values = pagina.Values.Select(kit => (KitViewModel)kit).ToList()
        };

        return (ResultPartner<PaginacaoViewModel<KitViewModel>>)resultado;
    }

    public async Task<ResultPartner<ResultadoPadraoViewModel>> InativarAtivarAsync(
        Guid id,
        bool ativo)
    {
        var kit = await kitRepository.ObterAsync(id, rastrear: true);
        if (kit == null)
            return (ResultPartner<ResultadoPadraoViewModel>)"Não foi possível localizar o kit";

        kit.InativarAtivar(ativo);

        await kitRepository.SaveChangesAsync();
        return (ResultPartner<ResultadoPadraoViewModel>)new ResultadoPadraoViewModel
        {
            Resultado = true
        };
    }

    private async Task<string?> ValidarAsync(KitDto dto, Guid? ignorarKitId = null)
    {
        var erro = dto.Validar();
        if (erro != null)
            return erro;

        var kitComMesmoNome = await kitRepository.ObterPorNomeAsync(
            dto.Nome.Trim(), ignorarKitId);
        if (kitComMesmoNome != null)
        {
            return kitComMesmoNome.Ativo
                ? "Já existe um kit ativo com este nome"
                : "Já existe um kit inativo com este nome";
        }

        if (!await kitRepository.ProdutosExistemAsync(dto.Itens.Select(item => item.ProdutoId)))
            return "Produto não encontrado para este parceiro";

        if (!await kitRepository.TamanhosExistemAsync(dto.Itens
                .Where(item => item.TamanhoId.HasValue)
                .Select(item => item.TamanhoId!.Value)))
            return "Tamanho não encontrado para este parceiro";

        if (!await kitRepository.PesosExistemAsync(dto.Itens
                .Where(item => item.PesoId.HasValue)
                .Select(item => item.PesoId!.Value)))
            return "Peso não encontrado para este parceiro";

        if (!await kitRepository.TabelasDePrecoExistemAsync(
                dto.Precos.Select(preco => preco.TabelaPrecoId)))
            return "Tabela de preço não encontrada para este parceiro";

        return null;
    }

    private async Task<(string? Url, string? NomeBlob)> ObterImagemAsync(string? imagem)
    {
        if (string.IsNullOrWhiteSpace(imagem))
            return (null, null);

        var conteudo = imagem.Trim();
        if (conteudo.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return (conteudo, null);

        if (conteudo.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
        {
            var separador = conteudo.IndexOf(',');
            if (separador >= 0)
                conteudo = conteudo[(separador + 1)..];
        }

        var nomeBlob = $"{Guid.NewGuid()}.jpeg";
        var url = await uploadImageBlobClient.UploadImageAsync(conteudo, nomeBlob);
        return (url, nomeBlob);
    }

    private async Task AtualizarItensAsync(Kit kit, IList<KitItemDto> itensDto)
    {
        var itensDesejados = itensDto.ToDictionary(item =>
            (item.ProdutoId, item.TamanhoId, item.PesoId));

        foreach (var itemAtual in kit.Itens)
        {
            var chave = (itemAtual.ProdutoId, itemAtual.TamanhoId, itemAtual.PesoId);
            if (itensDesejados.Remove(chave, out var itemDto))
            {
                itemAtual.Atualizar(
                    true,
                    itemDto.ProdutoId,
                    itemDto.TamanhoId,
                    itemDto.PesoId,
                    itemDto.Quantidade);
                continue;
            }

            itemAtual.Atualizar(
                false,
                itemAtual.ProdutoId,
                itemAtual.TamanhoId,
                itemAtual.PesoId,
                itemAtual.Quantidade);
        }

        foreach (var itemDto in itensDesejados.Values)
        {
            var novoItem = KitItem.Novo(
                kit.Id,
                itemDto.ProdutoId,
                itemDto.TamanhoId,
                itemDto.PesoId,
                itemDto.Quantidade);

            await kitRepository.AddAsync(novoItem);
        }
    }

    private async Task AtualizarPrecosAsync(Kit kit, IList<KitPrecoDto> precosDto)
    {
        var precosDesejados = precosDto.ToDictionary(preco => preco.TabelaPrecoId);

        foreach (var precoAtual in kit.Precos)
        {
            if (precosDesejados.Remove(precoAtual.TabelaPrecoId, out var precoDto))
            {
                precoAtual.Atualizar(
                    true,
                    precoDto.TabelaPrecoId,
                    precoDto.PrecoPessoaFisica,
                    precoDto.PrecoPessoaJuridica);
                continue;
            }

            precoAtual.Atualizar(
                false,
                precoAtual.TabelaPrecoId,
                precoAtual.PrecoPessoaFisica,
                precoAtual.PrecoPessoaJuridica);
        }

        foreach (var precoDto in precosDesejados.Values)
        {
            var novoPreco = KitTabelaPreco.Novo(
                kit.Id,
                precoDto.TabelaPrecoId,
                precoDto.PrecoPessoaFisica,
                precoDto.PrecoPessoaJuridica);

            await kitRepository.AddAsync(novoPreco);
        }
    }
}
