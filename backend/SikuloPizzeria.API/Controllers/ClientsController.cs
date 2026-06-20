using Microsoft.AspNetCore.Mvc;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.API.Controllers;

[ApiController]
[Route("api/clients")]
public sealed class ClientsController : ControllerBase
{
private readonly IClientService _clientService;


public ClientsController(IClientService clientService)
{
    _clientService = clientService;
}

[HttpGet]
[ProducesResponseType(typeof(IEnumerable<Client>), StatusCodes.Status200OK)]
public async Task<ActionResult<IEnumerable<Client>>> GetAll()
{
    IEnumerable<Client> clients = await _clientService.GetAllAsync();
    return Ok(clients);
}

[HttpGet("{id:int:min(1)}")]
[ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<Client>> GetById(int id)
{
    Client? client = await _clientService.GetByIdAsync(id);

    if (client is null)
    {
        return NotFound(new
        {
            message = $"Le client avec l'identifiant {id} est introuvable."
        });
    }

    return Ok(client);
}

[HttpPost]
[ProducesResponseType(typeof(Client), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public async Task<ActionResult<Client>> Create([FromBody] CreateClientDto dto)
{
    try
    {
        Client client = await _clientService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = client.Id },
            client
        );
    }
    catch (DuplicateResourceException exception)
    {
        return Conflict(new
        {
            message = exception.Message
        });
    }
    catch (BusinessRuleException exception)
    {
        return BadRequest(new
        {
            message = exception.Message
        });
    }
}

[HttpPut("{id:int:min(1)}")]
[ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public async Task<ActionResult<Client>> Update(
    int id,
    [FromBody] UpdateClientDto dto)
{
    try
    {
        Client? client = await _clientService.UpdateAsync(id, dto);

        if (client is null)
        {
            return NotFound(new
            {
                message = $"Le client avec l'identifiant {id} est introuvable."
            });
        }

        return Ok(client);
    }
    catch (DuplicateResourceException exception)
    {
        return Conflict(new
        {
            message = exception.Message
        });
    }
    catch (BusinessRuleException exception)
    {
        return BadRequest(new
        {
            message = exception.Message
        });
    }
}

[HttpDelete("{id:int:min(1)}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Disable(int id)
{
    bool desactive = await _clientService.DisableAsync(id);

    if (!desactive)
    {
        return NotFound(new
        {
            message = $"Le client avec l'identifiant {id} est introuvable ou déjà inactif."
        });
    }

    return NoContent();
}

[HttpPatch("{id:int:min(1)}/reactiver")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Reactivate(int id)
{
    bool reactive = await _clientService.ReactivateAsync(id);

    if (!reactive)
    {
        return NotFound(new
        {
            message = $"Le client avec l'identifiant {id} est introuvable ou déjà actif."
        });
    }

    return NoContent();
}

[HttpDelete("{id:int:min(1)}/definitif")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public async Task<IActionResult> DeletePermanently(int id)
{
    try
    {
        bool supprime = await _clientService.DeletePermanentlyAsync(id);

        if (!supprime)
        {
            return NotFound(new
            {
                message = $"Le client avec l'identifiant {id} est introuvable."
            });
        }

        return NoContent();
    }
    catch (BusinessRuleException exception)
    {
        return Conflict(new
        {
            message = exception.Message
        });
    }
}

}
