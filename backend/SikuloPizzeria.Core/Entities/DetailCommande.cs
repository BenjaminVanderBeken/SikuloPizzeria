namespace SikuloPizzeria.Core.Entities;

public sealed class DetailCommande
{
public int Id { get; set; }
public int CommandeId { get; set; }
public int ProduitId { get; set; }
public int? VarianteId { get; set; }
public int Quantite { get; set; }
public decimal PrixUnitaire { get; set; }
public decimal PrixTotal { get; set; }
public string? NomVariante { get; set; }
public string? NotesParticulieres { get; set; }
public decimal CoutSupplements { get; set; }
}
