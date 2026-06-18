using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class CreateClientDto
{
[Required]
[MaxLength(100)]
public string Nom { get; set; } = string.Empty;


[MaxLength(100)]
public string? Prenom { get; set; }

[EmailAddress]
[MaxLength(150)]
public string? Email { get; set; }

[MaxLength(25)]
public string? Telephone { get; set; }

[MaxLength(200)]
public string? AdresseRue { get; set; }

[MaxLength(20)]
public string? AdresseNumero { get; set; }

[MaxLength(10)]
public string? AdresseBoite { get; set; }

[MaxLength(10)]
public string? AdresseCodePostal { get; set; }

[MaxLength(100)]
public string? AdresseVille { get; set; }

[Required]
[MaxLength(50)]
public string AdressePays { get; set; } = "Belgique";

public string? Notes { get; set; }

public bool ClientVip { get; set; }


}
