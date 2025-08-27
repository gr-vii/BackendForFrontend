using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BackendForFrontend.Application.Contracts.Auth;
using BackendForFrontend.Infrastructure.Models;
using System.Text.Json;

namespace BackendForFrontend.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
public class MappingBenchmarks
{
    private readonly ProviderAuthResponse _providerResponse;
    private readonly LoginRequest _loginRequest;
    private readonly JsonSerializerOptions _jsonOptions;

    public MappingBenchmarks()
    {
        _providerResponse = new ProviderAuthResponse(
            Success: true,
            Token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test",
            Expires: DateTime.UtcNow.AddHours(1)
        );

        _loginRequest = new LoginRequest("test@example.com", "password123");

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Benchmark]
    public LoginResponse MapProviderResponseToBffResponse()
    {
        return new LoginResponse(
            IsSuccess: _providerResponse.Success,
            Jwt: _providerResponse.Token,
            ExpiresAt: _providerResponse.Expires
        );
    }

    [Benchmark]
    public ProviderAuthRequest MapBffRequestToProviderRequest()
    {
        return new ProviderAuthRequest(_loginRequest.Username, _loginRequest.Password);
    }

    [Benchmark]
    public string SerializeLoginRequest()
    {
        return JsonSerializer.Serialize(_loginRequest, _jsonOptions);
    }

    [Benchmark]
    public LoginRequest DeserializeLoginRequest()
    {
        var json = """{"username":"test@example.com","password":"password123"}""";
        return JsonSerializer.Deserialize<LoginRequest>(json, _jsonOptions)!;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<MappingBenchmarks>();
    }
}

