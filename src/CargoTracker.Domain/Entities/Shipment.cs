using CargoTracker.Domain.Enums;

namespace CargoTracker.Domain.Entities;

public class Shipment
{
    public Guid Id { get; private set; }
    public string TrackingNumber { get; private set; }
    public string ReceiverName { get; private set; }
    public string OriginCity { get; private set; }
    public string DestinationCity { get; private set; }
    public decimal WeightKg { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Shipment(string trackingNumber, string receiverName, string originCity, string destinationCity, decimal weightKg)
    {
        if(string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Takip numarası boş olamaz.", nameof(trackingNumber));
        if(string.IsNullOrWhiteSpace(receiverName))
            throw new ArgumentException("Alıcı adı boş olamaz.", nameof(receiverName));
        if(string.IsNullOrWhiteSpace(originCity))
            throw new ArgumentException("Çıkış şehri boş olamaz.", nameof(originCity));
        if(string.IsNullOrWhiteSpace(destinationCity))
            throw new ArgumentException("Varış şehri boş olamaz.", nameof(destinationCity));
        if(weightKg<=0)
            throw new ArgumentException("Ağırlık 0 veya negatif olamaz.", nameof(weightKg));

        TrackingNumber=trackingNumber;
        Id = Guid.NewGuid();
        ReceiverName = receiverName;
        OriginCity = originCity;
        DestinationCity = destinationCity;
        WeightKg = weightKg;
        Status = ShipmentStatus.Created;
        CreatedAt=DateTime.UtcNow;
    }
    

}
