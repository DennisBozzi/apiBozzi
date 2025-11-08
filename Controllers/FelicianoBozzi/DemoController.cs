using apiBozzi.Services.FelicianoBozzi;
using apiBozzi.Services.Firebase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DemoController : ControllerBase
{
    private readonly DemoSeedService _seedService;
    private readonly IWebHostEnvironment _env;
    private readonly FirebaseUserProvider _userProvider;

    public DemoController(DemoSeedService seedService, IWebHostEnvironment env, FirebaseUserProvider userProvider)
    {
        _seedService = seedService;
        _env = env;
        _userProvider = userProvider;
    }

    // Reinicia e popula os dados demo a partir do arquivo demo-seed.json na raiz do projeto
    [HttpPost("ResetDemo")]
    public async Task<IActionResult> ResetDemo()
    {
        if (!_userProvider.IsAdmin)
            return Forbid();

        var path = Path.Combine(_env.ContentRootPath, "demo-seed.json");
        try
        {
            var result = await _seedService.SeedFromFileAsync(path);
            return Ok(result);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Arquivo demo-seed.json não encontrado.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Erro ao executar seed demo: {e.Message}");
        }
    }
}
