using Microsoft.AspNetCore.Mvc;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.API.Controllers;

[ApiController]
[Route("api/produits")]
public sealed class ProduitsController : ControllerBase
{
private readonly IProduitService _produitService;


public ProduitsController(IProduitService produitService)
{
    _produitService = produitService;
}

[HttpGet]
[ProducesResponseType(
    typeof(IEnumerable<ProduitDto>),
    StatusCodes.Status200OK)]
public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll()
{
    IEnumerable<ProduitDto> produits =
        await _produitService.GetAllAsync();

    return Ok(produits);
}

[HttpGet("{id:int:min(1)}")]
[ProducesResponseType(
    typeof(ProduitDto),
    StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<ProduitDto>> GetById(int id)
{
    ProduitDto? produit =
        await _produitService.GetByIdAsync(id);

    if (produit is null)
    {
        return NotFound(new
        {
            message =
                $"Le produit avec l'identifiant {id} est introuvable."
        });
    }

    return Ok(produit);
}

[HttpPost]
[ProducesResponseType(
    typeof(ProduitDto),
    StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public async Task<ActionResult<ProduitDto>> Create(
    [FromBody] CreateProduitDto dto)
{
    try
    {
        ProduitDto produit =
            await _produitService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = produit.Id },
            produit);
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
[ProducesResponseType(
    typeof(ProduitDto),
    StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public async Task<ActionResult<ProduitDto>> Update(
    int id,
    [FromBody] UpdateProduitDto dto)
{
    try
    {
        ProduitDto? produit =
            await _produitService.UpdateAsync(id, dto);

        if (produit is null)
        {
            return NotFound(new
            {
                message =
                    $"Le produit avec l'identifiant {id} est introuvable."
            });
        }

        return Ok(produit);
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
    bool desactive =
        await _produitService.DisableAsync(id);

    if (!desactive)
    {
        return NotFound(new
        {
            message =
                $"Le produit avec l'identifiant {id} est introuvable ou deja inactif."
        });
    }

    return NoContent();
}

[HttpPatch("{id:int:min(1)}/reactiver")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Reactivate(int id)
{
    bool reactive =
        await _produitService.ReactivateAsync(id);

    if (!reactive)
    {
        return NotFound(new
        {
            message =
                $"Le produit avec l'identifiant {id} est introuvable ou deja actif."
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
bool supprime =
await _produitService.DeleteAsync(id);


    if (!supprime)
    {
        return NotFound(new
        {
            message =
                $"Le produit avec l'identifiant {id} est introuvable."
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
