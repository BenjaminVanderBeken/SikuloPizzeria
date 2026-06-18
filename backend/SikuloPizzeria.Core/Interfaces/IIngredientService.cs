using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface IIngredientService
{
    Task<IEnumerable<Ingredient>> GetAllAsync();

    Task<Ingredient?> GetByIdAsync(int id);

    Task<Ingredient> CreateAsync(CreateIngredientDto dto);

    Task<Ingredient?> UpdateAsync(
        int id,
        UpdateIngredientDto dto);

    Task<bool> DisableAsync(int id);
    Task<bool> ReactivateAsync(int id);
Task<bool> DeletePermanentlyAsync(int id);

}

