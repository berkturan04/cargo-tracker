using CargoTracker.Domain.Entities;
using CargoTracker.Domain.Enums;
using CargoTracker.Domain.Exceptions;
using Xunit;

namespace CargoTracker.Domain.Tests;

public class ShipmentTests
{
    private static Shipment CreateValidShipment()
    {
        return new Shipment("TRK1234567890", "Ahmet Yılmaz", "İstanbul", "Ankara", 2.5m);
    }

    [Fact]
    public void Constructor_EmptyReceiverName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Shipment("TRK1", "", "İstanbul", "Ankara", 1m));
    }

    [Fact]
    public void Constructor_EmptyOriginCity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Shipment("TRK1", "Ahmet", "", "Ankara", 1m));
    }

    [Fact]
    public void Constructor_EmptyDestinationCity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Shipment("TRK1", "Ahmet", "İstanbul", "", 1m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_InvalidWeight_ThrowsArgumentException(decimal weight)
    {
        Assert.Throws<ArgumentException>(() => new Shipment("TRK1", "Ahmet", "İstanbul", "Ankara", weight));
    }

    [Fact]
    public void NewShipment_HasSingleInitialHistoryEntry()
    {
        var shipment = CreateValidShipment();

        Assert.Single(shipment.StatusHistory);
        Assert.Null(shipment.StatusHistory.First().FromStatus);
        Assert.Equal(ShipmentStatus.Created, shipment.StatusHistory.First().ToStatus);
    }

    [Fact]
    public void AdvanceTo_ValidTransition_UpdatesStatus()
    {
        var shipment = CreateValidShipment();

        shipment.AdvanceTo(ShipmentStatus.AtBranch);

        Assert.Equal(ShipmentStatus.AtBranch, shipment.Status);
    }

    [Fact]
    public void AdvanceTo_ValidTransition_AddsHistoryEntry()
    {
        var shipment = CreateValidShipment();

        shipment.AdvanceTo(ShipmentStatus.AtBranch);

        Assert.Equal(2, shipment.StatusHistory.Count);
        var lastEntry = shipment.StatusHistory.Last();
        Assert.Equal(ShipmentStatus.Created, lastEntry.FromStatus);
        Assert.Equal(ShipmentStatus.AtBranch, lastEntry.ToStatus);
    }

    [Fact]
    public void AdvanceTo_InvalidTransition_ThrowsException()
    {
        var shipment = CreateValidShipment();

        Assert.Throws<InvalidShipmentStatusTransitionException>(() => shipment.AdvanceTo(ShipmentStatus.Delivered));
    }

    [Fact]
    public void AdvanceTo_FullFlow_EndsAtDelivered()
    {
        var shipment = CreateValidShipment();

        shipment.AdvanceTo(ShipmentStatus.AtBranch);
        shipment.AdvanceTo(ShipmentStatus.InTransit);
        shipment.AdvanceTo(ShipmentStatus.OutForDelivery);
        shipment.AdvanceTo(ShipmentStatus.Delivered);

        Assert.Equal(ShipmentStatus.Delivered, shipment.Status);
        Assert.Equal(5, shipment.StatusHistory.Count);
    }

    [Fact]
    public void AdvanceTo_OutForDeliveryToReturned_Succeeds()
    {
        var shipment = CreateValidShipment();
        shipment.AdvanceTo(ShipmentStatus.AtBranch);
        shipment.AdvanceTo(ShipmentStatus.InTransit);
        shipment.AdvanceTo(ShipmentStatus.OutForDelivery);

        shipment.AdvanceTo(ShipmentStatus.Returned);

        Assert.Equal(ShipmentStatus.Returned, shipment.Status);
    }

    [Fact]
    public void AdvanceTo_FromReturned_ThrowsException()
    {
        var shipment = CreateValidShipment();
        shipment.AdvanceTo(ShipmentStatus.AtBranch);
        shipment.AdvanceTo(ShipmentStatus.InTransit);
        shipment.AdvanceTo(ShipmentStatus.OutForDelivery);
        shipment.AdvanceTo(ShipmentStatus.Returned);

        Assert.Throws<InvalidShipmentStatusTransitionException>(() => shipment.AdvanceTo(ShipmentStatus.AtBranch));
    }
}