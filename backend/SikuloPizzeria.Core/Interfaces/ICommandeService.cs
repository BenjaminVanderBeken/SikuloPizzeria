using SikuloPizzeria.Core.DTOs;

namespace SikuloPizzeria.Core.Interfaces;

public interface ICommandeService
{
Task<IEnumerable<CommandeDto>> GetAllAsync();
Task<CommandeDto?> GetByIdAsync(int id);
Task<CommandeDto> CreateAsync(CreateCommandeDto dto);
Task<bool> UpdateStatusAsync(int id, UpdateCommandeStatusDto dto);
Task<bool> UpdatePaymentAsync(int id, UpdateCommandePaymentDto dto);
}
//On crée maintenant l’interface du service métier des commandes.