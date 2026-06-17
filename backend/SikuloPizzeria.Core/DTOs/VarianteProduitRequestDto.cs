using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class VarianteProduitRequestDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Le nom de la variante est obligatoire.")]
    [StringLength(50, MinimumLength = 1)]
    public string Nom { get; set; } = string.Empty;

    [Range(
        0,
        10000,
        ErrorMessage = "Le prix doit être positif.")]
    public decimal Prix { get; set; }

    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "L'ordre d'affichage doit être positif.")]
    public int OrdreAffichage { get; set; }

    public bool Actif { get; set; } = true;
}