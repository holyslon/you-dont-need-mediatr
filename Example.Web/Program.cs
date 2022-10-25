using System.Collections.Immutable;
using Example.App;
using Example.App.Logging;
using Example.App.MediatR.Behaviors.Logging;
using Example.App.Metrics;
using Example.App.Native;
using Example.App.Services.Calculation;
using Example.Web.Controllers;
using Example.Web.Middleware;
using MediatR;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddSingleton<FactorialService>();
builder.Services.AddSingleton<FibonacciService>();
builder.Services.AddSingleton<CalculationUnit>();
builder.Services.AddMediatR(typeof(App));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(opts =>
    opts.LoggingFields = opts.LoggingFields |
                         HttpLoggingFields.RequestBody | 
                         HttpLoggingFields.ResponseBody);

builder.Services.AddElapsedMetric();
builder.Services.AddLoggingScopes();
builder.Services.Configure<ScopeGeneratorOptions<NativeController.CalculateInput>>(opts =>
    opts.WithGenerator(input => ImmutableDictionary.CreateRange(new[]
        { new KeyValuePair<string, string>("target", input.target.ToString() ?? string.Empty) })));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseHttpLogging();
app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.Run();