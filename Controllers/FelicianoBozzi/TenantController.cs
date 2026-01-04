using System.ComponentModel.DataAnnotations;
using apiBozzi.Configurations.Transaction;
using apiBozzi.Models.Dtos;
using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers.FelicianoBozzi;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TenantController : ControllerBase
{
    private readonly TenantService _tenants;
    private readonly ContractService _contract;

    public TenantController(TenantService tenants, ContractService contract)
    {
        _tenants = tenants;
        _contract = contract;
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
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}/Contracts")]
    public async Task<IActionResult> GetContract(int id, bool active)
    {
        try
        {
            return Ok(await _contract.GetContractsByTenant(id, active));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Transaction]
    public async Task<IActionResult> AddTenant([FromBody] NewTenant dto)
    {
        try
        {
            var tenant = await _tenants.AddTenant(dto);
            return Ok(tenant);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTenantById(int id)
    {
        try
        {
            var ten = await _tenants.GetTenantById(id);

            if (ten == null) return NotFound();

            return Ok(ten);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}