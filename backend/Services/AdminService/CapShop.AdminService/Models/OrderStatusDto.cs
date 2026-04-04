namespace CapShop.AdminService.Models;

public class OrderStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? PaymentStatus { get; set; }
}