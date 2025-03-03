using System.Reflection;
using pizzashop.repository.Models;
using Microsoft.EntityFrameworkCore;

namespace pizzashop.web
{
    public static class DependencyInjection
    {
        public static void RegisterServices(IServiceCollection services, string connectionString )
        {
            services.AddDbContext<PizzaShopContext>(options =>
                options.UseNpgsql(connectionString)
            );
            var allReferencedTypes = Assembly
                .GetAssembly(typeof(DependencyInjection))!
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    t.Namespace != null
                    && (
                        t.Namespace.StartsWith("pizzashop.repository")
                        || t.Namespace.StartsWith("pizzashop.service")
                    )
                )
                .ToList();
            var interfaces = allReferencedTypes.Where(t => t.IsInterface);

            foreach (var serviceInterface in interfaces)
            {
                var implementation = allReferencedTypes.FirstOrDefault(c =>
                    c.IsClass && !c.IsAbstract && serviceInterface.Name.Substring(1) == c.Name
                );

                if (implementation != null)
                {
                    services.AddScoped(serviceInterface, implementation);
                }
            }
        }
    }
}
