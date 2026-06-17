using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class UpdateCategorieDto
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(100, MinimumLength = 2)]
    public string Nom { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "L'ordre d'affichage doit être positif.")]
    public int OrdreAffichage { get; set; }

    public bool Actif { get; set; }
}