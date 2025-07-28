using System.ComponentModel.DataAnnotations;
using apiBozzi.Models.Dtos;
using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers.FelicianoBozzi;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TenantsController : ControllerBase
{
    private readonly TenantService _tenants;

    public TenantsController(TenantService tenants)
    {
        _tenants = tenants;
    }

    [HttpPost]
    public async Task<IActionResult> AddTenant([FromBody] NewTenant dto)
    {
        try
        {
            var tenant = await _tenants.AddTenant(dto);
            return Ok(tenant);
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

    [HttpGet]
    public async Task<IActionResult> ListTenants([FromQuery] TenantFilter filter)
    {
        try
        {
            var tenant = await _tenants.ListTenants(filter);
            return Ok(tenant);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Erro interno do servidor: ${e.Message}");
        }
    }
}