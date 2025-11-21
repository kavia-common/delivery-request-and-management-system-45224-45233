using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using DeliveryBackend.Services;
using DeliveryBackend.DTOs;
using DeliveryBackend.Models;

namespace DeliveryBackend.Controllers
{
    /// <summary>
    /// Handles delivery order creation and management.
    /// </summary>
    [ApiController]
    [Route("api/orders")]
    [Tags("Orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrdersController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Creates a new delivery order.
        /// </summary>
        /// <param name="request">Order creation payload including userId, origin, destination, and package details.</param>
        /// <returns>The created order.</returns>
        [HttpPost]
        [OpenApiOperation("CreateOrder")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<OrderResponse> Create([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResponse.FromModelState(ModelState));

            var user = _userService.GetById(request.UserId);
            if (user == null)
                return BadRequest(new ErrorResponse("Invalid userId"));

            var order = _orderService.CreateOrder(request.UserId, request.Origin, request.Destination, request.Package);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, OrderResponse.FromModel(order));
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Gets order by id.
        /// </summary>
        /// <param name="id">Order id.</param>
        /// <returns>Order details.</returns>
        [HttpGet("{id}")]
        [OpenApiOperation("GetOrderById")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public ActionResult<OrderResponse> GetById([FromRoute] Guid id)
        {
            var order = _orderService.GetById(id);
            if (order == null)
                return NotFound(new ErrorResponse("Order not found"));

            return Ok(OrderResponse.FromModel(order));
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Lists all orders for a specific user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>List of orders.</returns>
        [HttpGet("user/{userId}")]
        [OpenApiOperation("ListUserOrders")]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<OrderResponse>> ListByUser([FromRoute] Guid userId)
        {
            var orders = _orderService.GetOrdersByUser(userId).Select(OrderResponse.FromModel);
            return Ok(orders);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="id">Order id.</param>
        /// <param name="request">New status value.</param>
        /// <returns>Updated order.</returns>
        [HttpPatch("{id}/status")]
        [OpenApiOperation("UpdateOrderStatus")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public ActionResult<OrderResponse> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResponse.FromModelState(ModelState));

            var updated = _orderService.UpdateStatus(id, request.Status);
            if (updated == null)
                return NotFound(new ErrorResponse("Order not found"));

            return Ok(OrderResponse.FromModel(updated));
        }
    }
}
