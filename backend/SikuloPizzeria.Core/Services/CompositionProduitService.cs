using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.Core.Services;

public sealed class CompositionProduitService : ICompositionProduitService
{
private readonly ICompositionProduitRepository _compositionRepository;
private readonly IProduitRepository _produitRepository;
private readonly IIngredientRepository _ingredientRepository;


public CompositionProduitService(
    ICompositionProduitRepository compositionRepository,
    IProduitRepository produitRepository,
    IIngredientRepository ingredientRepository)
{
    _compositionRepository = compositionRepository;
    _produitRepository = produitRepository;
    _ingredientRepository = ingredientRepository;
}

public Task<IEnumerable<CompositionProduitDto>> GetAllAsync()
{
    return _compositionRepository.GetAllAsync();
}

public Task<IEnumerable<CompositionProduitDto>> GetByProduitIdAsync(
    int produitId)
{
    return _compositionRepository.GetByProduitIdAsync(produitId);
}

public Task<CompositionProduitDto?> GetByIdAsync(int id)
{
    return _compositionRepository.GetByIdAsync(id);
}

public async Task<CompositionProduitDto> CreateAsync(
    CreateCompositionProduitDto dto)
{
    ProduitDto? produit =
        await _produitRepository.GetByIdAsync(dto.ProduitId);

    if (produit is null)
    {
        throw new BusinessRuleException(
            "Le produit sÃ©lectionnÃ© est introuvable."
        );
    }

    if (!produit.Actif)
    {
        throw new BusinessRuleException(
            "Le produit sÃ©lectionnÃ© est inactif."
        );
    }

    Ingredient? ingredient =
        await _ingredientRepository.GetByIdAsync(dto.IngredientId);

    if (ingredient is null)
    {
        throw new BusinessRuleException(
            "L'ingrÃ©dient sÃ©lectionnÃ© est introuvable."
        );
    }

    if (!ingredient.Actif)
    {
        throw new BusinessRuleException(
            "L'ingrÃ©dient sÃ©lectionnÃ© est inactif."
        );
    }

    bool existeDeja = await _compositionRepository.ExistsAsync(
        dto.ProduitId,
        dto.IngredientId
    );

    if (existeDeja)
    {
        throw new BusinessRuleException(
            "Cet ingrÃ©dient est dÃ©jÃ  prÃ©sent dans la composition du produit."
        );
    }

    CompositionProduit composition = new()
    {
        ProduitId = dto.ProduitId,
        IngredientId = dto.IngredientId,
        Quantite = dto.Quantite,
        Unite = NettoyerUnite(dto.Unite),
        OrdreAffichage = dto.OrdreAffichage
    };

    int id = await _compositionRepository.CreateAsync(composition);

    return await _compositionRepository.GetByIdAsync(id)
        ?? throw new InvalidOperationException(
            "La composition a Ã©tÃ© enregistrÃ©e mais n'a pas pu Ãªtre relue."
        );
}

public async Task<bool> DeleteAsync(int id)
{
    CompositionProduitDto? composition =
        await _compositionRepository.GetByIdAsync(id);

    if (composition is null)
    {
        return false;
    }

    return await _compositionRepository.DeleteAsync(id);
}

private static string NettoyerUnite(string unite)
{
    string texte = unite.Trim();

    if (texte.Length == 0)
    {
        throw new BusinessRuleException(
            "L'unitÃ© est obligatoire."
        );
    }

    return texte;
}


}

