using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FamilyTreeLibrary
{
    public static class FamilyTreeUtils
    {
        public static ILoggerFactory AddFamilyTreeLogger(this ILoggerFactory factory, FamilyTreeVault vault, ILoggerProvider? fallbackProvider = null)
        {
            factory.AddProvider(new FamilyTreeLoggerProvider(vault, fallbackProvider));
            return factory;
        }

        public static ILoggingBuilder AddFamilyTreeLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, FamilyTreeLoggerProvider>(sp =>
            {
                return new(sp.GetRequiredService<FamilyTreeVault>());
            });
            return builder;
        }
    }
}