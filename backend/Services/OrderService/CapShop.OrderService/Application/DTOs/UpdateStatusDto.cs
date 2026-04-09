namespace CapShop.OrderService.Application.DTOs;

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? PaymentStatus { get; set; }
}
