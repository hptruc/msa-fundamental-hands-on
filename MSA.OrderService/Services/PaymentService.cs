using System.Text;
using System.Text.Json;
using MSA.OrderService.Dtos;

namespace MSA.OrderService.Services;

public class PaymentService(HttpClient httpClient) : IPaymentService
{
    public async Task<PaymentResponseDto?> PaymentProcessAsync(PaymentRequestDto paymentRequestDto, string token)
    {
        var jsonContent = JsonSerializer.Serialize(paymentRequestDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Add("Authorization", token);

        var response = await httpClient.PostAsync("v1/payment", content);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var paymentResponseDto = JsonSerializer.Deserialize<PaymentResponseDto>(responseContent);

        return paymentResponseDto;
    }
}