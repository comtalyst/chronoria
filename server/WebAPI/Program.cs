using Chronoria_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Databases
builder.Services.AddDbContext<PendingContext>(o => o.UseNpgsql("TODO"));
builder.Services.AddDbContext<ActiveContext>(o => o.UseNpgsql("TODO"));
builder.Services.AddDbContext<ArchivedContext>(o => o.UseNpgsql("TODO"));

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

app.MapControllers();

app.Run();
