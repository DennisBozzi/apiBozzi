using apiBozzi.Exceptions;
using apiBozzi.Models.Dtos;
using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers.FelicianoBozzi;

// [Authorize]
[ApiController]
[Route("[controller]")]
public class ApartmentsController : ControllerBase
{
    private readonly ApartmentService _apartment;

    public ApartmentsController(ApartmentService apartment)
    {
        _apartment = apartment;
    }

    [HttpGet]
    public async Task<IActionResult> ListApartments([FromQuery] ApartmentFilter apartmentFiltro)
    {
        try
        {
            var apartments = await _apartment.ListApartments(apartmentFiltro);
            return Ok(apartments);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Erro interno do servidor: ${e.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOneApartment(int id)
    {
        try
        {
            var ap = await _apartment.GetApartmentById(id);
            return Ok(ap);
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
    public async Task<IActionResult> AddApartment(string numero, int andar, decimal valor)
    {
        try
        {
            var apartment = await _apartment.AddApartment(numero, andar, valor);
            return Ok(apartment);
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

    [HttpPut("MakeResponsible")]
    public async Task<IActionResult> MakeResponsible(int apId, int tenId)
    {
        try
        {
            var apartment = await _apartment.MakeResponsible(apId, tenId);
            return Ok(apartment);
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