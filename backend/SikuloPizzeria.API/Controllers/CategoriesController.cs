using Microsoft.AspNetCore.Mvc;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.API.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ICategorieService _categorieService;

    public CategoriesController(ICategorieService categorieService)
    {
        _categorieService = categorieService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<Categorie>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Categorie>>> GetAll()
    {
        IEnumerable<Categorie> categories =
            await _categorieService.GetAllAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(
        typeof(Categorie),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Categorie>> GetById(int id)
    {
        Categorie? categorie =
            await _categorieService.GetByIdAsync(id);

        if (categorie is null)
        {
            return NotFound(new
            {
                message = $"La catégorie avec l'identifiant {id} est introuvable."
            });
        }

        return Ok(categorie);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(Categorie),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Categorie>> Create(
        [FromBody] CreateCategorieDto dto)
    {
        try
        {
            Categorie categorie =
                await _categorieService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = categorie.Id },
                categorie);
        }
        catch (DuplicateResourceException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(
        typeof(Categorie),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Categorie>> Update(
        int id,
        [FromBody] UpdateCategorieDto dto)
    {
        try
        {
            Categorie? categorie =
                await _categorieService.UpdateAsync(id, dto);

            if (categorie is null)
            {
                return NotFound(new
                {
                    message =
                        $"La catégorie avec l'identifiant {id} est introuvable."
                });
            }

            return Ok(categorie);
        }
        catch (DuplicateResourceException exception)
        {
            return Conflict(new
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
        bool categorieDesactivee =
            await _categorieService.DisableAsync(id);

        if (!categorieDesactivee)
        {
            return NotFound(new
            {
                message =
                    $"La catégorie avec l'identifiant {id} est introuvable ou déjà inactive."
            });
        }

        return NoContent();
    }
}