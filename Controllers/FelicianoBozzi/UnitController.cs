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
public class UnitController : ControllerBase
{
    private readonly UnitService _unit;
    private readonly ContractService _contract;

    public UnitController(UnitService unit, ContractService contract)
    {
        _unit = unit;
        _contract = contract;
    }

    [HttpGet]
    public async Task<IActionResult> ListUnits([FromQuery] UnitFilter unitFilter)
    {
        try
        {
            return Ok(await _unit.ListUnits(unitFilter));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}/Contracts")]
    public async Task<IActionResult> GetContract(int id, bool active)
    {
        try
        {
            return Ok(await _contract.GetContractByUnit(id));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUnitById(int id)
    {
        try
        {
            var uni = await _unit.GetUnitById(id);

            if (uni == null) return NotFound();

            return Ok(uni);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Transaction]
    public async Task<IActionResult> AddUnit(NewUnit value)
    {
        try
        {
            return Ok(await _unit.AddUnit(value));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}