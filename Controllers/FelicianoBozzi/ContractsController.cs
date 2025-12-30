using apiBozzi.Configurations.Transaction;
using apiBozzi.Models.Dtos;
using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers.FelicianoBozzi;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ContractsController : ControllerBase
{
    private readonly ContractService _contracts;

    public ContractsController(ContractService contracts)
    {
        _contracts = contracts;
    }

    [HttpPost]
    [Authorize]
    [Transaction]
    public async Task<IActionResult> NewContract([FromBody] NewContract dto)
    {
        try
        {
            var contract = await _contracts.NewContract(dto);
            return Ok(contract);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Server error: ${e.Message}");
        }
    }
}