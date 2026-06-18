using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface ICategorieRepository
{
Task<IEnumerable<Categorie>> GetAllAsync();
Task<Categorie?> GetByIdAsync(int id);
Task<int> CreateAsync(Categorie categorie);
Task<bool> UpdateAsync(Categorie categorie);
Task<bool> DeleteAsync(int id);
Task<bool> ReactivateAsync(int id);
Task<bool> HasProductsAsync(int id);
Task<bool> DeletePermanentlyAsync(int id);
Task<bool> ExistsByNameAsync(string nom, int? excludedId = null);
}
