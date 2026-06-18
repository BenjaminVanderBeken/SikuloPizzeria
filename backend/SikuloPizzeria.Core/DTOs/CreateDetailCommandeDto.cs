using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class CreateDetailCommandeDto
{
[Range(1, int.MaxValue)]
public int ProduitId { get; set; }


[Range(1, int.MaxValue)]
public int? VarianteId { get; set; }

[Range(1, 100)]
public int Quantite { get; set; } = 1;

public string? NotesParticulieres { get; set; }


}
