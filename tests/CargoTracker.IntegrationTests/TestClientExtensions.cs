using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CargoTracker.IntegrationTests;

public static class TestClientExtensions
{
    public static async Task<string> RegisterAndLoginAsCustomerAsync(this HttpClient client, string? email = null)
    {
        email ??= $"{Guid.NewGuid()}@test.com";
        const string password = "Test1234!";

        await client.PostAsJsonAsync("/api/auth/register", new { email, password });
        return await client.LoginAsync(email, password);
    }

    public static Task<string> LoginAsAdminAsync(this HttpClient client)
        => client.LoginAsync("admin@test.local", "TestAdmin123!");

    public static async Task<string> CreateAndLoginAsCourierAsync(this HttpClient client, string adminToken)
    {
        var email = $"{Guid.NewGuid()}@test.com";
        const string password = "Test1234!";

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/users")
        {
            Content = JsonContent.Create(new { email, password, role = "Courier" })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        await client.SendAsync(request);

        return await client.LoginAsync(email, password);
    }

    private static async Task<string> LoginAsync(this HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return body!.Token;
    }

    public static void SetBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private record LoginResponseDto(string Token, DateTime ExpiresAt, string Email, string Role);
}