using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.Core.Services;

public sealed class CategorieService : ICategorieService
{
    private readonly ICategorieRepository _categorieRepository;

    public CategorieService(ICategorieRepository categorieRepository)
    {
        _categorieRepository = categorieRepository;
    }

    public Task<IEnumerable<Categorie>> GetAllAsync()
    {
        return _categorieRepository.GetAllAsync();
    }

    public Task<Categorie?> GetByIdAsync(int id)
    {
        return _categorieRepository.GetByIdAsync(id);
    }

    public async Task<Categorie> CreateAsync(CreateCategorieDto dto)
    {
        string nomNormalise = dto.Nom.Trim();

        bool nomExiste = await _categorieRepository.ExistsByNameAsync(
            nomNormalise);

        if (nomExiste)
        {
            throw new DuplicateResourceException(
                $"Une catégorie portant le nom '{nomNormalise}' existe déjà.");
        }

        var categorie = new Categorie
        {
            Nom = nomNormalise,
            Description = NettoyerTexteOptionnel(dto.Description),
            OrdreAffichage = dto.OrdreAffichage,
            Actif = true
        };

        categorie.Id = await _categorieRepository.CreateAsync(categorie);

        return categorie;
    }

    public async Task<Categorie?> UpdateAsync(
        int id,
        UpdateCategorieDto dto)
    {
        Categorie? categorieExistante =
            await _categorieRepository.GetByIdAsync(id);

        if (categorieExistante is null)
        {
            return null;
        }

        string nomNormalise = dto.Nom.Trim();

        bool nomExiste = await _categorieRepository.ExistsByNameAsync(
            nomNormalise,
            id);

        if (nomExiste)
        {
            throw new DuplicateResourceException(
                $"Une autre catégorie portant le nom '{nomNormalise}' existe déjà.");
        }

        categorieExistante.Nom = nomNormalise;
        categorieExistante.Description =
            NettoyerTexteOptionnel(dto.Description);
        categorieExistante.OrdreAffichage = dto.OrdreAffichage;
        categorieExistante.Actif = dto.Actif;

        await _categorieRepository.UpdateAsync(categorieExistante);

        return categorieExistante;
    }

    public Task<bool> DisableAsync(int id)
    {
        return _categorieRepository.DeleteAsync(id);
    }

    private static string? NettoyerTexteOptionnel(string? valeur)
    {
        return string.IsNullOrWhiteSpace(valeur)
            ? null
            : valeur.Trim();
    }
    public Task<bool> ReactivateAsync(int id)
{
return _categorieRepository.ReactivateAsync(id);
}

public async Task<bool> DeletePermanentlyAsync(int id)
{
Categorie? categorie = await _categorieRepository.GetByIdAsync(id);


if (categorie is null)
{
    return false;
}

bool contientProduits = await _categorieRepository.HasProductsAsync(id);

if (contientProduits)
{
    throw new BusinessRuleException(
        "Cette categorie ne peut pas etre supprimee car elle contient des produits."
    );
}

return await _categorieRepository.DeletePermanentlyAsync(id);


}
}