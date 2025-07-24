using apiBozzi.Configurations;
using DotNetEnv;
using apiBozzi.Context;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/clientes", () => "API is running.");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.ConfigureMiddlewares(app.Environment);

app.UseHttpsRedirection();

app.Run();