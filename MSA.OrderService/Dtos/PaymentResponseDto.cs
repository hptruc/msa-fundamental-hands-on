using System.Text.Json.Serialization;

namespace MSA.OrderService.Dtos;

public class PaymentResponseDto
{
    [JsonPropertyName("paymentId")]
    public Guid PaymentId { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}