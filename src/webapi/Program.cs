using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using webapi.Contract;
using Serilog;
using Microsoft.OpenApi.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TestAPI",
        Version = "v1"
    });
});

//Log
Log.Logger = new LoggerConfiguration()
                     .ReadFrom.Configuration(builder.Configuration)
                     .CreateBootstrapLogger();
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

//service
builder.Services.AddScoped<IProcessor, Processor>();

//application
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod()
.WithHeaders("authorization", "accept", "content-type", "origin"));
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

app.Run();
