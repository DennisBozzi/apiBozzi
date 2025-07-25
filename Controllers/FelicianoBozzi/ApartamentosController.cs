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
public class ApartamentosController : ControllerBase
{
    private readonly ApartamentoService _apartamento;

    public ApartamentosController(ApartamentoService apartamento)
    {
        _apartamento = apartamento;
    }

    [HttpGet]
    public async Task<ServiceResponse<PagedResult<Apartamento>>> ListarApartamentos([FromQuery] ApartamentoFiltro apartamentoFiltro)
    {
        return await _apartamento.ListerApartamentos(apartamentoFiltro);
    }

    [HttpPost]
    public async Task<ServiceResponse<Apartamento>> CriarApartamento(string numero, int andar, decimal valor)
    {
        return await _apartamento.CriarApartamento(numero, andar, valor);
    }
}