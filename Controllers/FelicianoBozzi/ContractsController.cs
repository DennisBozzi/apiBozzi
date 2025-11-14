using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers.FelicianoBozzi;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ContractsController : ControllerBase
{
    private readonly ContractService _contracts;

    public ContractsController(ContractService contracts)
    {
        _contracts = contracts;
    }
}