using System.Collections.Concurrent;
using DeliveryBackend.Models;

namespace DeliveryBackend.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, User> _users = new();
        private readonly ConcurrentDictionary<string, Guid> _emailIndex = new(StringComparer.OrdinalIgnoreCase);

        public User Add(User user)
        {
            _users[user.Id] = user;
            _emailIndex[user.Email] = user.Id;
            return user;
        }

        public User? GetByEmail(string email)
        {
            if (_emailIndex.TryGetValue(email, out var id))
            {
                return _users.TryGetValue(id, out var u) ? u : null;
            }

            return null;
        }

        public User? GetById(Guid id) => _users.TryGetValue(id, out var u) ? u : null;
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<Guid, Order> _orders = new();
        private readonly ConcurrentDictionary<Guid, List<Guid>> _userOrders = new();

        public Order Add(Order order)
        {
            _orders[order.Id] = order;
            _userOrders.AddOrUpdate(order.UserId, [order.Id], (_, list) =>
            {
                list.Add(order.Id);
                return list;
            });
            return order;
        }

        public Order? GetById(Guid id) => _orders.TryGetValue(id, out var o) ? o : null;

        public IEnumerable<Order> GetByUser(Guid userId)
        {
            if (_userOrders.TryGetValue(userId, out var list))
            {
                foreach (var id in list)
                {
                    if (_orders.TryGetValue(id, out var o))
                        yield return o;
                }
            }
        }

        public Order? Update(Order order)
        {
            if (!_orders.ContainsKey(order.Id))
                return null;

            _orders[order.Id] = order;
            return order;
        }
    }

    public class InMemoryTrackingRepository : ITrackingRepository
    {
        private readonly ConcurrentDictionary<Guid, List<TrackingEvent>> _events = new();

        public void Add(TrackingEvent trackingEvent)
        {
            var list = _events.GetOrAdd(trackingEvent.OrderId, _ => new List<TrackingEvent>());
            list.Add(trackingEvent);
        }

        public IEnumerable<TrackingEvent> GetByOrderId(Guid orderId)
        {
            if (_events.TryGetValue(orderId, out var list))
                return list.OrderBy(e => e.Timestamp);

            return Enumerable.Empty<TrackingEvent>();
        }
    }
}
