using Chronoria_PersistentWorkers.Producers;
using Chronoria_PersistentWorkers.Repositories;
using Chronoria_PersistentWorkers.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

// Initialize builder/primary config
var builder = WebApplication.CreateBuilder(args);
var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
if (builder.Environment.IsDevelopment())
{
    configBuilder = configBuilder.AddUserSecrets<Program>();
}
IConfiguration Configuration = configBuilder.Build();

// Add secondary config
if (builder.Environment.IsProduction())
{
    configBuilder = configBuilder.AddAzureKeyVault(
        new SecretClient(
            new Uri($"https://{Configuration["Vault:KeyVaultName"]}.vault.azure.net/"),
            new DefaultAzureCredential()
            ),
        new KeyVaultSecretManager()
        );
}
Configuration = configBuilder.Build();



// Azure Service Bus Producers
builder.Services.AddScoped<IExpireClearProducer, ExpireClearProducer>(
    sp => new ExpireClearProducer(Configuration["ServiceBus:Connections:Prime"]));

// Database Repositories
builder.Services.AddScoped<ICapsuleRepository<PendingContext>, CapsuleRepository<PendingContext>>();
builder.Services.AddScoped<ICapsuleRepository<ActiveContext>, CapsuleRepository<ActiveContext>>();

// Databases Contexts
builder.Services.AddDbContext<PendingContext>(o => o.UseNpgsql(Configuration["Db:Connections:Pending"]));
builder.Services.AddDbContext<ActiveContext>(o => o.UseNpgsql(Configuration["Db:Connections:Active"]));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
