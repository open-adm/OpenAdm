using Microsoft.AspNetCore.Mvc;
using OpenAdm.Api.Attributes;
using OpenAdm.Api.Extensions;
using OpenAdm.Application.Dtos.Kits;
using OpenAdm.Application.Dtos.Response;
using OpenAdm.Application.Interfaces;
using OpenAdm.Application.Models;
using OpenAdm.Application.Models.Kits;
using OpenAdm.Domain.Model;
using OpenAdm.Infra.Paginacao;

namespace OpenAdm.Api.Controllers;

[ApiController]
[Route("kits")]
[AcessoParceiro]
[Autentica]
[IsFuncionario]
public sealed class KitController(IKitService kitService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<KitViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarAsync(KitDto dto) =>
        (await kitService.CriarAsync(dto)).ToActionResult();

    [HttpPut("{id:guid}")]
    [ProducesResponseType<KitViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtualizarAsync(Guid id, KitDto dto) =>
        (await kitService.AtualizarAsync(id, dto)).ToActionResult();

    [HttpGet("{id:guid}")]
    [ProducesResponseType<KitViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterAsync(Guid id) =>
        (await kitService.ObterAsync(id)).ToActionResult();

    [HttpPost("paginacao")]
    [ProducesResponseType<PaginacaoViewModel<KitViewModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PaginarAsync(PaginacaoKitDto filtro) =>
        (await kitService.PaginarAsync(filtro)).ToActionResult();

    [HttpPut, Route("ativar/{id}/{ativo}")]
    [ProducesResponseType<ResultadoPadraoViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InativarAtivarAsync(Guid id, bool ativo) =>
        (await kitService.InativarAtivarAsync(id, ativo)).ToActionResult();
}
