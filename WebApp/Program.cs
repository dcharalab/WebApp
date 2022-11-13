using Domain.Abstractions;
using Infrastructure.Models;
using Infrastructure.Repositories;
using IpWebApi.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MediatR;
using System.Reflection;
using Application;
using Infrastructure.Repositories.Cached;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureOptions<DatabaseOptionsSetup>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IpdbContext>(
        (serviceProvider, dbContextOptionsBuilder) =>
        {
            var databaseOptions = serviceProvider.GetService < IOptions < DatabaseOptions > >()!.Value;

            dbContextOptionsBuilder.UseSqlServer(databaseOptions.ConnectionString, sqlServerAction =>
            {
                sqlServerAction.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
                sqlServerAction.CommandTimeout(databaseOptions.CommandTimeout);
            });
            dbContextOptionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
        }
    );

builder.Services.AddSingleton<IIpRepository, IpRepository>();
builder.Services.Decorate<IIpRepository, CachedIpRepository>();

builder.Services.AddApplicationModule();

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
