using DeliveryBackend.Models;

namespace DeliveryBackend.Repositories
{
    public interface IUserRepository
    {
        User Add(User user);
        User? GetById(Guid id);
        User? GetByEmail(string email);
    }

    public interface IOrderRepository
    {
        Order Add(Order order);
        Order? GetById(Guid id);
        IEnumerable<Order> GetByUser(Guid userId);
        Order? Update(Order order);
    }

    public interface ITrackingRepository
    {
        IEnumerable<TrackingEvent> GetByOrderId(Guid orderId);
        void Add(TrackingEvent trackingEvent);
    }
}
