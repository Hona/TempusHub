using System;
using System.Linq;
using System.Reflection;
using Empedo.Discord.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Empedo.Discord.Helpers
{
    public static class BotServiceExtensions
    {
        public static IServiceCollection InjectBotServices(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.ExportedTypes.Where(type =>
            {
                var typeInfo = type.GetTypeInfo();

                // Does it have the `MicroserviceAttribute` 
                return typeInfo.GetCustomAttributes().Any(x => x.GetType() == typeof(BotServiceAttribute));
            }).ToList();

            foreach (var type in types)
            {
                var microserviceAttribute = type.GetCustomAttribute<BotServiceAttribute>();

                if (microserviceAttribute != null && (microserviceAttribute.Type == BotServiceType.Inject ||
                                                      microserviceAttribute.Type ==
                                                      BotServiceType.InjectAndInitialize))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
        
        public static void InitializeMicroservices(this IServiceProvider services, Assembly assembly)
        {
            var types = assembly.ExportedTypes.Where(type =>
            {
                var typeInfo = type.GetTypeInfo();

                // Does it have the `MicroserviceAttribute` 
                return typeInfo.GetCustomAttributes().Any(x => x.GetType() == typeof(BotServiceAttribute));
            });

            foreach (var type in types)
            {
                var microserviceAttribute = type.GetCustomAttribute<BotServiceAttribute>();

                if (microserviceAttribute != null && microserviceAttribute.Type == BotServiceType.InjectAndInitialize)
                {
                    // Initialize the service
                    services.GetRequiredService(type);
                }
            }
        }
    }
}