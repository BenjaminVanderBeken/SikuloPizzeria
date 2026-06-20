using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface ICommandeRepository
{
    Task<IEnumerable<CommandeDto>> GetAllAsync();

    Task<CommandeDto?> GetByIdAsync(int id);

    Task<int> CreateAsync(
        Commande commande,
        IReadOnlyCollection<DetailCommande> details);

    Task<bool> UpdateStatusAsync(int id, string statut);

    Task<bool> UpdatePaymentAsync(
        int id,
        string modePaiement,
        string statutPaiement);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsByNumberAsync(string numeroCommande);

    Task<bool> UpdateAsync(
        Commande commande,
        IReadOnlyCollection<DetailCommande> details);
}
