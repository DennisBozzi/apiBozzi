using apiBozzi.Models.Dtos;
using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using apiBozzi.Configurations.Transaction;
using apiBozzi.Context;

namespace apiBozzi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class SeedController(TenantService tenantService, UnitService unitService, IWebHostEnvironment env)
    : ControllerBase
{
    [HttpPost("Run")]
    [Transaction]
    public async Task<IActionResult> RunSeed()
    {
        try
        {
            var seedDataPath = Path.Combine(env.ContentRootPath, "Configurations", "SeedData");

            var tenantsFilePath = Path.Combine(seedDataPath, "tenants.json");
            var unitsFilePath = Path.Combine(seedDataPath, "units.json");

            var erros = new List<string>();
            var unitsAdded = 0;
            var tenantsAdded = 0;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Seed Tenants
            if (System.IO.File.Exists(tenantsFilePath))
            {
                var tenantsJson = await System.IO.File.ReadAllTextAsync(tenantsFilePath);
                var tenantsList = JsonSerializer.Deserialize<List<NewTenant>>(tenantsJson, options);

                if (tenantsList != null)
                {
                    foreach (var tenant in tenantsList)
                    {
                        try
                        {
                            await tenantService.AddTenant(tenant);
                            tenantsAdded++;
                        }
                        catch (Exception ex)
                        {
                            erros.Add($"Error adding tenant {tenant.Cpf ?? tenant.FirstName}: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                erros.Add($"File not found: {tenantsFilePath}");
            }

            // Seed Units
            if (System.IO.File.Exists(unitsFilePath))
            {
                var unitsJson = await System.IO.File.ReadAllTextAsync(unitsFilePath);
                var unitsList = JsonSerializer.Deserialize<List<NewUnit>>(unitsJson, options);

                if (unitsList != null)
                {
                    foreach (var unit in unitsList)
                    {
                        try
                        {
                            await unitService.AddUnit(unit);
                            unitsAdded++;
                        }
                        catch (Exception ex)
                        {
                            erros.Add($"Error adding unit {unit.Number}: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                erros.Add($"File not found: {unitsFilePath}");
            }

            var results = new
            {
                tenantsAdded,
                unitsAdded,
                erros
            };

            return Ok(results);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Error = e.Message });
        }
    }
}