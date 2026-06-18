using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface ICommandeRepository
{
Task<IEnumerable<CommandeDto>> GetAllAsync();
Task<CommandeDto?> GetByIdAsync(int id);


Task<int> CreateAsync(
    Commande commande,
    IReadOnlyCollection<DetailCommande> details
);

Task<bool> UpdateStatusAsync(int id, string statut);

Task<bool> UpdatePaymentAsync(
    int id,
    string modePaiement,
    string statutPaiement
);

Task<bool> ExistsByNumberAsync(string numeroCommande);

Task<bool> UpdateAsync(
    Commande commande,
    IReadOnlyCollection<DetailCommande> details
);


}
//La méthode CreateAsync ouvrira une transaction MySQL, enregistrera d’abord la commande, puis toutes ses lignes dans details_commandes. En cas d’erreur, elle exécutera un Rollback.