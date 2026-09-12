using CoffeeShopApi.Api.Extensions;
using CoffeeShopApi.Api.Middleware;   // new — for GlobalExceptionHandler
using Scalar.AspNetCore;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddApplication().AddInfrastructure(builder.Configuration).AddApiServices(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}
app.UseCors(DependencyInjection.CorsPolicy);
app.MapControllers();
app.Run();
