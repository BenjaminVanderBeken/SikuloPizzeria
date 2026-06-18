using SikuloPizzeria.Core.DTOs;

namespace SikuloPizzeria.Core.Interfaces;

public interface IProduitService
{
Task<IEnumerable<ProduitDto>> GetAllAsync();


Task<ProduitDto?> GetByIdAsync(int id);

Task<ProduitDto> CreateAsync(CreateProduitDto dto);

Task<ProduitDto?> UpdateAsync(
    int id,
    UpdateProduitDto dto);

Task<bool> DisableAsync(int id);

Task<bool> ReactivateAsync(int id);

Task<bool> DeleteAsync(int id);

}
