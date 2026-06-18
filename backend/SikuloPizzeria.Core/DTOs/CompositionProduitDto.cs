namespace SikuloPizzeria.Core.DTOs;

public sealed class CompositionProduitDto
{
public int Id { get; set; }
public int ProduitId { get; set; }
public string ProduitNom { get; set; } = string.Empty;
public int IngredientId { get; set; }
public string IngredientNom { get; set; } = string.Empty;
public decimal Quantite { get; set; }
public string Unite { get; set; } = string.Empty;
public int OrdreAffichage { get; set; }
}
