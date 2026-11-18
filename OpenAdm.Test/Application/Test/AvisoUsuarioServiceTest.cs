using OpenAdm.Application.Dtos.AvisosUsuarios;
using OpenAdm.Application.Services;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Interfaces;

namespace OpenAdm.Test.Application.Test;

public class AvisoUsuarioServiceTest
{
    private readonly Mock<IAvisoUsuarioRepository> _repository;
    private readonly Mock<IUsuarioAutenticado> _usuarioAutenticado;
    private readonly AvisoUsuarioService _service;

    public AvisoUsuarioServiceTest()
    {
        _repository = new Mock<IAvisoUsuarioRepository>();
        _usuarioAutenticado = new Mock<IUsuarioAutenticado>();
        _usuarioAutenticado.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Usuario);
        _service = new AvisoUsuarioService(_repository.Object, _usuarioAutenticado.Object);
    }

    [Fact]
    public async Task DeveCriarAvisoVinculadoAoDestinatarioAindaNaoVisto()
    {
        var destinatarioId = Guid.NewGuid();
        _repository.Setup(x => x.UsuarioExisteAsync(destinatarioId)).ReturnsAsync(true);

        var resultado = await _service.CriarAsync(new CriarAvisoUsuarioDto
        {
            UsuarioId = destinatarioId,
            Mensagem = "Não conseguimos contato pelo WhatsApp."
        });

        Assert.Null(resultado.Error);
        Assert.NotNull(resultado.Result);
        Assert.False(resultado.Result.Visto);
        Assert.True(resultado.Result.Ativo);
        Assert.Equal("Não conseguimos contato pelo WhatsApp.", resultado.Result.Mensagem);
        _repository.Verify(x => x.AddAsync(It.Is<AvisoUsuario>(aviso =>
            aviso.UsuarioId == destinatarioId && aviso.DataDeVisualizacao == null)), Times.Once);
    }

    [Fact]
    public async Task DeveListarAvisosNaoLidosUsandoAIdentidadeDoClienteAutenticado()
    {
        var clienteId = Guid.NewGuid();
        _usuarioAutenticado.SetupGet(x => x.Id).Returns(clienteId);
        _repository.Setup(x => x.ListarNaoLidosAsync(clienteId)).ReturnsAsync([]);

        var resultado = await _service.ListarMeusNaoLidosAsync();

        Assert.NotNull(resultado.Result);
        Assert.Empty(resultado.Result);
        _repository.Verify(x => x.ListarNaoLidosAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task DeveRecusarConfirmacaoQuandoAvisoNaoPertenceAoCliente()
    {
        var clienteId = Guid.NewGuid();
        var avisoId = Guid.NewGuid();
        _usuarioAutenticado.SetupGet(x => x.Id).Returns(clienteId);
        _repository.Setup(x => x.MarcarComoVistoAsync(clienteId, avisoId)).ReturnsAsync(false);

        var resultado = await _service.MarcarComoVistoAsync(avisoId);

        Assert.Equal("Aviso não encontrado.", resultado.Error);
        _repository.Verify(x => x.MarcarComoVistoAsync(clienteId, avisoId), Times.Once);
    }
}
