namespace SikuloPizzeria.Core.DTOs;

public sealed class CommandeDto
{
public int Id { get; set; }
public string NumeroCommande { get; set; } = string.Empty;
public int? ClientId { get; set; }
public string ClientNomComplet { get; set; } = "Vente comptoir";
public DateTime DateCommande { get; set; }
public DateTime? DateLivraisonPrevue { get; set; }
public DateTime? DateLivraisonReelle { get; set; }
public string TypeCommande { get; set; } = string.Empty;
public string Statut { get; set; } = string.Empty;
public decimal SousTotal { get; set; }
public decimal TvaMontant { get; set; }
public decimal FraisLivraison { get; set; }
public decimal ReductionMontant { get; set; }
public decimal MontantTotal { get; set; }
public string ModePaiement { get; set; } = string.Empty;
public string StatutPaiement { get; set; } = string.Empty;
public string? NotesClient { get; set; }
public string? NotesCuisine { get; set; }
public string? DemandesSpeciales { get; set; }
public List<DetailCommandeDto> Details { get; set; } = [];
}
//Ce DTO sera rempli grâce à des jointures entre commandes, clients, details_commandes et produits