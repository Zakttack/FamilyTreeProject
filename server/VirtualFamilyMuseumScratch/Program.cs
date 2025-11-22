// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.Configuration.Models;

HostApplicationBuilder? builder = Host.CreateApplicationBuilder(args);
builder.AddFamilyConfiguration();
Console.WriteLine(builder.Configuration["FAMILY_CONFIGURATION_URI"]);
IHost? host = builder.Build();
FamilyVaultConfig vaultConfig = host.Services.GetRequiredService<IOptions<FamilyVaultConfig>>().Value;
Console.WriteLine(vaultConfig.Uri);
