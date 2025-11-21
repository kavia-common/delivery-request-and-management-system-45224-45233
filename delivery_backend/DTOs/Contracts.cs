using System.ComponentModel.DataAnnotations;
using DeliveryBackend.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DeliveryBackend.DTOs
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }

        public ErrorResponse(string message, IDictionary<string, string[]>? errors = null)
        {
            Message = message;
            Errors = errors;
        }

        public static ErrorResponse FromModelState(ModelStateDictionary modelState)
        {
            var errors = modelState.Where(kvp => kvp.Value?.Errors.Any() == true)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value" : e.ErrorMessage).ToArray()
                );

            return new ErrorResponse("Validation failed", errors);
        }
    }

    public class RegisterRequest
    {
        [Required, MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class CreateOrderRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required, MinLength(3)]
        public string Origin { get; set; } = string.Empty;

        [Required, MinLength(3)]
        public string Destination { get; set; } = string.Empty;

        [Required]
        public PackageDetailsRequest Package { get; set; } = new PackageDetailsRequest();
    }

    public class PackageDetailsRequest
    {
        [Required, MinLength(2)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 1000)]
        public double WeightKg { get; set; }

        [Range(0.1, 1000)]
        public double LengthCm { get; set; }

        [Range(0.1, 1000)]
        public double WidthCm { get; set; }

        [Range(0.1, 1000)]
        public double HeightCm { get; set; }

        public PackageDetails ToModel() => new PackageDetails
        {
            Description = Description,
            WeightKg = WeightKg,
            LengthCm = LengthCm,
            WidthCm = WidthCm,
            HeightCm = HeightCm
        };
    }

    public class UpdateOrderStatusRequest
    {
        [Required]
        public OrderStatus Status { get; set; }
    }

    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public static UserResponse FromModel(User u) => new UserResponse
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            CreatedAt = u.CreatedAt
        };
    }

    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public PackageDetailsResponse Package { get; set; } = new PackageDetailsResponse();
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public static OrderResponse FromModel(Models.Order o) => new OrderResponse
        {
            Id = o.Id,
            UserId = o.UserId,
            Origin = o.Origin,
            Destination = o.Destination,
            Package = PackageDetailsResponse.FromModel(o.Package),
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        };
    }

    public class PackageDetailsResponse
    {
        public string Description { get; set; } = string.Empty;
        public double WeightKg { get; set; }
        public double LengthCm { get; set; }
        public double WidthCm { get; set; }
        public double HeightCm { get; set; }

        public static PackageDetailsResponse FromModel(PackageDetails p) => new PackageDetailsResponse
        {
            Description = p.Description,
            WeightKg = p.WeightKg,
            LengthCm = p.LengthCm,
            WidthCm = p.WidthCm,
            HeightCm = p.HeightCm
        };
    }

    public class TrackingEventResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Notes { get; set; }

        public static TrackingEventResponse FromModel(TrackingEvent e) => new TrackingEventResponse
        {
            Id = e.Id,
            OrderId = e.OrderId,
            Status = e.Status,
            Location = e.Location,
            Timestamp = e.Timestamp,
            Notes = e.Notes
        };
    }
}
