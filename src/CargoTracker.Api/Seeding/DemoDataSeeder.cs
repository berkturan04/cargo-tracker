using CargoTracker.Application.Abstractions;
using CargoTracker.Domain.Entities;
using CargoTracker.Domain.Enums;

namespace CargoTracker.Api.Seeding;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("SeedDemoData"))
            return;

        using var scope = services.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var shipmentRepository = scope.ServiceProvider.GetRequiredService<IShipmentRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        const string demoCustomerEmail = "customer@demo.local";
        if (await userRepository.GetByEmailAsync(demoCustomerEmail) is not null)
            return; // zaten tohumlanmış, tekrar etme

        var customer = new User(demoCustomerEmail, hasher.Hash("Demo1234!"), UserRole.Customer);
        var customer2 = new User("customer2@demo.local", hasher.Hash("Demo1234!"), UserRole.Customer);
        var courier = new User("courier@demo.local", hasher.Hash("Demo1234!"), UserRole.Courier);

        await userRepository.AddAsync(customer);
        await userRepository.AddAsync(customer2);
        await userRepository.AddAsync(courier);

        var s1 = CreateDemoShipment("Ahmet Yılmaz", "İstanbul", "Ankara", 2.5m, customer.Id);
        var s2 = CreateDemoShipment("Zeynep Kaya", "İzmir", "Bursa", 1.2m, customer.Id);
        var s3 = CreateDemoShipment("Mehmet Demir", "Ankara", "Antalya", 5.0m, customer2.Id);
        var s4 = CreateDemoShipment("Ayşe Çelik", "Bursa", "İstanbul", 0.8m, customer2.Id);
        var s5 = CreateDemoShipment("Can Öztürk", "Antalya", "İzmir", 3.3m, customer.Id);

        foreach (var shipment in new[] { s1, s2, s3, s4, s5 })
            await shipmentRepository.AddAsync(shipment);

        // Farklı durumlarda örnekler olsun diye birkaçını ilerletiyoruz; s5 kasıtlı olarak
        // "Created" durumunda bırakılıyor.
        s1.AssignCourier(courier.Id);
        s1.AdvanceTo(ShipmentStatus.AtBranch);

        s2.AssignCourier(courier.Id);
        s2.AdvanceTo(ShipmentStatus.AtBranch);
        s2.AdvanceTo(ShipmentStatus.InTransit);

        s3.AssignCourier(courier.Id);
        s3.AdvanceTo(ShipmentStatus.AtBranch);
        s3.AdvanceTo(ShipmentStatus.InTransit);
        s3.AdvanceTo(ShipmentStatus.OutForDelivery);
        s3.AdvanceTo(ShipmentStatus.Delivered);

        s4.AssignCourier(courier.Id);
        s4.AdvanceTo(ShipmentStatus.AtBranch);
        s4.AdvanceTo(ShipmentStatus.InTransit);
        s4.AdvanceTo(ShipmentStatus.OutForDelivery);
        s4.AdvanceTo(ShipmentStatus.Returned);

        await shipmentRepository.SaveChangesAsync();
    }

    private static Shipment CreateDemoShipment(string receiverName, string originCity, string destinationCity, decimal weightKg, Guid customerId)
    {
        var trackingNumber = "TRK" + Random.Shared.NextInt64(1_000_000_000, 10_000_000_000);
        return new Shipment(trackingNumber, receiverName, originCity, destinationCity, weightKg, customerId);
    }
}