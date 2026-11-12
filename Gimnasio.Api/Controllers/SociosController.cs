using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SociosController : ControllerBase
{
    private readonly ISocioService _socioService;

    public SociosController(ISocioService socioService)
    {
        _socioService = socioService;
    }

    /// <summary>Lista socios con filtros opcionales por query string.</summary>
    /// <remarks>Ejemplo: GET api/socios?nombre=ra&amp;idMembresia=1&amp;activo=true</remarks>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SocioDto>>> Listar([FromQuery] SocioFiltroDto filtro)
    {
        var socios = await _socioService.ListarAsync(filtro);
        return Ok(socios);
    }

    /// <summary>Obtiene un socio por su id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SocioDto>> ObtenerPorId(int id)
    {
        var socio = await _socioService.ObtenerPorIdAsync(id);
        if (socio is null)
            return NotFound(new { mensaje = $"No existe el socio {id}." });

        return Ok(socio);
    }

    /// <summary>Inscribe un socio nuevo (nace activo).</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SocioDto>> Crear(SocioCrearDto dto)
    {
        try
        {
            var creado = await _socioService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdSocio }, creado);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    /// <summary>Actualiza los datos de un socio.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, SocioActualizarDto dto)
    {
        try
        {
            var actualizado = await _socioService.ActualizarAsync(id, dto);
            if (!actualizado)
                return NotFound(new { mensaje = $"No existe el socio {id}." });

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    /// <summary>Elimina un socio.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _socioService.EliminarAsync(id);
        if (!eliminado)
            return NotFound(new { mensaje = $"No existe el socio {id}." });

        return NoContent();
    }
}
