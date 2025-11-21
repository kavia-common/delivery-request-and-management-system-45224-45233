using DeliveryBackend.Models;
using DeliveryBackend.Repositories;

namespace DeliveryBackend.Services
{
    public interface IUserService
    {
        User Register(string name, string email, string password);
        User? Login(string email, string password);
        User? GetById(Guid id);
    }

    public interface IOrderService
    {
        Order CreateOrder(Guid userId, string origin, string destination, PackageDetailsRequest packageRequest);
        Order? GetById(Guid id);
        IEnumerable<Order> GetOrdersByUser(Guid userId);
        Order? UpdateStatus(Guid id, OrderStatus status);
    }

    public interface ITrackingService
    {
        IEnumerable<TrackingEvent> GetTrackingEvents(Guid orderId);
        void AddEvent(Guid orderId, string status, string location, string? notes = null);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public User? GetById(Guid id) => _repo.GetById(id);

        public User? Login(string email, string password)
        {
            var user = _repo.GetByEmail(email);
            if (user == null) return null;

            // Placeholder auth: plain comparison (not for production)
            return user.Password == password ? user : null;
        }

        public User Register(string name, string email, string password)
        {
            var existing = _repo.GetByEmail(email);
            if (existing != null)
                throw new InvalidOperationException("Email is already registered");

            var u = new User
            {
                Name = name.Trim(),
                Email = email.Trim(),
                Password = password // placeholder
            };

            return _repo.Add(u);
        }
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orders;
        private readonly ITrackingRepository _tracking;

        public OrderService(IOrderRepository orders, ITrackingRepository tracking)
        {
            _orders = orders;
            _tracking = tracking;
        }

        public Order CreateOrder(Guid userId, string origin, string destination, PackageDetailsRequest packageRequest)
        {
            var order = new Order
            {
                UserId = userId,
                Origin = origin.Trim(),
                Destination = destination.Trim(),
                Package = packageRequest.ToModel(),
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow
            };

            _orders.Add(order);

            // Create initial tracking event
            _tracking.Add(new TrackingEvent
            {
                OrderId = order.Id,
                Status = "Created",
                Location = origin,
                Notes = "Order created"
            });

            return order;
        }

        public Order? GetById(Guid id) => _orders.GetById(id);

        public IEnumerable<Order> GetOrdersByUser(Guid userId) => _orders.GetByUser(userId);

        public Order? UpdateStatus(Guid id, OrderStatus status)
        {
            var o = _orders.GetById(id);
            if (o == null) return null;

            o.Status = status;
            o.UpdatedAt = DateTime.UtcNow;

            var updated = _orders.Update(o);
            if (updated != null)
            {
                _tracking.Add(new TrackingEvent
                {
                    OrderId = o.Id,
                    Status = status.ToString(),
                    Location = status == OrderStatus.Delivered ? o.Destination : o.Origin,
                    Notes = $"Status changed to {status}"
                });
            }

            return updated;
        }
    }

    public class TrackingService : ITrackingService
    {
        private readonly ITrackingRepository _repo;

        public TrackingService(ITrackingRepository repo)
        {
            _repo = repo;
        }

        public void AddEvent(Guid orderId, string status, string location, string? notes = null)
        {
            _repo.Add(new TrackingEvent
            {
                OrderId = orderId,
                Status = status,
                Location = location,
                Notes = notes
            });
        }

        public IEnumerable<TrackingEvent> GetTrackingEvents(Guid orderId) => _repo.GetByOrderId(orderId);
    }

    // Helper DTO for package request (referenced by OrderService)
    public class PackageDetailsRequest
    {
        public string Description { get; set; } = string.Empty;
        public double WeightKg { get; set; }
        public double LengthCm { get; set; }
        public double WidthCm { get; set; }
        public double HeightCm { get; set; }
    }
}
