namespace SikuloPizzeria.Core.Entities;

public sealed class Commande
{
public int Id { get; set; }
public string NumeroCommande { get; set; } = string.Empty;
public int? ClientId { get; set; }
public int? PromotionId { get; set; }
public DateTime DateCommande { get; set; }
public DateTime? DateLivraisonPrevue { get; set; }
public DateTime? DateLivraisonReelle { get; set; }
public string TypeCommande { get; set; } = "A_EMPORTER";
public string Statut { get; set; } = "EN_ATTENTE";
public decimal SousTotal { get; set; }
public decimal TvaMontant { get; set; }
public decimal FraisLivraison { get; set; }
public decimal ReductionMontant { get; set; }
public decimal MontantTotal { get; set; }
public string ModePaiement { get; set; } = "EN_ATTENTE";
public string StatutPaiement { get; set; } = "NON_PAYEE";
public string? NotesClient { get; set; }
public string? NotesCuisine { get; set; }
public string? DemandesSpeciales { get; set; }
public DateTime DateModification { get; set; }
}
