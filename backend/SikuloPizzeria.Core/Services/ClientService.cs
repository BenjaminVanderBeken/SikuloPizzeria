using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.Core.Services;

public sealed class ClientService : IClientService
{
private readonly IClientRepository _clientRepository;


public ClientService(IClientRepository clientRepository)
{
    _clientRepository = clientRepository;
}

public Task<IEnumerable<Client>> GetAllAsync()
{
    return _clientRepository.GetAllAsync();
}

public Task<Client?> GetByIdAsync(int id)
{
    return _clientRepository.GetByIdAsync(id);
}

public async Task<Client> CreateAsync(CreateClientDto dto)
{
    string nom = dto.Nom.Trim();
    string? email = NettoyerTexte(dto.Email);

    if (string.IsNullOrWhiteSpace(nom))
    {
        throw new BusinessRuleException("Le nom du client est obligatoire.");
    }

    if (email is not null && await _clientRepository.ExistsByEmailAsync(email))
    {
        throw new DuplicateResourceException(
            $"Un client utilise déjà l'adresse e-mail {email}."
        );
    }

    Client client = new()
    {
        Nom = nom,
        Prenom = NettoyerTexte(dto.Prenom),
        Email = email,
        Telephone = NettoyerTexte(dto.Telephone),
        AdresseRue = NettoyerTexte(dto.AdresseRue),
        AdresseNumero = NettoyerTexte(dto.AdresseNumero),
        AdresseBoite = NettoyerTexte(dto.AdresseBoite),
        AdresseCodePostal = NettoyerTexte(dto.AdresseCodePostal),
        AdresseVille = NettoyerTexte(dto.AdresseVille),
        AdressePays = NettoyerTexte(dto.AdressePays) ?? "Belgique",
        Notes = NettoyerTexte(dto.Notes),
        ClientVip = dto.ClientVip,
        Actif = true
    };

    client.Id = await _clientRepository.CreateAsync(client);

    return await _clientRepository.GetByIdAsync(client.Id) ?? client;
}

public async Task<Client?> UpdateAsync(int id, UpdateClientDto dto)
{
    Client? client = await _clientRepository.GetByIdAsync(id);

    if (client is null)
    {
        return null;
    }

    string nom = dto.Nom.Trim();
    string? email = NettoyerTexte(dto.Email);

    if (string.IsNullOrWhiteSpace(nom))
    {
        throw new BusinessRuleException("Le nom du client est obligatoire.");
    }

    if (
        email is not null &&
        await _clientRepository.ExistsByEmailAsync(email, id)
    )
    {
        throw new DuplicateResourceException(
            $"Un autre client utilise déjà l'adresse e-mail {email}."
        );
    }

    client.Nom = nom;
    client.Prenom = NettoyerTexte(dto.Prenom);
    client.Email = email;
    client.Telephone = NettoyerTexte(dto.Telephone);
    client.AdresseRue = NettoyerTexte(dto.AdresseRue);
    client.AdresseNumero = NettoyerTexte(dto.AdresseNumero);
    client.AdresseBoite = NettoyerTexte(dto.AdresseBoite);
    client.AdresseCodePostal = NettoyerTexte(dto.AdresseCodePostal);
    client.AdresseVille = NettoyerTexte(dto.AdresseVille);
    client.AdressePays = NettoyerTexte(dto.AdressePays) ?? "Belgique";
    client.Notes = NettoyerTexte(dto.Notes);
    client.ClientVip = dto.ClientVip;
    client.Actif = dto.Actif;

    bool modifie = await _clientRepository.UpdateAsync(client);

    if (!modifie)
    {
        return null;
    }

    return await _clientRepository.GetByIdAsync(id);
}

public Task<bool> DisableAsync(int id)
{
    return _clientRepository.DisableAsync(id);
}

public Task<bool> ReactivateAsync(int id)
{
    return _clientRepository.ReactivateAsync(id);
}

public async Task<bool> DeletePermanentlyAsync(int id)
{
    Client? client = await _clientRepository.GetByIdAsync(id);

    if (client is null)
    {
        return false;
    }

    bool possedeCommandes = await _clientRepository.HasOrdersAsync(id);

    if (possedeCommandes)
    {
        throw new BusinessRuleException(
            "Ce client ne peut pas être supprimé car il est associé à une commande."
        );
    }

    return await _clientRepository.DeletePermanentlyAsync(id);
}

private static string? NettoyerTexte(string? valeur)
{
    string texte = valeur?.Trim() ?? string.Empty;
    return texte.Length == 0 ? null : texte;
}


}
