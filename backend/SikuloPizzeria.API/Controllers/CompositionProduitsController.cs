using Microsoft.AspNetCore.Mvc;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.API.Controllers;

[ApiController]
[Route("api/composition-produits")]
public sealed class CompositionProduitsController : ControllerBase
{
private readonly ICompositionProduitService _compositionService;


public CompositionProduitsController(
    ICompositionProduitService compositionService)
{
    _compositionService = compositionService;
}

[HttpGet]
public async Task<ActionResult<IEnumerable<CompositionProduitDto>>> GetAll()
{
    IEnumerable<CompositionProduitDto> compositions =
        await _compositionService.GetAllAsync();

    return Ok(compositions);
}

[HttpGet("produit/{produitId:int}")]
public async Task<ActionResult<IEnumerable<CompositionProduitDto>>> GetByProduitId(
    int produitId)
{
    IEnumerable<CompositionProduitDto> compositions =
        await _compositionService.GetByProduitIdAsync(produitId);

    return Ok(compositions);
}

[HttpGet("{id:int}")]
public async Task<ActionResult<CompositionProduitDto>> GetById(int id)
{
    CompositionProduitDto? composition =
        await _compositionService.GetByIdAsync(id);

    if (composition is null)
    {
        return NotFound();
    }

    return Ok(composition);
}

[HttpPost]
public async Task<ActionResult<CompositionProduitDto>> Create(
    CreateCompositionProduitDto dto)
{
    try
    {
        CompositionProduitDto composition =
            await _compositionService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = composition.Id },
            composition
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

[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id)
{
    bool supprimee =
        await _compositionService.DeleteAsync(id);

    if (!supprimee)
    {
        return NotFound();
    }

    return NoContent();
}


}
