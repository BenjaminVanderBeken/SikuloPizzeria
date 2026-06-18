using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface IIngredientRepository
{
Task<IEnumerable<Ingredient>> GetAllAsync();


Task<Ingredient?> GetByIdAsync(int id);

Task<int> CreateAsync(Ingredient ingredient);

Task<bool> UpdateAsync(Ingredient ingredient);

Task<bool> DisableAsync(int id);

Task<bool> ExistsByNameAsync(
    string nom,
    int? excludedId = null);
//ingredients
Task<bool> ReactivateAsync(int id);
Task<bool> IsUsedInCompositionAsync(int id);
Task<bool> DeletePermanentlyAsync(int id);

}
