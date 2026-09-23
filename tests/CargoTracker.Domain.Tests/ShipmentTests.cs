using CargoTracker.Domain.Entities;
using CargoTracker.Domain.Enums;
using CargoTracker.Domain.Exceptions;

namespace CargoTracker.Domain.Tests;

public class ShipmentTests
{
    private static Shipment CreateValidShipment()
    {
        return new Shipment("TRK1234567890", "Ahmet Yılmaz", "İstanbul", "Ankara", 2.5m);
    }

    [Fact]
    public void AdvanceTo_FullLifecycle_FlowsSuccessfully()
    {
        var shipment = CreateValidShipment();
        Assert.Equal(ShipmentStatus.Created, shipment.Status);

        shipment.AdvanceTo(ShipmentStatus.AtBranch);
        Assert.Equal(ShipmentStatus.AtBranch, shipment.Status);

        shipment.AdvanceTo(ShipmentStatus.InTransit);
        Assert.Equal(ShipmentStatus.InTransit, shipment.Status);

        shipment.AdvanceTo(ShipmentStatus.OutForDelivery);
        Assert.Equal(ShipmentStatus.OutForDelivery, shipment.Status);

        shipment.AdvanceTo(ShipmentStatus.Delivered);
        Assert.Equal(ShipmentStatus.Delivered, shipment.Status);
    }

    [Fact]
    public void AdvanceTo_OutForDeliveryToReturned_UpdatesStatus()
    {
        var shipment = CreateValidShipment();
        shipment.AdvanceTo(ShipmentStatus.AtBranch);
        shipment.AdvanceTo(ShipmentStatus.InTransit);
        shipment.AdvanceTo(ShipmentStatus.OutForDelivery);

        shipment.AdvanceTo(ShipmentStatus.Returned);

        Assert.Equal(ShipmentStatus.Returned, shipment.Status);
    }

    [Fact]
    public void AdvanceTo_WhenReturned_ThrowsException()
    {
        var shipment = CreateValidShipment();
        shipment.AdvanceTo(ShipmentStatus.AtBranch);
        shipment.AdvanceTo(ShipmentStatus.InTransit);
        shipment.AdvanceTo(ShipmentStatus.OutForDelivery);
        shipment.AdvanceTo(ShipmentStatus.Returned);

        Assert.Throws<InvalidShipmentStatusTransitionException>(() => shipment.AdvanceTo(ShipmentStatus.Delivered));
    }

    [Fact]
    public void Constructor_EmptyReceiverName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Shipment("TRK1234567890", "", "İstanbul", "Ankara", 2.5m));
    }

    [Fact]
    public void AdvanceTo_ValidTransition_UpdatesStatus()
    {
        var shipment = CreateValidShipment();

        shipment.AdvanceTo(ShipmentStatus.AtBranch);

        Assert.Equal(ShipmentStatus.AtBranch, shipment.Status);
    }

    [Fact]
    public void AdvanceTo_InvalidTransition_ThrowsException()
    {
        var shipment = CreateValidShipment();

        Assert.Throws<InvalidShipmentStatusTransitionException>(() => shipment.AdvanceTo(ShipmentStatus.Delivered));
    }
}
