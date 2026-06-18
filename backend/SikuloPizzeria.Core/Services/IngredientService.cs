using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.Core.Services;

public sealed class IngredientService : IIngredientService
{
    private static readonly string[] TypesAutorises =
    [
        "SAUCE",
        "VIANDE",
        "FROMAGE",
        "LEGUME",
        "ACCOMPAGNEMENT",
        "AUTRE"
    ];

    private readonly IIngredientRepository _ingredientRepository;

    public IngredientService(
        IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public Task<IEnumerable<Ingredient>> GetAllAsync()
    {
        return _ingredientRepository.GetAllAsync();
    }

    public Task<Ingredient?> GetByIdAsync(int id)
    {
        return _ingredientRepository.GetByIdAsync(id);
    }

    public async Task<Ingredient> CreateAsync(
        CreateIngredientDto dto)
    {
        string nom = dto.Nom.Trim();
        string type = NormaliserType(dto.Type);

        await VerifierNomUniqueAsync(nom);
        VerifierType(type);

        var ingredient = new Ingredient
        {
            Nom = nom,
            Type = type,
            StockActuel = dto.StockActuel,
            UniteMesure = dto.UniteMesure.Trim(),
            PrixUnitaire = dto.PrixUnitaire,
            Allergenes = NettoyerTexte(dto.Allergenes),
            Actif = true
        };

        ingredient.Id =
            await _ingredientRepository.CreateAsync(ingredient);

        return ingredient;
    }

    public async Task<Ingredient?> UpdateAsync(
        int id,
        UpdateIngredientDto dto)
    {
        Ingredient? ingredientExistant =
            await _ingredientRepository.GetByIdAsync(id);

        if (ingredientExistant is null)
        {
            return null;
        }

        string nom = dto.Nom.Trim();
        string type = NormaliserType(dto.Type);

        await VerifierNomUniqueAsync(nom, id);
        VerifierType(type);

        var ingredient = new Ingredient
        {
            Id = id,
            Nom = nom,
            Type = type,
            StockActuel = dto.StockActuel,
            UniteMesure = dto.UniteMesure.Trim(),
            PrixUnitaire = dto.PrixUnitaire,
            Allergenes = NettoyerTexte(dto.Allergenes),
            Actif = dto.Actif
        };

        bool modifie =
            await _ingredientRepository.UpdateAsync(ingredient);

        return modifie ? ingredient : null;
    }

    public Task<bool> DisableAsync(int id)
    {
        return _ingredientRepository.DisableAsync(id);
    }

    private async Task VerifierNomUniqueAsync(
        string nom,
        int? excludedId = null)
    {
        bool existe =
            await _ingredientRepository.ExistsByNameAsync(
                nom,
                excludedId);

        if (existe)
        {
            throw new DuplicateResourceException(
                $"Un ingredient nomme '{nom}' existe deja.");
        }
    }

    private static void VerifierType(string type)
    {
        if (!TypesAutorises.Contains(type))
        {
            throw new BusinessRuleException(
                "Le type d'ingredient est invalide.");
        }
    }

    private static string NormaliserType(string type)
    {
        return type.Trim().ToUpperInvariant();
    }

    private static string? NettoyerTexte(string? valeur)
    {
        return string.IsNullOrWhiteSpace(valeur)
            ? null
            : valeur.Trim();
    }
    public Task<bool> ReactivateAsync(int id)
{
return _ingredientRepository.ReactivateAsync(id);
}

public async Task<bool> DeletePermanentlyAsync(int id)
{
Ingredient? ingredient = await _ingredientRepository.GetByIdAsync(id);


if (ingredient is null)
{
    return false;
}

bool utiliseDansComposition =
    await _ingredientRepository.IsUsedInCompositionAsync(id);

if (utiliseDansComposition)
{
    throw new BusinessRuleException(
        "Cet ingrédient ne peut pas être supprimé car il est utilisé dans la composition d'un produit."
    );
}

return await _ingredientRepository.DeletePermanentlyAsync(id);


}

}