using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSA.BankService.Dtos;
using MSA.BankService.Services;

namespace MSA.BankService.Controllers;

[ApiController]
[Route("v1/payment")]
[Authorize]
public class PaymentController(IPaymentService paymentService) : ControllerBase
{
    [HttpGet("orderId")]
    [Authorize("read_access")]
    public async Task<ActionResult<PaymentDto>> GetPaymentByOrderIdAsync(Guid orderId)
    {
        return Ok(await paymentService.GetPaymentByOrderIdAsync(orderId));
    }

    [HttpPost]
    [Authorize("write_access")]
    public async Task<ActionResult<PaymentResponseDto>> ProcessPaymentAsync(PaymentRequestDto paymentRequestDto)
    {
        return Ok(await paymentService.ProcessPaymentAsync(paymentRequestDto));
    }
}