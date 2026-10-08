using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CargoTracker.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithCustomerRole()
    {
        var request = new { email = $"{Guid.NewGuid()}@test.com", password = "Test1234!" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
        Assert.Equal("Customer", body!.Role);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        var email = $"{Guid.NewGuid()}@test.com";
        var request = new { email, password = "Test1234!" };

        await _client.PostAsJsonAsync("/api/auth/register", request);
        var secondResponse = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var email = $"{Guid.NewGuid()}@test.com";
        var password = "Test1234!";
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var email = $"{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Test1234!" });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "yanlis" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private record RegisterResponseDto(Guid Id, string Email, string Role, DateTime CreatedAt);
    private record LoginResponseDto(string Token, DateTime ExpiresAt, string Email, string Role);
}
