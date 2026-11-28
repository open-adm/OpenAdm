using OpenAdm.Test.Domain.Builder;

namespace OpenAdm.Test.Domain.Test;

public class UsuarioTest
{
    [Fact]
    public void ValidarAcessoEcommerce_DeveRetornarNulo_QuandoUsuarioAtivoTemTelefone()
    {
        var usuario = UsuarioBuilder.Init().Build();

        Assert.Null(usuario.ValidarAcessoEcommerce());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidarAcessoEcommerce_DeveRetornarErro_QuandoUsuarioAtivoNaoTemTelefoneValido(string? telefone)
    {
        var usuario = UsuarioBuilder.Init().SemTelefone(telefone).Build();

        Assert.Equal("Seu cadastro esta incompleto, acesse sua conta e cadastre seu telefone!",
            usuario.ValidarAcessoEcommerce());
    }

    [Fact]
    public void ValidarAcessoEcommerce_DeveRetornarErro_QuandoUsuarioInativoTemTelefone()
    {
        var usuario = UsuarioBuilder.Init().Build();
        usuario.AtivarBloquear();

        Assert.Equal("Seu acesso esta bloqueado!", usuario.ValidarAcessoEcommerce());
    }
}
