using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class CreateCommandeDto
{
[Range(1, int.MaxValue)]
public int? ClientId { get; set; }


public DateTime? DateLivraisonPrevue { get; set; }

[Required]
[RegularExpression(
    "^(SUR_PLACE|A_EMPORTER|LIVRAISON)$",
    ErrorMessage = "Le type de commande est invalide."
)]
public string TypeCommande { get; set; } = "A_EMPORTER";

[Required]
[RegularExpression(
    "^(ESPECES|CARTE|VIREMENT|EN_ATTENTE)$",
    ErrorMessage = "Le mode de paiement est invalide."
)]
public string ModePaiement { get; set; } = "EN_ATTENTE";

public string? NotesClient { get; set; }

public string? NotesCuisine { get; set; }

public string? DemandesSpeciales { get; set; }

[Required]
[MinLength(1, ErrorMessage = "La commande doit contenir au moins un produit.")]
public List<CreateDetailCommandeDto> Details { get; set; } = [];


}

