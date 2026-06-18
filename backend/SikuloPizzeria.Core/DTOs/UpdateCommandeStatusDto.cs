using System.ComponentModel.DataAnnotations;

namespace SikuloPizzeria.Core.DTOs;

public sealed class UpdateCommandeStatusDto
{
[Required]
[RegularExpression(
"^(EN_ATTENTE|CONFIRMEE|EN_PREPARATION|PRETE|LIVREE|TERMINEE|ANNULEE)$",
ErrorMessage = "Le statut de la commande est invalide."
)]
public string Statut { get; set; } = string.Empty;
}
//Les statuts acceptés correspondent exactement à la contrainte MySQL de la table commandes