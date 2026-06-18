namespace SikuloPizzeria.Core.Entities;

public sealed class CompositionProduit
{
public int Id { get; set; }
public int ProduitId { get; set; }
public int IngredientId { get; set; }
public decimal Quantite { get; set; }
public string Unite { get; set; } = string.Empty;
public int OrdreAffichage { get; set; }
}
