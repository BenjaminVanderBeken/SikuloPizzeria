using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface ICompositionProduitRepository
{
Task<IEnumerable<CompositionProduitDto>> GetAllAsync();
Task<IEnumerable<CompositionProduitDto>> GetByProduitIdAsync(int produitId);
Task<CompositionProduitDto?> GetByIdAsync(int id);
Task<int> CreateAsync(CompositionProduit composition);
Task<bool> DeleteAsync(int id);
Task<bool> ExistsAsync(int produitId, int ingredientId);
}

