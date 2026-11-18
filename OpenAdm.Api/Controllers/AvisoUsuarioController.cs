using Microsoft.AspNetCore.Mvc;
using OpenAdm.Api.Attributes;
using OpenAdm.Api.Extensions;
using OpenAdm.Application.Dtos.AvisosUsuarios;
using OpenAdm.Application.Dtos.Response;
using OpenAdm.Application.Interfaces;
using OpenAdm.Application.Models;
using OpenAdm.Application.Models.AvisosUsuarios;

namespace OpenAdm.Api.Controllers;

[ApiController]
[Route("avisos-usuarios")]
[AcessoParceiro]
[Autentica]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
public sealed class AvisoUsuarioController(IAvisoUsuarioService avisoUsuarioService) : ControllerBase
{
    [HttpPost]
    [IsFuncionario]
    [ProducesResponseType<AvisoUsuarioViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarAsync(CriarAvisoUsuarioDto dto) =>
        (await avisoUsuarioService.CriarAsync(dto)).ToActionResult();

    [HttpGet("{usuarioId:guid}")]
    [IsFuncionario]
    [ProducesResponseType<List<AvisoUsuarioViewModel>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorUsuarioAsync(Guid usuarioId) =>
        (await avisoUsuarioService.ListarPorUsuarioAsync(usuarioId)).ToActionResult();

    [HttpPut("{id:guid}/ativo/{ativo:bool}")]
    [IsFuncionario]
    [ProducesResponseType<ResultadoPadraoViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlterarAtivoAsync(Guid id, bool ativo) =>
        (await avisoUsuarioService.AlterarAtivoAsync(id, ativo)).ToActionResult();

    [HttpGet("meus/nao-lidos")]
    [ProducesResponseType<List<AvisoUsuarioViewModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarMeusNaoLidosAsync() =>
        (await avisoUsuarioService.ListarMeusNaoLidosAsync()).ToActionResult();

    [HttpPut("meus/{id:guid}/visto")]
    [ProducesResponseType<ResultadoPadraoViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarcarComoVistoAsync(Guid id) =>
        (await avisoUsuarioService.MarcarComoVistoAsync(id)).ToActionResult();
}
