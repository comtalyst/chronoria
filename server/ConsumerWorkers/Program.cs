using Chronoria_ConsumerWorkers.Producers;
using Chronoria_ConsumerWorkers.Repositories;
using Chronoria_ConsumerWorkers.Models;
using Chronoria_ConsumerWorkers.Services;
using Chronoria_ConsumerWorkers.Consumers;
using Microsoft.EntityFrameworkCore;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.AspNetCore;

// Initialize builder/primary config
var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
IConfiguration BasicConfig = configBuilder.Build();
var builder = WebHost.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostBuilderContext, configBuilder) =>
    {
        // Add secondary config
        if (hostBuilderContext.HostingEnvironment.IsProduction())
        {
            configBuilder.AddAzureKeyVault(
                new SecretClient(
                    new Uri($"https://{BasicConfig["Vault:KeyVaultName"]}.vault.azure.net/"),
                    new DefaultAzureCredential()
                    ),
                new KeyVaultSecretManager()
                );
        }
    });

builder.Configure((_) => { });

builder.ConfigureServices((hostBuilderContext, services) =>
{
    var Configuration = hostBuilderContext.Configuration;
    // Services
    services.AddScoped<IExpireClearService, ExpireClearService>();


    // Azure Service Bus Producers
    /*services.AddScoped<IExpireClearProducer, ExpireClearProducer>(
        sp => new ExpireClearProducer(Configuration["ServiceBus:Connections:Prime"]));
    services.AddScoped<ICapsuleReleaseProducer, CapsuleReleaseProducer>(
        sp => new CapsuleReleaseProducer(Configuration["ServiceBus:Connections:Prime"]));*/

    // Database Repositories
    services.AddScoped<ICapsuleRepository<PendingContext>, CapsuleRepository<PendingContext>>();
    services.AddScoped<ICapsuleRepository<ActiveContext>, CapsuleRepository<ActiveContext>>();

    // Databases Contexts
    services.AddDbContext<PendingContext>(o => o.UseNpgsql(Configuration["Db:Connections:Pending"]));
    services.AddDbContext<ActiveContext>(o => o.UseNpgsql(Configuration["Db:Connections:Active"]));

    // Azure Service Bus Consumers
    services.AddHostedService<ExpireClearConsumer>(
        sp => new ExpireClearConsumer(
            Configuration["ServiceBus:Connections:Prime"], sp
        )
    );
});


var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();

