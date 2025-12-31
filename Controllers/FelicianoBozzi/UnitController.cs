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

    public UnitController(UnitService unit)
    {
        _unit = unit;
    }

    [HttpGet]
    public async Task<IActionResult> ListUnits([FromQuery] UnitFilter unitFilter)
    {
        try
        {
            return Ok(await _unit.ListUnits(unitFilter));
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Erro interno do servidor: ${e.Message}");
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
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Erro interno do servidor: ${e.Message}");
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
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Erro interno do servidor: ${e.Message}");
        }
    }
}