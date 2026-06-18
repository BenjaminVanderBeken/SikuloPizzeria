namespace SikuloPizzeria.Core.Entities;

public sealed class Client
{
public int Id { get; set; }
public string Nom { get; set; } = string.Empty;
public string? Prenom { get; set; }
public string? Email { get; set; }
public string? Telephone { get; set; }
public string? AdresseRue { get; set; }
public string? AdresseNumero { get; set; }
public string? AdresseBoite { get; set; }
public string? AdresseCodePostal { get; set; }
public string? AdresseVille { get; set; }
public string AdressePays { get; set; } = "Belgique";
public string? Notes { get; set; }
public bool ClientVip { get; set; }
public bool Actif { get; set; } = true;
public DateTime DateCreation { get; set; }
public DateTime DateModification { get; set; }
}
//Cette entité représente les colonnes de la table clients, notamment l’adresse, le statut VIP et l’activation logique.