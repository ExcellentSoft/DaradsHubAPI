using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DaradsHubAPI.Shared;

public class SharedBootstrapper
{
    public static void InitServices(IServiceCollection services)
    {
        AutoInjectLayers(services);
    }

    private static void AutoInjectLayers(IServiceCollection serviceCollection)
    {
        var targetAssembly = Assembly.GetExecutingAssembly();
        serviceCollection.Scan(scan => scan.FromAssemblies(targetAssembly).AddClasses(classes => classes
                .Where(type => type.Name.EndsWith("UnitOfWork") || type.Name.EndsWith("Service")), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }
}
