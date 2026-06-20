using Microsoft.AspNetCore.Mvc;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.API.Controllers;

[ApiController]
[Route("api/commandes")]
public sealed class CommandesController : ControllerBase
{
private readonly ICommandeService _commandeService;


public CommandesController(ICommandeService commandeService)
{
    _commandeService = commandeService;
}

[HttpGet]
[ProducesResponseType(typeof(IEnumerable<CommandeDto>), StatusCodes.Status200OK)]
public async Task<ActionResult<IEnumerable<CommandeDto>>> GetAll()
{
    IEnumerable<CommandeDto> commandes = await _commandeService.GetAllAsync();
    return Ok(commandes);
}

[HttpGet("{id:int:min(1)}")]
[ProducesResponseType(typeof(CommandeDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<CommandeDto>> GetById(int id)
{
    CommandeDto? commande = await _commandeService.GetByIdAsync(id);

    if (commande is null)
    {
        return NotFound(new
        {
            message = $"La commande avec l'identifiant {id} est introuvable."
        });
    }

    return Ok(commande);
}

[HttpPost]
[ProducesResponseType(typeof(CommandeDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<CommandeDto>> Create(
    [FromBody] CreateCommandeDto dto)
{
    try
    {
        CommandeDto commande = await _commandeService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = commande.Id },
            commande
        );
    }
    catch (BusinessRuleException exception)
    {
        return BadRequest(new
        {
            message = exception.Message
        });
    }
}

[HttpPatch("{id:int:min(1)}/statut")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> UpdateStatus(
    int id,
    [FromBody] UpdateCommandeStatusDto dto)
{
    bool modifiee = await _commandeService.UpdateStatusAsync(id, dto);

    if (!modifiee)
    {
        return NotFound(new
        {
            message = $"La commande avec l'identifiant {id} est introuvable."
        });
    }

    return NoContent();
}

[HttpPatch("{id:int:min(1)}/paiement")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> UpdatePayment(
    int id,
    [FromBody] UpdateCommandePaymentDto dto)
{
    bool modifiee = await _commandeService.UpdatePaymentAsync(id, dto);

    if (!modifiee)
    {
        return NotFound(new
        {
            message = $"La commande avec l'identifiant {id} est introuvable."
        });
    }

    return NoContent();
}

[HttpDelete("{id:int:min(1)}/definitif")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> DeletePermanently(int id)
{
    bool supprimee = await _commandeService.DeleteAsync(id);

    if (!supprimee)
    {
        return NotFound(new
        {
            message = $"La commande avec l'identifiant {id} est introuvable."
        });
    }

    return NoContent();
}


}
