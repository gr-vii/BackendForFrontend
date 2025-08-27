using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapPost("/api/authenticate", async ([FromBody] AuthRequest request) =>
{
    await Task.Delay(Random.Shared.Next(20, 100));

    return Results.Ok(new AuthResponse
    {
        Success = request.User == "test@example.com" && request.Pwd == "password123",
        Token = request.User == "test@example.com" ? "mock-jwt-token-12345" : null,
        Expires = request.User == "test@example.com" ? DateTime.UtcNow.AddHours(1) : null
    });
});

app.MapPost("/api/pay", async ([FromBody] PaymentRequest request) =>
{
    await Task.Delay(Random.Shared.Next(30, 80)); // Simulate processing time

    return Results.Ok(new PaymentResponse
    {
        Success = request.Total > 0 && !string.IsNullOrEmpty(request.Dest),
        PaymentId = $"PAY_{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
        Reference = $"REF_{Random.Shared.Next(100000, 999999)}",
        Timestamp = DateTime.UtcNow
    });
});

app.Run("http://localhost:5001");

public record AuthRequest(string User, string Pwd);
public record AuthResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public DateTime? Expires { get; set; }
}

public record PaymentRequest(decimal Total, string Curr, string Dest);
public record PaymentResponse
{
    public bool Success { get; set; }
    public string? PaymentId { get; set; }
    public string? Reference { get; set; }
    public DateTime? Timestamp { get; set; }
}