namespace SikuloPizzeria.Core.Entities;

public sealed class Ingredient
{
public int Id { get; set; }

public string Nom { get; set; } = string.Empty;

public string Type { get; set; } = string.Empty;

public decimal StockActuel { get; set; }

public string UniteMesure { get; set; } = string.Empty;

public decimal? PrixUnitaire { get; set; }

public string? Allergenes { get; set; }

public bool Actif { get; set; }

}
