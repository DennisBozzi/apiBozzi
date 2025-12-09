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
    // private readonly DemoSeedService _seedService;
    // private readonly IWebHostEnvironment _env;
    // private readonly FirebaseUserProvider _userProvider;
    //
    // public DemoController(DemoSeedService seedService, IWebHostEnvironment env, FirebaseUserProvider userProvider)
    // {
    //     _seedService = seedService;
    //     _env = env;
    //     _userProvider = userProvider;
    // }
    //
    // [HttpPost("ResetDemo")]
    // public async Task<IActionResult> ResetDemo()
    // {
    //     var path = Path.Combine(_env.ContentRootPath, "Utils/demo-seed.json");
    //     try
    //     {
    //         var result = await _seedService.SeedFromFileAsync(path);
    //         return Ok(result);
    //     }
    //     catch (FileNotFoundException)
    //     {
    //         return NotFound("Arquivo demo-seed.json não encontrado.");
    //     }
    //     catch (Exception e)
    //     {
    //         return StatusCode(500, $"Erro ao executar seed demo: {e.Message}");
    //     }
    // }
}