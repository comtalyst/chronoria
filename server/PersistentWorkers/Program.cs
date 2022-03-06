using Chronoria_PersistentWorkers.Producers;
using Chronoria_PersistentWorkers.Repositories;
using Chronoria_PersistentWorkers.Models;
using Chronoria_PersistentWorkers.Schedulers;
using Microsoft.EntityFrameworkCore;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

// Initialize builder/primary config
var builder = Host.CreateDefaultBuilder(args);
var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
builder.ConfigureAppConfiguration((hostBuilderContext, build) =>
{
    if (hostBuilderContext.HostingEnvironment.IsDevelopment())
    {
        configBuilder = configBuilder.AddUserSecrets<Program>();            // TODO: use Generic Host built-in config instead
    }
});
IConfiguration Configuration = configBuilder.Build();
builder.ConfigureAppConfiguration((hostBuilderContext, build) =>
{
    // Add secondary config
    if (hostBuilderContext.HostingEnvironment.IsProduction())
    {
        configBuilder = configBuilder.AddAzureKeyVault(
            new SecretClient(
                new Uri($"https://{Configuration["Vault:KeyVaultName"]}.vault.azure.net/"),
                new DefaultAzureCredential()
                ),
            new KeyVaultSecretManager()
            );
    }
});
Configuration = configBuilder.Build();

builder.ConfigureServices(services =>
{
    // Azure Service Bus Producers
    services.AddScoped<IExpireClearProducer, ExpireClearProducer>(
        sp => new ExpireClearProducer(Configuration["ServiceBus:Connections:Prime"]));
    services.AddScoped<ICapsuleReleaseProducer, CapsuleReleaseProducer>(
        sp => new CapsuleReleaseProducer(Configuration["ServiceBus:Connections:Prime"]));

    // Database Repositories
    services.AddScoped<ICapsuleRepository<PendingContext>, CapsuleRepository<PendingContext>>();
    services.AddScoped<ICapsuleRepository<ActiveContext>, CapsuleRepository<ActiveContext>>();

    // Databases Contexts
    services.AddDbContext<PendingContext>(o => o.UseNpgsql(Configuration["Db:Connections:Pending"]));
    services.AddDbContext<ActiveContext>(o => o.UseNpgsql(Configuration["Db:Connections:Active"]));

    // Schedulers
    services.AddHostedService<ExpireClearScheduler>(
        sp => new ExpireClearScheduler(
            long.Parse(Configuration["Policies:PendingExpireTime"]), sp
        )
    );
    services.AddHostedService<CapsuleReleaseScheduler>(
        sp => new CapsuleReleaseScheduler(
            long.Parse(Configuration["Policies:MinCapsuleAge"]) - long.Parse(Configuration["Policies:PendingExpireTime"]), sp
        )
    );
});


var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();


