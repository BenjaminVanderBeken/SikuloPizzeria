using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface IClientRepository
{
Task<IEnumerable<Client>> GetAllAsync();
Task<Client?> GetByIdAsync(int id);
Task<int> CreateAsync(Client client);
Task<bool> UpdateAsync(Client client);
Task<bool> DisableAsync(int id);
Task<bool> ReactivateAsync(int id);
Task<bool> HasOrdersAsync(int id);
Task<bool> DeletePermanentlyAsync(int id);
Task<bool> ExistsByEmailAsync(string email, int? excludedId = null);
}
//HasOrdersAsync empêchera de supprimer définitivement un client déjà associé à une commande, afin de conserver l’historique commercial