using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using DeliveryBackend.Models;
using DeliveryBackend.DTOs;
using DeliveryBackend.Services;

namespace DeliveryBackend.Controllers
{
    /// <summary>
    /// Handles user registration and login.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Tags("Users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">Registration payload with name, email, and password.</param>
        /// <returns>The created user profile with a generated userId.</returns>
        [HttpPost("register")]
        [OpenApiOperation("RegisterUser")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<UserResponse> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.FromModelState(ModelState));
            }

            var user = _userService.Register(request.Name, request.Email, request.Password);
            var dto = UserResponse.FromModel(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, dto);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Logs in a user using placeholder authentication (no tokens).
        /// </summary>
        /// <param name="request">Login payload with email and password.</param>
        /// <returns>User profile if credentials match (placeholder logic).</returns>
        [HttpPost("login")]
        [OpenApiOperation("LoginUser")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public ActionResult<UserResponse> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.FromModelState(ModelState));
            }

            var user = _userService.Login(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new ErrorResponse("Invalid email or password"));
            }

            return Ok(UserResponse.FromModel(user));
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Get a user by id.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>User details.</returns>
        [HttpGet("{id}")]
        [OpenApiOperation("GetUserById")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public ActionResult<UserResponse> GetById([FromRoute] Guid id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound(new ErrorResponse("User not found"));

            return Ok(UserResponse.FromModel(user));
        }
    }
}
