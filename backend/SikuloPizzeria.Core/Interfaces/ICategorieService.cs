using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface ICategorieService
{
    Task<IEnumerable<Categorie>> GetAllAsync();

    Task<Categorie?> GetByIdAsync(int id);

    Task<Categorie> CreateAsync(CreateCategorieDto dto);

    Task<Categorie?> UpdateAsync(int id, UpdateCategorieDto dto);

    Task<bool> DisableAsync(int id);
    Task<bool> ReactivateAsync(int id);
Task<bool> DeletePermanentlyAsync(int id);

}