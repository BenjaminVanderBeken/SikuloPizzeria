using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.Core.Services;

public sealed class ProduitService : IProduitService
{
    private readonly IProduitRepository _produitRepository;
    private readonly ICategorieRepository _categorieRepository;

    public ProduitService(
        IProduitRepository produitRepository,
        ICategorieRepository categorieRepository)
    {
        _produitRepository = produitRepository;
        _categorieRepository = categorieRepository;
    }

    public Task<IEnumerable<ProduitDto>> GetAllAsync()
    {
        return _produitRepository.GetAllAsync();
    }

    public Task<ProduitDto?> GetByIdAsync(int id)
    {
        return _produitRepository.GetByIdAsync(id);
    }

    public async Task<ProduitDto> CreateAsync(CreateProduitDto dto)
    {
        await VerifierCategorieAsync(dto.CategorieId);
        VerifierPrix(dto.PrixBase, dto.PrixPromotion);
        VerifierVariantes(dto.Variantes);

        string nomNormalise = dto.Nom.Trim();

        bool existe = await _produitRepository.ExistsByNameAsync(
            dto.CategorieId,
            nomNormalise);

        if (existe)
        {
            throw new DuplicateResourceException(
                $"Le produit '{nomNormalise}' existe déjà dans cette catégorie.");
        }

        var produit = new Produit
        {
            CategorieId = dto.CategorieId,
            Nom = nomNormalise,
            Description = NettoyerTexte(dto.Description),
            PrixBase = dto.PrixBase,
            PrixPromotion = dto.PrixPromotion,
            ImageUrl = NettoyerTexte(dto.ImageUrl),
            PermetSupplement = dto.PermetSupplement,
            Actif = true,
            Populaire = dto.Populaire,
            OrdreAffichage = dto.OrdreAffichage
        };

        List<VarianteProduit> variantes =
            CreerVariantes(dto.Variantes);

        produit.Id = await _produitRepository.CreateAsync(
            produit,
            variantes);

        ProduitDto? produitCree =
            await _produitRepository.GetByIdAsync(produit.Id);

        return produitCree
            ?? throw new InvalidOperationException(
                "Le produit a été créé, mais il n'a pas pu être relu.");
    }

    public async Task<ProduitDto?> UpdateAsync(
        int id,
        UpdateProduitDto dto)
    {
        ProduitDto? produitExistant =
            await _produitRepository.GetByIdAsync(id);

        if (produitExistant is null)
        {
            return null;
        }

        await VerifierCategorieAsync(dto.CategorieId);
        VerifierPrix(dto.PrixBase, dto.PrixPromotion);
        VerifierVariantes(dto.Variantes);

        string nomNormalise = dto.Nom.Trim();

        bool existe = await _produitRepository.ExistsByNameAsync(
            dto.CategorieId,
            nomNormalise,
            id);

        if (existe)
        {
            throw new DuplicateResourceException(
                $"Un autre produit nommé '{nomNormalise}' existe déjà dans cette catégorie.");
        }

        var produit = new Produit
        {
            Id = id,
            CategorieId = dto.CategorieId,
            Nom = nomNormalise,
            Description = NettoyerTexte(dto.Description),
            PrixBase = dto.PrixBase,
            PrixPromotion = dto.PrixPromotion,
            ImageUrl = NettoyerTexte(dto.ImageUrl),
            PermetSupplement = dto.PermetSupplement,
            Actif = dto.Actif,
            Populaire = dto.Populaire,
            OrdreAffichage = dto.OrdreAffichage
        };

        List<VarianteProduit> variantes =
            CreerVariantes(dto.Variantes);

        bool modifie = await _produitRepository.UpdateAsync(
            produit,
            variantes);

        if (!modifie)
        {
            return null;
        }

        return await _produitRepository.GetByIdAsync(id);
    }

    public Task<bool> DisableAsync(int id)
    {
        return _produitRepository.DisableAsync(id);
    }

public Task<bool> ReactivateAsync(int id)
{
return _produitRepository.ReactivateAsync(id);
}

    private async Task VerifierCategorieAsync(int categorieId)
    {
        Categorie? categorie =
            await _categorieRepository.GetByIdAsync(categorieId);

        if (categorie is null)
        {
            throw new BusinessRuleException(
                "La catégorie sélectionnée est introuvable.");
        }

        if (!categorie.Actif)
        {
            throw new BusinessRuleException(
                "Il est impossible d'utiliser une catégorie inactive.");
        }
    }
public async Task<bool> DeleteAsync(int id)
{
ProduitDto? produit =
await _produitRepository.GetByIdAsync(id);


if (produit is null)
{
    return false;
}

bool utiliseDansCommande =
    await _produitRepository.IsUsedInOrderAsync(id);

if (utiliseDansCommande)
{
    throw new BusinessRuleException(
        "Ce produit ne peut pas etre supprime car il est utilise dans une commande.");
}

return await _produitRepository.DeleteAsync(id);


}

    private static void VerifierPrix(
        decimal prixBase,
        decimal? prixPromotion)
    {
        if (prixPromotion.HasValue &&
            prixPromotion.Value > prixBase)
        {
            throw new BusinessRuleException(
                "Le prix promotionnel ne peut pas dépasser le prix de base.");
        }
    }

    private static void VerifierVariantes(
        IEnumerable<VarianteProduitRequestDto> variantes)
    {
        List<string> noms = variantes
            .Select(variante => variante.Nom.Trim())
            .Where(nom => !string.IsNullOrWhiteSpace(nom))
            .ToList();

        bool doublon = noms
            .GroupBy(
                nom => nom,
                StringComparer.OrdinalIgnoreCase)
            .Any(groupe => groupe.Count() > 1);

        if (doublon)
        {
            throw new BusinessRuleException(
                "Deux variantes d'un même produit ne peuvent pas porter le même nom.");
        }
    }

    private static List<VarianteProduit> CreerVariantes(
        IEnumerable<VarianteProduitRequestDto> variantes)
    {
        return variantes
            .Select(variante => new VarianteProduit
            {
                Id = variante.Id ?? 0,
                Nom = variante.Nom.Trim(),
                Prix = variante.Prix,
                Actif = variante.Actif,
                OrdreAffichage = variante.OrdreAffichage
            })
            .ToList();
    }

    private static string? NettoyerTexte(string? valeur)
    {
        return string.IsNullOrWhiteSpace(valeur)
            ? null
            : valeur.Trim();
    }
}