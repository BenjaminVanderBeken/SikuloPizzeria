using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class CreateIngredientDto
{
[Required(ErrorMessage = "Le nom est obligatoire.")]
[MaxLength(100, ErrorMessage = "Le nom ne peut pas depasser 100 caracteres.")]
public string Nom { get; set; } = string.Empty;


[Required(ErrorMessage = "Le type est obligatoire.")]
public string Type { get; set; } = string.Empty;

[Range(0, double.MaxValue, ErrorMessage = "Le stock ne peut pas etre negatif.")]
public decimal StockActuel { get; set; }

[Required(ErrorMessage = "L'unite de mesure est obligatoire.")]
[MaxLength(30)]
public string UniteMesure { get; set; } = string.Empty;

[Range(0, double.MaxValue, ErrorMessage = "Le prix unitaire ne peut pas etre negatif.")]
public decimal? PrixUnitaire { get; set; }

[MaxLength(255)]
public string? Allergenes { get; set; }

}
