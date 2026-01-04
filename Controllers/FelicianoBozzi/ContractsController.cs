using apiBozzi.Configurations.Transaction;
using apiBozzi.Exceptions;
using apiBozzi.Models.Dtos;
using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers.FelicianoBozzi;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ContractController : ControllerBase
{
    private readonly ContractService _contracts;

    public ContractController(ContractService contracts)
    {
        _contracts = contracts;
    }

    [HttpPost]
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
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet]
    [Transaction]
    public async Task<IActionResult> GetContractById(int id)
    {
        try
        {
            var contract = await _contracts.GetContractById(id);
            return Ok(contract);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("Model")]
    [Transaction]
    public async Task<IActionResult> NewModel(IFormFile file)
    {
        try
        {
            var res = await _contracts.NewModel(file);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("Model")]
    public async Task<IActionResult> GetModel()
    {
        try
        {
            var res = await _contracts.GetModel();
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("Model/Example")]
    public async Task<IActionResult> FillModel([FromBody] ContractModelFillRequest request)
    {
        try
        {
            var (fileName, stream) = await _contracts.FillModelAsync(request);
            return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}