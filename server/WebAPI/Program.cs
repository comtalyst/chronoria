using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Services;
using Chronoria_WebAPI.Repositories;
using Chronoria_WebAPI.Producers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddUserSecrets<Program>();
IConfiguration Configuration = configBuilder.Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Services
builder.Services.AddSingleton<IIdService, IdService>();
builder.Services.AddSingleton<ISubmissionService, SubmissionService>();
builder.Services.AddSingleton<IBlocklistService, BlocklistService>();
builder.Services.AddSingleton<IRequestValidationService>(new RequestValidationService(Configuration.GetSection("Constraints")));

// Azure Service Bus Producers
builder.Services.AddSingleton<IConfEmailProducer>(new ConfEmailProducer(Configuration["ServiceBus:Connections:Prime"]));

// Database Repositories
builder.Services.AddScoped<IBlocklistRepository, BlocklistRepository>();

builder.Services.AddScoped<ICapsuleRepository<PendingContext>, CapsuleRepository<PendingContext>>();
builder.Services.AddScoped<ICapsuleRepository<ActiveContext>, CapsuleRepository<ActiveContext>>();
builder.Services.AddScoped<ICapsuleRepository<ArchivedContext>, CapsuleRepository<ArchivedContext>>();

builder.Services.AddScoped<IFileContentRepository<PendingContext>, FileContentRepository<PendingContext>>();
builder.Services.AddScoped<IFileContentRepository<ActiveContext>, FileContentRepository<ActiveContext>>();
builder.Services.AddScoped<IFileContentRepository<ArchivedContext>, FileContentRepository<ArchivedContext>>();

builder.Services.AddScoped<ITextContentRepository<PendingContext>, TextContentRepository<PendingContext>>();
builder.Services.AddScoped<ITextContentRepository<ActiveContext>, TextContentRepository<ActiveContext>>();
builder.Services.AddScoped<ITextContentRepository<ArchivedContext>, TextContentRepository<ArchivedContext>>();

// Databases Contexts
builder.Services.AddDbContext<MetaContext>(o => o.UseNpgsql(Configuration["Db:Connections:Meta"]));
builder.Services.AddDbContext<PendingContext>(o => o.UseNpgsql(Configuration["Db:Connections:Pending"]));
builder.Services.AddDbContext<ActiveContext>(o => o.UseNpgsql(Configuration["Db:Connections:Active"]));
builder.Services.AddDbContext<ArchivedContext>(o => o.UseNpgsql(Configuration["Db:Connections:Archived"]));

// Azure Blob Storage
builder.Services.AddSingleton<PendingBlobServiceClient>(new PendingBlobServiceClient(Configuration["Blob:Connections:Pending"]));
builder.Services.AddSingleton<ActiveBlobServiceClient>(new ActiveBlobServiceClient(Configuration["Blob:Connections:Active"]));

builder.Services.AddScoped<IFileBlobRepository<ActiveBlobServiceClient>, FileBlobRepository<ActiveBlobServiceClient>>(
    sp => new FileBlobRepository<ActiveBlobServiceClient>(
        sp.GetRequiredService<ActiveBlobServiceClient>(),
        Configuration["Blob:Containers:Active:File"]
    )
);
builder.Services.AddScoped<IFileBlobRepository<PendingBlobServiceClient>, FileBlobRepository<PendingBlobServiceClient>>(
    sp => new FileBlobRepository<PendingBlobServiceClient>(
        sp.GetRequiredService<PendingBlobServiceClient>(),
        Configuration["Blob:Containers:Pending:File"]
    )
);
builder.Services.AddScoped<ITextBlobRepository<ActiveBlobServiceClient>, TextBlobRepository<ActiveBlobServiceClient>>(
    sp => new TextBlobRepository<ActiveBlobServiceClient>(
        sp.GetRequiredService<ActiveBlobServiceClient>(),
        Configuration["Blob:Containers:Active:Text"]
    )
);
builder.Services.AddScoped<ITextBlobRepository<PendingBlobServiceClient>, TextBlobRepository<PendingBlobServiceClient>>(
    sp => new TextBlobRepository<PendingBlobServiceClient>(
        sp.GetRequiredService<PendingBlobServiceClient>(),
        Configuration["Blob:Containers:Pending:Text"]
    )
);




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MainPolicy", builder =>
    {
        builder.WithOrigins("*").WithMethods("POST", "GET", "PUT", "DELETE");
    });
});
app.UseCors("MainPolicy");

app.MapControllers();

app.Run();
