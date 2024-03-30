using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShoppingCart.Api.Extensions;
using ShoppingCart.Api.Seed;
using ShoppingCart.Api.CustomMiddlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceExtensions(builder.Configuration);
builder.Services.AddRepositoryExtensions();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

//Seed
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var seed = services.GetService<SeedData>();
    await seed.SeedDatabase();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

app.Run();
