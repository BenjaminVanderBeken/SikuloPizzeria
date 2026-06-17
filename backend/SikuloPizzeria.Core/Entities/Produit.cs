namespace SikuloPizzeria.Core.Entities;

public sealed class Produit
{
    public int Id { get; set; }

    public int CategorieId { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal PrixBase { get; set; }

    public decimal? PrixPromotion { get; set; }

    public string? ImageUrl { get; set; }

    public bool PermetSupplement { get; set; }

    public bool Actif { get; set; }

    public bool Populaire { get; set; }

    public int OrdreAffichage { get; set; }
}