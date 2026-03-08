using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers.FelicianoBozzi;

[ApiController]
[Route("[controller]")]
public class PaymentController(PaymentService paymentService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        try
        {
            var contract = await paymentService.CreatePaymentsByContract(id);
            if (contract == null) return NotFound();

            return Ok(contract);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}