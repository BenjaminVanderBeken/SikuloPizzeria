using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class CreateCompositionProduitDto
{
[Required]
[Range(1, int.MaxValue, ErrorMessage = "Le produit est obligatoire.")]
public int ProduitId { get; set; }


[Required]
[Range(1, int.MaxValue, ErrorMessage = "L'ingrédient est obligatoire.")]
public int IngredientId { get; set; }

[Required]
[Range(0.01, 999999, ErrorMessage = "La quantité doit être supérieure à zéro.")]
public decimal Quantite { get; set; }

[Required]
[StringLength(20)]
public string Unite { get; set; } = string.Empty;

[Range(0, int.MaxValue)]
public int OrdreAffichage { get; set; }


}
