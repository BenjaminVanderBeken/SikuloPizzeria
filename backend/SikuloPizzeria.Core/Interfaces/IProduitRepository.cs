using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface IProduitRepository
{
    Task<IEnumerable<ProduitDto>> GetAllAsync();

    Task<ProduitDto?> GetByIdAsync(int id);

    Task<int> CreateAsync(
        Produit produit,
        IReadOnlyCollection<VarianteProduit> variantes);

    Task<bool> UpdateAsync(
        Produit produit,
        IReadOnlyCollection<VarianteProduit> variantes);

    Task<bool> DisableAsync(int id);

    Task<bool> ExistsByNameAsync(
        int categorieId,
        string nom,
        int? excludedId = null);
}