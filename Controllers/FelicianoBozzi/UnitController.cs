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
        catch (Exception e)
        {
            return StatusCode(500, $"Erro interno do servidor: ${e.Message}");
        }
    }

    // [HttpGet("Available")]
    // public async Task<IActionResult> ListAvailableApartments([FromQuery] ApartmentFilter apartmentFiltro)
    // {
    //     try
    //     {
    //         var apartments = await _apartment.ListAvailableApartments(apartmentFiltro);
    //         return Ok(apartments);
    //     }
    //     catch (Exception e)
    //     {
    //         return StatusCode(500, $"Erro interno do servidor: ${e.Message}");
    //     }
    // }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOneApartmentById(int id)
    {
        try
        {
            var uni = await _unit.GetUnitById(id);

            if (uni == null) return NotFound();

            return Ok(uni);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Erro interno do servidor: ${e.Message}");
        }
    }

    // [HttpGet("number/{number}")]
    // public async Task<IActionResult> GetOneApartmentByNumber(string number)
    // {
    //     try
    //     {
    //         var ap = await _apartment.GetApartmentByNumber(number);
    //         return Ok(ap);
    //     }
    //     catch (ValidationException e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    //     catch (Exception e)
    //     {
    //         return StatusCode(500, $"Erro interno do servidor: ${e.Message}");
    //     }
    // }

    [HttpPost]
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