using Microsoft.AspNetCore.Mvc;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.API.Controllers;

[ApiController]
[Route("api/ingredients")]
public sealed class IngredientsController : ControllerBase
{
private readonly IIngredientService _ingredientService;


public IngredientsController(IIngredientService ingredientService)
{
    _ingredientService = ingredientService;
}

[HttpGet]
public async Task<ActionResult<IEnumerable<Ingredient>>> GetAll()
{
    IEnumerable<Ingredient> ingredients = await _ingredientService.GetAllAsync();
    return Ok(ingredients);
}

[HttpGet("{id:int:min(1)}")]
public async Task<ActionResult<Ingredient>> GetById(int id)
{
    Ingredient? ingredient = await _ingredientService.GetByIdAsync(id);

    if (ingredient is null)
    {
        return NotFound(new
        {
            message = $"L'ingrédient avec l'identifiant {id} est introuvable."
        });
    }

    return Ok(ingredient);
}

[HttpPost]
public async Task<ActionResult<Ingredient>> Create([FromBody] CreateIngredientDto dto)
{
    try
    {
        Ingredient ingredient = await _ingredientService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = ingredient.Id },
            ingredient
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
public async Task<ActionResult<Ingredient>> Update(
    int id,
    [FromBody] UpdateIngredientDto dto)
{
    try
    {
        Ingredient? ingredient = await _ingredientService.UpdateAsync(id, dto);

        if (ingredient is null)
        {
            return NotFound(new
            {
                message = $"L'ingrédient avec l'identifiant {id} est introuvable."
            });
        }

        return Ok(ingredient);
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
public async Task<IActionResult> Disable(int id)
{
    bool desactive = await _ingredientService.DisableAsync(id);

    if (!desactive)
    {
        return NotFound(new
        {
            message = $"L'ingrédient avec l'identifiant {id} est introuvable ou déjà inactif."
        });
    }

    return NoContent();
}

[HttpPatch("{id:int:min(1)}/reactiver")]
public async Task<IActionResult> Reactivate(int id)
{
    bool reactive = await _ingredientService.ReactivateAsync(id);

    if (!reactive)
    {
        return NotFound(new
        {
            message = $"L'ingrédient avec l'identifiant {id} est introuvable ou déjà actif."
        });
    }

    return NoContent();
}

[HttpDelete("{id:int:min(1)}/definitif")]
public async Task<IActionResult> DeletePermanently(int id)
{
    try
    {
        bool supprime = await _ingredientService.DeletePermanentlyAsync(id);

        if (!supprime)
        {
            return NotFound(new
            {
                message = $"L'ingrédient avec l'identifiant {id} est introuvable."
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
