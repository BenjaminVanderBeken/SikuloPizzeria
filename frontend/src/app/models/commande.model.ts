export type TypeCommande =
| 'SUR_PLACE'
| 'A_EMPORTER'
| 'LIVRAISON';

export type StatutCommande =
| 'EN_ATTENTE'
| 'CONFIRMEE'
| 'EN_PREPARATION'
| 'PRETE'
| 'LIVREE'
| 'TERMINEE'
| 'ANNULEE';

export type ModePaiement =
| 'ESPECES'
| 'CARTE'
| 'VIREMENT'
| 'EN_ATTENTE';

export type StatutPaiement =
| 'NON_PAYEE'
| 'PAYEE'
| 'REMBOURSEE';

export interface DetailCommande {
id: number;
produitId: number;
produitNom: string;
varianteId: number | null;
nomVariante: string | null;
quantite: number;
prixUnitaire: number;
prixTotal: number;
notesParticulieres: string | null;
coutSupplements: number;
}

export interface Commande {
id: number;
numeroCommande: string;
clientId: number | null;
clientNomComplet: string;
dateCommande: string;
dateLivraisonPrevue: string | null;
dateLivraisonReelle: string | null;
typeCommande: TypeCommande;
statut: StatutCommande;
sousTotal: number;
tvaMontant: number;
fraisLivraison: number;
reductionMontant: number;
montantTotal: number;
modePaiement: ModePaiement;
statutPaiement: StatutPaiement;
notesClient: string | null;
notesCuisine: string | null;
demandesSpeciales: string | null;
details: DetailCommande[];
}

export interface CreateDetailCommandeDto {
produitId: number;
varianteId: number | null;
quantite: number;
notesParticulieres: string | null;
}

export interface CreateCommandeDto {
clientId: number | null;
dateLivraisonPrevue: string | null;
typeCommande: TypeCommande;
modePaiement: ModePaiement;
notesClient: string | null;
notesCuisine: string | null;
demandesSpeciales: string | null;
details: CreateDetailCommandeDto[];
}

export interface UpdateCommandeStatusDto {
statut: StatutCommande;
}

export interface UpdateCommandePaymentDto {
modePaiement: ModePaiement;
statutPaiement: StatutPaiement;
}
