namespace OpenAdm.Application.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ValidaGuidAttribute(string erro = "Campo obrigatório.") : ValidaBaseAttribute
{
    public override string? Validar(object? valor) =>
        valor is Guid id && id != Guid.Empty ? null : erro;
}
