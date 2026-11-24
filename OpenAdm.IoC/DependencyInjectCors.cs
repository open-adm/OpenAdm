using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenAdm.IoC;

public static class DependencyInjectCors
{
    public static void InjectCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "base",
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .WithExposedHeaders("Content-Disposition", "Novotoken")
                        .AllowAnyHeader();

                    var maxAgeHoras = configuration
                        .GetValue<long?>("ConfiguracaoCors:MaxAgeHoras");

                    if (maxAgeHoras is > 0)
                    {
                        policy.SetPreflightMaxAge(
                            TimeSpan.FromHours(maxAgeHoras.Value));
                    }
                });
        });
    }
}