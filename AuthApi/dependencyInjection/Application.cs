using Authentication.Applicacion;

namespace AuthApi.dependencyInjection;
public static class Application
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<SignUp>();

            return services;
        }
}