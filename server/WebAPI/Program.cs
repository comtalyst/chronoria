using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Services;
using Chronoria_WebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
IConfiguration Configuration = configBuilder.Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Services
builder.Services.AddSingleton<IIdService, IdService>();
builder.Services.AddSingleton<ISubmissionService, SubmissionService>();

// Database Repositories
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
builder.Services.AddDbContext<PendingContext>(o => o.UseNpgsql(Configuration["Db:Connection:Pending"]));
builder.Services.AddDbContext<ActiveContext>(o => o.UseNpgsql(Configuration["Db:Connection:Active"]));
builder.Services.AddDbContext<ArchivedContext>(o => o.UseNpgsql(Configuration["Db:Connection:Archived"]));

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
