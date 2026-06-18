using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class UpdateCommandePaymentDto
{
[Required]
[RegularExpression(
"^(ESPECES|CARTE|VIREMENT|EN_ATTENTE)$",
ErrorMessage = "Le mode de paiement est invalide."
)]
public string ModePaiement { get; set; } = "EN_ATTENTE";


[Required]
[RegularExpression(
    "^(NON_PAYEE|PAYEE|REMBOURSEE)$",
    ErrorMessage = "Le statut de paiement est invalide."
)]
public string StatutPaiement { get; set; } = "NON_PAYEE";


}
