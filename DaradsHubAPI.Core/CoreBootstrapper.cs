using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DaradsHubAPI.Core
{
    public class CoreBootstrapper
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

}