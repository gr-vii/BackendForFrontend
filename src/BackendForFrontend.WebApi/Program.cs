using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using BackendForFrontend.Application.Handlers.Auth;
using BackendForFrontend.Application.Validators.Auth;
using BackendForFrontend.Infrastructure.Clients;
using BackendForFrontend.Infrastructure.Interfaces;
using BackendForFrontend.Infrastructure.Resilience;
using BackendForFrontend.WebApi.HealthChecks;
using BackendForFrontend.WebApi.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// API Versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

// HTTP Clients with Polly
builder.Services.AddHttpClient<IPaymentProviderClient, PaymentProviderClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
    client.Timeout = TimeSpan.FromSeconds(5);
})
.AddPolicyHandler((services, _) => PolicyFactory.CreateRetryPolicy(
    services.GetRequiredService<ILogger<PaymentProviderClient>>()))
.AddPolicyHandler((services, _) => PolicyFactory.CreateCircuitBreakerPolicy(
    services.GetRequiredService<ILogger<PaymentProviderClient>>()))
.AddPolicyHandler(PolicyFactory.CreateTimeoutPolicy());

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<ProviderHealthCheck>("provider", tags: new[] { "ready" })
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"Payment Gateway API {description.GroupName.ToUpperInvariant()}");
        }
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();

app.UseRouting();

app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.Run();