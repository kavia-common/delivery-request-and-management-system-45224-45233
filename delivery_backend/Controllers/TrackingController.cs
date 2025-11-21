using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using DeliveryBackend.Services;
using DeliveryBackend.DTOs;

namespace DeliveryBackend.Controllers
{
    /// <summary>
    /// Provides tracking info for orders.
    /// </summary>
    [ApiController]
    [Route("api/tracking")]
    [Tags("Tracking")]
    public class TrackingController : ControllerBase
    {
        private readonly ITrackingService _trackingService;
        private readonly IOrderService _orderService;

        public TrackingController(ITrackingService trackingService, IOrderService orderService)
        {
            _trackingService = trackingService;
            _orderService = orderService;
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Gets tracking updates for an order.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>List of tracking events.</returns>
        [HttpGet("{orderId}")]
        [OpenApiOperation("GetTrackingUpdates")]
        [ProducesResponseType(typeof(IEnumerable<TrackingEventResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<TrackingEventResponse>> Get([FromRoute] Guid orderId)
        {
            var order = _orderService.GetById(orderId);
            if (order == null)
                return NotFound(new ErrorResponse("Order not found"));

            var events = _trackingService.GetTrackingEvents(orderId).Select(TrackingEventResponse.FromModel);
            return Ok(events);
        }
    }
}
