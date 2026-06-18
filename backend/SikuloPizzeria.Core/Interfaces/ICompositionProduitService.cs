using SikuloPizzeria.Core.DTOs;

namespace SikuloPizzeria.Core.Interfaces;

public interface ICompositionProduitService
{
Task<IEnumerable<CompositionProduitDto>> GetAllAsync();
Task<IEnumerable<CompositionProduitDto>> GetByProduitIdAsync(int produitId);
Task<CompositionProduitDto?> GetByIdAsync(int id);
Task<CompositionProduitDto> CreateAsync(CreateCompositionProduitDto dto);
Task<bool> DeleteAsync(int id);
}
