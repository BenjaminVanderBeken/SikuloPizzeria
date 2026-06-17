namespace SikuloPizzeria.Core.Entities;

public sealed class VarianteProduit
{
    public int Id { get; set; }

    public int ProduitId { get; set; }

    public string Nom { get; set; } = string.Empty;

    public decimal Prix { get; set; }

    public bool Actif { get; set; }

    public int OrdreAffichage { get; set; }
}