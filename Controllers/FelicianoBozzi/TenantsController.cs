using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers.FelicianoBozzi;

// [Authorize]
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
    public IActionResult ListTenants([FromQuery] NewTenant dto)
    {
        return Ok(_tenants.AddTenant(dto));
    }
}