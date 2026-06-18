using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Exceptions;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.Core.Services;

public sealed class CommandeService : ICommandeService
{
private const decimal TauxTva = 0.06m;
private const decimal FraisLivraison = 3.00m;


private readonly ICommandeRepository _commandeRepository;
private readonly IProduitRepository _produitRepository;
private readonly IClientRepository _clientRepository;

public CommandeService(
    ICommandeRepository commandeRepository,
    IProduitRepository produitRepository,
    IClientRepository clientRepository)
{
    _commandeRepository = commandeRepository;
    _produitRepository = produitRepository;
    _clientRepository = clientRepository;
}

public Task<IEnumerable<CommandeDto>> GetAllAsync()
{
    return _commandeRepository.GetAllAsync();
}

public Task<CommandeDto?> GetByIdAsync(int id)
{
    return _commandeRepository.GetByIdAsync(id);
}

public async Task<CommandeDto> CreateAsync(CreateCommandeDto dto)
{
    if (dto.Details.Count == 0)
    {
        throw new BusinessRuleException(
            "La commande doit contenir au moins un produit."
        );
    }

    Client? client = null;

    if (dto.ClientId.HasValue)
    {
        client = await _clientRepository.GetByIdAsync(dto.ClientId.Value);

        if (client is null)
        {
            throw new BusinessRuleException(
                "Le client sélectionné est introuvable."
            );
        }

        if (!client.Actif)
        {
            throw new BusinessRuleException(
                "Le client sélectionné est inactif."
            );
        }
    }

    if (dto.TypeCommande == "LIVRAISON" && client is null)
    {
        throw new BusinessRuleException(
            "Un client doit être sélectionné pour une livraison."
        );
    }

    List<DetailCommande> details = [];

    foreach (CreateDetailCommandeDto detailDto in dto.Details)
    {
        ProduitDto? produit =
            await _produitRepository.GetByIdAsync(detailDto.ProduitId);

        if (produit is null)
        {
            throw new BusinessRuleException(
                $"Le produit avec l'identifiant {detailDto.ProduitId} est introuvable."
            );
        }

        if (!produit.Actif)
        {
            throw new BusinessRuleException(
                $"Le produit « {produit.Nom} » est inactif."
            );
        }

        decimal prixUnitaire;
        string? nomVariante = null;

        if (detailDto.VarianteId.HasValue)
        {
            var variante = produit.Variantes.FirstOrDefault(
                item => item.Id == detailDto.VarianteId.Value
            );

            if (variante is null)
            {
                throw new BusinessRuleException(
                    $"La variante sélectionnée n'appartient pas au produit « {produit.Nom} »."
                );
            }

            if (!variante.Actif)
            {
                throw new BusinessRuleException(
                    $"La variante « {variante.Nom} » est inactive."
                );
            }

            prixUnitaire = variante.Prix;
            nomVariante = variante.Nom;
        }
        else
        {
            prixUnitaire = produit.PrixPromotion ?? produit.PrixBase;
        }

        decimal prixTotal = Math.Round(
            prixUnitaire * detailDto.Quantite,
            2
        );

        details.Add(new DetailCommande
        {
            ProduitId = produit.Id,
            VarianteId = detailDto.VarianteId,
            Quantite = detailDto.Quantite,
            PrixUnitaire = prixUnitaire,
            PrixTotal = prixTotal,
            NomVariante = nomVariante,
            NotesParticulieres = NettoyerTexte(
                detailDto.NotesParticulieres
            ),
            CoutSupplements = 0
        });
    }

    decimal sousTotal = Math.Round(
        details.Sum(detail => detail.PrixTotal + detail.CoutSupplements),
        2
    );

    decimal tvaMontant = Math.Round(sousTotal * TauxTva, 2);

    decimal fraisLivraison =
        dto.TypeCommande == "LIVRAISON"
            ? FraisLivraison
            : 0;

    decimal montantTotal = Math.Round(
        sousTotal + tvaMontant + fraisLivraison,
        2
    );

    string numeroCommande = await GenererNumeroCommandeAsync();

    Commande commande = new()
    {
        NumeroCommande = numeroCommande,
        ClientId = dto.ClientId,
        PromotionId = null,
        DateLivraisonPrevue =
            dto.TypeCommande == "LIVRAISON"
                ? dto.DateLivraisonPrevue ?? DateTime.Now.AddMinutes(45)
                : dto.DateLivraisonPrevue,
        DateLivraisonReelle = null,
        TypeCommande = dto.TypeCommande,
        Statut = "EN_ATTENTE",
        SousTotal = sousTotal,
        TvaMontant = tvaMontant,
        FraisLivraison = fraisLivraison,
        ReductionMontant = 0,
        MontantTotal = montantTotal,
        ModePaiement = dto.ModePaiement,
        StatutPaiement = "NON_PAYEE",
        NotesClient = NettoyerTexte(dto.NotesClient),
        NotesCuisine = NettoyerTexte(dto.NotesCuisine),
        DemandesSpeciales = NettoyerTexte(dto.DemandesSpeciales)
    };

    int commandeId = await _commandeRepository.CreateAsync(
        commande,
        details
    );

    return await _commandeRepository.GetByIdAsync(commandeId)
        ?? throw new InvalidOperationException(
            "La commande a été enregistrée mais n'a pas pu être relue."
        );
}

public async Task<bool> UpdateStatusAsync(
    int id,
    UpdateCommandeStatusDto dto)
{
    CommandeDto? commande =
        await _commandeRepository.GetByIdAsync(id);

    if (commande is null)
    {
        return false;
    }

    if (commande.Statut == dto.Statut)
    {
        return true;
    }

    return await _commandeRepository.UpdateStatusAsync(
        id,
        dto.Statut
    );
}

public async Task<bool> UpdatePaymentAsync(
    int id,
    UpdateCommandePaymentDto dto)
{
    CommandeDto? commande =
        await _commandeRepository.GetByIdAsync(id);

    if (commande is null)
    {
        return false;
    }

    if (
        commande.ModePaiement == dto.ModePaiement &&
        commande.StatutPaiement == dto.StatutPaiement
    )
    {
        return true;
    }

    return await _commandeRepository.UpdatePaymentAsync(
        id,
        dto.ModePaiement,
        dto.StatutPaiement
    );
}

private async Task<string> GenererNumeroCommandeAsync()
{
    for (int tentative = 0; tentative < 10; tentative++)
    {
        string numero =
            $"CMD-{DateTime.Now:yyyyMMdd-HHmmss}-{Random.Shared.Next(100, 1000)}";

        bool existe =
            await _commandeRepository.ExistsByNumberAsync(numero);

        if (!existe)
        {
            return numero;
        }
    }

    throw new InvalidOperationException(
        "Impossible de générer un numéro de commande unique."
    );
}

private static string? NettoyerTexte(string? valeur)
{
    string texte = valeur?.Trim() ?? string.Empty;
    return texte.Length == 0 ? null : texte;
}


}

//On crée maintenant CommandeService.cs. Il va :

//vérifier le client et les produits ;
//récupérer les prix dans MySQL ;
//vérifier que la variante appartient au produit ;
//calculer les lignes, la TVA et les frais de livraison ;
//appeler la transaction Dapper du repository.