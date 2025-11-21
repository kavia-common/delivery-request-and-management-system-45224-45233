namespace DeliveryBackend.Models
{
    /// <summary>
    /// User domain entity.
    /// </summary>
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // For placeholder auth only; do NOT store plaintext in production.
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Order domain entity.
    /// </summary>
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public PackageDetails Package { get; set; } = new PackageDetails();
        public OrderStatus Status { get; set; } = OrderStatus.Created;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Package details value object.
    /// </summary>
    public class PackageDetails
    {
        public string Description { get; set; } = string.Empty;
        public double WeightKg { get; set; }
        public double LengthCm { get; set; }
        public double WidthCm { get; set; }
        public double HeightCm { get; set; }
    }

    /// <summary>
    /// Tracking event for an order.
    /// </summary>
    public class TrackingEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }

    public enum OrderStatus
    {
        Created = 0,
        Assigned = 1,
        InTransit = 2,
        Delivered = 3,
        Cancelled = 4
    }
}
