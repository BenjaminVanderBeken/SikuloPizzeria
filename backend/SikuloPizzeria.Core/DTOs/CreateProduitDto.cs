using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class CreateProduitDto
{
    [Range(1, int.MaxValue, ErrorMessage = "La catégorie est obligatoire.")]
    public int CategorieId { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(150, MinimumLength = 2)]
    public string Nom { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(
        0,
        10000,
        ErrorMessage = "Le prix de base doit être positif.")]
    public decimal PrixBase { get; set; }

    [Range(
        0,
        10000,
        ErrorMessage = "Le prix promotionnel doit être positif.")]
    public decimal? PrixPromotion { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public bool PermetSupplement { get; set; }

    public bool Populaire { get; set; }

    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "L'ordre d'affichage doit être positif.")]
    public int OrdreAffichage { get; set; }

    public List<VarianteProduitRequestDto> Variantes { get; set; } = [];
}