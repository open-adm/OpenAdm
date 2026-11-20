using OpenAdm.Test.Domain.Builder;
using OpenAdm.Domain.Extensions;

namespace OpenAdm.Test.Domain.Test;

public class UsuarioTest
{
    [Theory]
    [InlineData("1133334444", true)]
    [InlineData("(11) 99999-8888", true)]
    [InlineData("999998888", false)]
    [InlineData("119999988888", false)]
    [InlineData("(11) 99999-888A", false)]
    [InlineData("+55 11 99999-8888", false)]
    public void TelefoneValido_DeveVerificarQuantidadeDeDigitosEAceitarMascara(string telefone, bool esperado)
    {
        Assert.Equal(esperado, telefone.TelefoneValido());
    }

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
    [InlineData("1234")]
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
