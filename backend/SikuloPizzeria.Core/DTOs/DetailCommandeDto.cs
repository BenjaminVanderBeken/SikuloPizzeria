namespace SikuloPizzeria.Core.DTOs;

public sealed class DetailCommandeDto
{
public int Id { get; set; }
public int ProduitId { get; set; }
public string ProduitNom { get; set; } = string.Empty;
public int? VarianteId { get; set; }
public string? NomVariante { get; set; }
public int Quantite { get; set; }
public decimal PrixUnitaire { get; set; }
public decimal PrixTotal { get; set; }
public string? NotesParticulieres { get; set; }
public decimal CoutSupplements { get; set; }
}
