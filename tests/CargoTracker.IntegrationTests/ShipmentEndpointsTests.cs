using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CargoTracker.IntegrationTests;

public class ShipmentEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ShipmentEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task CreateShipment_AsAuthenticatedCustomer_ReturnsCreated()
    {
        var token = await _client.RegisterAndLoginAsCustomerAsync();
        _client.SetBearerToken(token);

        var request = new { receiverName = "ali", originCity = "ankara", destinationCity = "bolu", weightKg = 5 };
        var response = await _client.PostAsJsonAsync("/api/shipments", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateShipment_WithoutAuthentication_ReturnsUnauthorized()
    {
        var request = new { receiverName = "ali", originCity = "ankara", destinationCity = "bolu", weightKg = 5 };

        var response = await _client.PostAsJsonAsync("/api/shipments", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetShipment_AsDifferentCustomer_ReturnsNotFound()
    {
        var ownerToken = await _client.RegisterAndLoginAsCustomerAsync();
        _client.SetBearerToken(ownerToken);
        var createResponse = await _client.PostAsJsonAsync("/api/shipments",
            new { receiverName = "ali", originCity = "ankara", destinationCity = "bolu", weightKg = 5 });
        var shipment = await createResponse.Content.ReadFromJsonAsync<ShipmentDto>();

        var otherToken = await _client.RegisterAndLoginAsCustomerAsync();
        _client.SetBearerToken(otherToken);
        var getResponse = await _client.GetAsync($"/api/shipments/{shipment!.TrackingNumber}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetAllShipments_AsCustomer_ReturnsForbidden()
    {
        var token = await _client.RegisterAndLoginAsCustomerAsync();
        _client.SetBearerToken(token);

        var response = await _client.GetAsync("/api/shipments");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_WithInvalidTransition_ReturnsBadRequest()
    {
        var adminToken = await _client.LoginAsAdminAsync();
        _client.SetBearerToken(adminToken);
        var createResponse = await _client.PostAsJsonAsync("/api/shipments",
            new { receiverName = "ali", originCity = "ankara", destinationCity = "bolu", weightKg = 5 });
        var shipment = await createResponse.Content.ReadFromJsonAsync<ShipmentDto>();

        var response = await _client.PatchAsJsonAsync($"/api/shipments/{shipment!.TrackingNumber}/status",
            new { newStatus = "Delivered" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_AsAssignedCourier_ReturnsOk()
    {
        var adminToken = await _client.LoginAsAdminAsync();
        _client.SetBearerToken(adminToken);
        var createResponse = await _client.PostAsJsonAsync("/api/shipments",
            new { receiverName = "ali", originCity = "ankara", destinationCity = "bolu", weightKg = 5 });
        var shipment = await createResponse.Content.ReadFromJsonAsync<ShipmentDto>();

        var courierToken = await _client.CreateAndLoginAsCourierAsync(adminToken);
        var courierMeResponse = await _client.GetAsync("/api/auth/me");
        // not: /me endpoint'i token gerektirir, aşağıdaki satırda admin token'ı hâlâ aktif;
        // kurye id'sini token'dan okumak yerine admin ile atama yapıp kuryeyle test ediyoruz
        await _client.PatchAsJsonAsync($"/api/shipments/{shipment!.TrackingNumber}/courier",
            new { courierId = await GetUserIdFromTokenAsync(courierToken) });

        _client.SetBearerToken(courierToken);
        var response = await _client.PatchAsJsonAsync($"/api/shipments/{shipment.TrackingNumber}/status",
            new { newStatus = "AtBranch" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<Guid> GetUserIdFromTokenAsync(string token)
    {
        using var tempClient = _factory.CreateClient();
        tempClient.SetBearerToken(token);
        var claims = await tempClient.GetFromJsonAsync<List<ClaimDto>>("/api/auth/me");
        var sub = claims!.First(c => c.Type == "sub").Value;
        return Guid.Parse(sub);
    }

    private record ShipmentDto(Guid Id, string TrackingNumber, string Status);
    private record ClaimDto(string Type, string Value);
}