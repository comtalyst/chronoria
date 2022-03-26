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
    services.AddScoped<ICoreEmailService, CoreEmailService>(
        _ => new CoreEmailService(
            Configuration["EmailService:ApiKey"],
            Configuration["EmailService:Senders:Main:Email"],
            Configuration["EmailService:Senders:Main:Name"]
        )
    );
    services.AddScoped<IExpireClearService, ExpireClearService>();
    services.AddScoped<ICapsuleReleaseService, CapsuleReleaseService>();
    services.AddScoped<IEmailTemplateService, EmailTemplateService>();
    services.AddScoped<IFrontLinkService, FrontLinkService>(
        _ => new FrontLinkService(
            Configuration["FrontLinks:Domain"],
            Configuration["FrontLinks:Services:Cancel"],
            Configuration["FrontLinks:Services:Confirm"],
            Configuration["FrontLinks:Services:Download"]
        )    
    );
    services.AddScoped<IActiveReceiptEmailService, ActiveReceiptEmailService>();
    services.AddScoped<ICanceledReceiptEmailService, CanceledReceiptEmailService>();
    services.AddScoped<ICapsuleDeliveryService, CapsuleDeliveryService>();
    services.AddScoped<IConfEmailService, ConfEmailService>();

    // Azure Service Bus Producers
    services.AddScoped<ICapsuleDeliveryProducer, CapsuleDeliveryProducer>(
        sp => new CapsuleDeliveryProducer(Configuration["ServiceBus:Connections:Prime"]));

    // Database Repositories
    services.AddScoped<ICapsuleRepository<PendingContext>, CapsuleRepository<PendingContext>>();
    services.AddScoped<ICapsuleRepository<ActiveContext>, CapsuleRepository<ActiveContext>>();
    services.AddScoped<ICapsuleRepository<ArchivedContext>, CapsuleRepository<ArchivedContext>>();
    services.AddScoped<ITextContentRepository<PendingContext>, TextContentRepository<PendingContext>>();
    services.AddScoped<ITextContentRepository<ActiveContext>, TextContentRepository<ActiveContext>>();
    services.AddScoped<ITextContentRepository<ArchivedContext>, TextContentRepository<ArchivedContext>>();
    services.AddScoped<IFileContentRepository<PendingContext>, FileContentRepository<PendingContext>>();
    services.AddScoped<IFileContentRepository<ActiveContext>, FileContentRepository<ActiveContext>>();
    services.AddScoped<IFileContentRepository<ArchivedContext>, FileContentRepository<ArchivedContext>>();

    // Databases Contexts
    services.AddDbContext<PendingContext>(o => o.UseNpgsql(Configuration["Db:Connections:Pending"]));
    services.AddDbContext<ActiveContext>(o => o.UseNpgsql(Configuration["Db:Connections:Active"]));
    services.AddDbContext<ArchivedContext>(o => o.UseNpgsql(Configuration["Db:Connections:Archived"]));

    // Azure Blob Storage
    services.AddSingleton<PendingBlobServiceClient>(new PendingBlobServiceClient(Configuration["Blob:Connections:Pending"]));
    services.AddSingleton<ActiveBlobServiceClient>(new ActiveBlobServiceClient(Configuration["Blob:Connections:Active"]));
    services.AddSingleton<ArchivedBlobServiceClient>(new ArchivedBlobServiceClient(Configuration["Blob:Connections:Archived"]));
    services.AddSingleton<StaticBlobServiceClient>(new StaticBlobServiceClient(Configuration["Blob:Connections:Static"]));

    services.AddScoped<IFileBlobRepository<ActiveBlobServiceClient>, FileBlobRepository<ActiveBlobServiceClient>>(
        sp => new FileBlobRepository<ActiveBlobServiceClient>(
            sp.GetRequiredService<ActiveBlobServiceClient>(),
            Configuration["Blob:Containers:Active:File"]
        )
    );
    services.AddScoped<IFileBlobRepository<PendingBlobServiceClient>, FileBlobRepository<PendingBlobServiceClient>>(
        sp => new FileBlobRepository<PendingBlobServiceClient>(
            sp.GetRequiredService<PendingBlobServiceClient>(),
            Configuration["Blob:Containers:Pending:File"]
        )
    );
    services.AddScoped<IFileBlobRepository<ArchivedBlobServiceClient>, FileBlobRepository<ArchivedBlobServiceClient>>(
        sp => new FileBlobRepository<ArchivedBlobServiceClient>(
            sp.GetRequiredService<ArchivedBlobServiceClient>(),
            Configuration["Blob:Containers:Archived:File"]
        )
    );
    services.AddScoped<ITextBlobRepository<ActiveBlobServiceClient>, TextBlobRepository<ActiveBlobServiceClient>>(
        sp => new TextBlobRepository<ActiveBlobServiceClient>(
            sp.GetRequiredService<ActiveBlobServiceClient>(),
            Configuration["Blob:Containers:Active:Text"]
        )
    );
    services.AddScoped<ITextBlobRepository<PendingBlobServiceClient>, TextBlobRepository<PendingBlobServiceClient>>(
        sp => new TextBlobRepository<PendingBlobServiceClient>(
            sp.GetRequiredService<PendingBlobServiceClient>(),
            Configuration["Blob:Containers:Pending:Text"]
        )
    );
    services.AddScoped<ITextBlobRepository<ArchivedBlobServiceClient>, TextBlobRepository<ArchivedBlobServiceClient>>(
        sp => new TextBlobRepository<ArchivedBlobServiceClient>(
            sp.GetRequiredService<ArchivedBlobServiceClient>(),
            Configuration["Blob:Containers:Archived:Text"]
        )
    );

    services.AddScoped<IEmailTemplateBlobRepository, EmailTemplateBlobRepository>(
        sp => new EmailTemplateBlobRepository(
            sp.GetRequiredService<StaticBlobServiceClient>(),
            Configuration["Blob:Containers:Static:EmailTemplates"]
        )
    );

    // Azure Service Bus Consumers
    services.AddHostedService<ExpireClearConsumer>(
        sp => new ExpireClearConsumer(
            Configuration["ServiceBus:Connections:Prime"], sp
        )
    );
    services.AddHostedService<CapsuleReleaseConsumer>(
        sp => new CapsuleReleaseConsumer(
            Configuration["ServiceBus:Connections:Prime"], sp
        )
    );
    services.AddHostedService<ActiveReceiptEmailConsumer>(
        sp => new ActiveReceiptEmailConsumer(
            Configuration["ServiceBus:Connections:Prime"], sp
        )
    );
    services.AddHostedService<CanceledReceiptEmailConsumer>(
        sp => new CanceledReceiptEmailConsumer(
            Configuration["ServiceBus:Connections:Prime"], sp
        )
    );
    services.AddHostedService<CapsuleDeliveryConsumer>(
        sp => new CapsuleDeliveryConsumer(
            Configuration["ServiceBus:Connections:Prime"], sp
        )
    );
    services.AddHostedService<ConfEmailConsumer>(
        sp => new ConfEmailConsumer(
            Configuration["ServiceBus:Connections:Prime"], sp
        )
    );
});


var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();


