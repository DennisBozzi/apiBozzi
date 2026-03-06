using apiBozzi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers;

[Route("[controller]")]
[ApiController]
public class FileController(FileService fileService) : ControllerBase
{
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Download(int id)
    {
        try
        {
            var res = await fileService.GetFileByIdAsync(id, withUrl: true);
            if (res is null)
                return NotFound();

            return Ok(res);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}