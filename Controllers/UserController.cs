using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{number}")]
    public async Task<bool> IsHigherNumber(int number)
    {
        if (number > 9)
            return true;

        return false;
    }
}