using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs.Commandes;

public class UpdateCommandeDto
{
public int? ClientId { get; set; }


public DateTime? DateLivraisonPrevue { get; set; }

[Required]
[RegularExpression("SUR_PLACE|A_EMPORTER|LIVRAISON")]
public string TypeCommande { get; set; } = "A_EMPORTER";

[Required]
[RegularExpression("ESPECES|CARTE|VIREMENT|EN_ATTENTE")]
public string ModePaiement { get; set; } = "EN_ATTENTE";

[StringLength(500)]
public string? NotesClient { get; set; }

[StringLength(500)]
public string? NotesCuisine { get; set; }

[StringLength(500)]
public string? DemandesSpeciales { get; set; }

[Required]
[MinLength(1, ErrorMessage = "La commande doit contenir au moins un produit.")]
public List<CreateDetailCommandeDto> Details { get; set; } = [];


}
