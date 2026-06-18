using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;

namespace SikuloPizzeria.Core.Interfaces;

public interface IClientService
{
Task<IEnumerable<Client>> GetAllAsync();
Task<Client?> GetByIdAsync(int id);
Task<Client> CreateAsync(CreateClientDto dto);
Task<Client?> UpdateAsync(int id, UpdateClientDto dto);
Task<bool> DisableAsync(int id);
Task<bool> ReactivateAsync(int id);
Task<bool> DeletePermanentlyAsync(int id);
}
