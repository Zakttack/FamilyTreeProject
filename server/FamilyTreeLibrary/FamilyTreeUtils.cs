using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FamilyTreeLibrary
{
    public static class FamilyTreeUtils
    {
        public static ILoggingBuilder AddFamilyTreeLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, FamilyTreeLoggerProvider>(sp =>
            {
                return new(sp.GetRequiredService<FamilyTreeVault>());
            });
            builder.Services.AddTransient(typeof(IExtendedLogger<>), typeof(FamilyTreeLogger<>));
            return builder;
        }
    }
}