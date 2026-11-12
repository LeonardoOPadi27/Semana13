using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MembresiasController : ControllerBase
{
    private readonly IMembresiaService _membresiaService;

    public MembresiasController(IMembresiaService membresiaService)
    {
        _membresiaService = membresiaService;
    }

    /// <summary>Lista todas las membresías.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MembresiaDto>>> Listar()
    {
        var membresias = await _membresiaService.ListarAsync();
        return Ok(membresias);
    }

    /// <summary>Obtiene una membresía por su id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembresiaDto>> ObtenerPorId(int id)
    {
        var membresia = await _membresiaService.ObtenerPorIdAsync(id);
        if (membresia is null)
            return NotFound(new { mensaje = $"No existe la membresía {id}." });

        return Ok(membresia);
    }

    /// <summary>Ruta anidada: socios inscritos en una membresía.</summary>
    [HttpGet("{id:int}/socios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<SocioDto>>> ListarSocios(int id)
    {
        var socios = await _membresiaService.ListarSociosAsync(id);
        if (socios is null)
            return NotFound(new { mensaje = $"No existe la membresía {id}." });

        return Ok(socios);
    }

    /// <summary>Crea una membresía nueva (nace activa).</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MembresiaDto>> Crear(MembresiaCrearDto dto)
    {
        try
        {
            var creada = await _membresiaService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.IdMembresia }, creada);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    /// <summary>Actualiza una membresía existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, MembresiaActualizarDto dto)
    {
        try
        {
            var actualizada = await _membresiaService.ActualizarAsync(id, dto);
            if (!actualizada)
                return NotFound(new { mensaje = $"No existe la membresía {id}." });

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    /// <summary>Elimina una membresía (solo si no tiene socios).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var eliminada = await _membresiaService.EliminarAsync(id);
            if (!eliminada)
                return NotFound(new { mensaje = $"No existe la membresía {id}." });

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
}
