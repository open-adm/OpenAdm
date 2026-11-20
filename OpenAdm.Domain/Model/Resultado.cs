using OpenAdm.Domain.Enuns;

namespace OpenAdm.Domain.Model;

public class ErroResultado
{
    public string Mensagem { get; set; } = string.Empty;
    public TipoErroEnum Tipo { get; set; }
}

public union Resultado<T>(T, ErroResultado) where T : class;

public static class ResultadoExtension
{
    public static bool EhErro<T>(this Resultado<T> resultado) where T : class
    {
        return resultado switch
        {
            T => false,
            ErroResultado => true
        };
    }
}