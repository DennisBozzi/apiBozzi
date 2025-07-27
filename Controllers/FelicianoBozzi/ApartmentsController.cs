using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Services.FelicianoBozzi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Controllers.FelicianoBozzi;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ApartmentsController : ControllerBase
{
    private readonly ApartamentoService _apartment;

    public ApartmentsController(ApartamentoService apartament)
    {
        _apartment = apartament;
    }

    [HttpGet]
    public async Task<ServiceResponse<PagedResult<Apartment>>> ListApartments([FromQuery] ApartamentoFiltro apartmentFiltro)
    {
        return await _apartment.ListerApartamentos(apartmentFiltro);
    }

    [HttpPost]
    public async Task<ServiceResponse<Apartment>> CriarApartamento(string numero, int andar, decimal valor)
    {
        return await _apartment.CriarApartamento(numero, andar, valor);
    }
}