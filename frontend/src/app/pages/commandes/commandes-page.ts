import { Component, inject, OnInit } from '@angular/core';
import {
FormArray,
FormBuilder,
FormGroup,
ReactiveFormsModule,
Validators,
} from '@angular/forms';
import {
Commande,
CreateCommandeDto,
CreateDetailCommandeDto,
ModePaiement,
StatutCommande,
StatutPaiement,
TypeCommande,
} from '../../models/commande.model';
import {
Produit,
VarianteProduit,
} from '../../models/produit.model';
import { ClientService } from '../../services/client.service';
import { CommandeService } from '../../services/commande.service';
import { ProduitService } from '../../services/produit.service';

@Component({
selector: 'app-commandes-page',
standalone: true,
imports: [ReactiveFormsModule],
templateUrl: './commandes-page.html',
styleUrl: './commandes-page.css',
})
export class CommandesPage implements OnInit {
private readonly formBuilder = inject(FormBuilder);

readonly commandeService = inject(CommandeService);
readonly clientService = inject(ClientService);
readonly produitService = inject(ProduitService);

readonly typesCommande: TypeCommande[] = [
'SUR_PLACE',
'A_EMPORTER',
'LIVRAISON',
];

readonly modesPaiement: ModePaiement[] = [
'EN_ATTENTE',
'ESPECES',
'CARTE',
'VIREMENT',
];

readonly statutsCommande: StatutCommande[] = [
'EN_ATTENTE',
'CONFIRMEE',
'EN_PREPARATION',
'PRETE',
'LIVREE',
'TERMINEE',
'ANNULEE',
];

readonly statutsPaiement: StatutPaiement[] = [
'NON_PAYEE',
'PAYEE',
'REMBOURSEE',
];

readonly formulaire = this.formBuilder.group({
clientId: [null as number | null],
dateLivraisonPrevue: [''],
typeCommande: ['A_EMPORTER' as TypeCommande, Validators.required],
modePaiement: ['EN_ATTENTE' as ModePaiement, Validators.required],
notesClient: [''],
notesCuisine: [''],
demandesSpeciales: [''],
details: this.formBuilder.array<FormGroup>([]),
});

get details(): FormArray<FormGroup> {
return this.formulaire.controls.details;
}

ngOnInit(): void {
this.clientService.charger().subscribe({
error: () => undefined,
});


this.produitService.charger().subscribe({
  error: () => undefined,
});

this.commandeService.charger().subscribe({
  error: () => undefined,
});

this.ajouterLigne();


}

ajouterLigne(): void {
this.details.push(
this.formBuilder.group({
produitId: [0, [Validators.required, Validators.min(1)]],
varianteId: [null as number | null],
quantite: [1, [Validators.required, Validators.min(1)]],
notesParticulieres: [''],
}),
);
}

supprimerLigne(index: number): void {
if (this.details.length === 1) {
return;
}


this.details.removeAt(index);


}

produitSelectionne(index: number): Produit | null {
const produitId = Number(
this.details.at(index).get('produitId')?.value ?? 0,
);


return (
  this.produitService
    .produits()
    .find((produit) => produit.id === produitId) ?? null
);


}

variantesDisponibles(index: number): VarianteProduit[] {
const produit = this.produitSelectionne(index);


if (produit === null) {
  return [];
}

return produit.variantes.filter((variante) => variante.actif);


}

changerProduit(index: number): void {
this.details.at(index).patchValue({
varianteId: null,
});
}

enregistrer(): void {
this.commandeService.effacerErreur();


if (this.formulaire.invalid) {
  this.formulaire.markAllAsTouched();
  return;
}

const valeur = this.formulaire.getRawValue();

const details: CreateDetailCommandeDto[] = valeur.details.map(
  (detail) => ({
    produitId: Number(detail['produitId']),
    varianteId:
      detail['varianteId'] === null
        ? null
        : Number(detail['varianteId']),
    quantite: Number(detail['quantite']),
    notesParticulieres: this.nettoyerTexte(
      detail['notesParticulieres'],
    ),
  }),
);

const dto: CreateCommandeDto = {
  clientId:
    valeur.clientId === null
      ? null
      : Number(valeur.clientId),
  dateLivraisonPrevue: valeur.dateLivraisonPrevue
    ? new Date(valeur.dateLivraisonPrevue).toISOString()
    : null,
  typeCommande:
    valeur.typeCommande ?? 'A_EMPORTER',
  modePaiement:
    valeur.modePaiement ?? 'EN_ATTENTE',
  notesClient: this.nettoyerTexte(valeur.notesClient),
  notesCuisine: this.nettoyerTexte(valeur.notesCuisine),
  demandesSpeciales: this.nettoyerTexte(
    valeur.demandesSpeciales,
  ),
  details,
};

this.commandeService.creer(dto).subscribe({
  next: () => this.reinitialiserFormulaire(),
  error: () => undefined,
});


}

changerStatut(
commande: Commande,
evenement: Event,
): void {
const statut = (evenement.target as HTMLSelectElement)
.value as StatutCommande;


this.commandeService
  .modifierStatut(commande.id, { statut })
  .subscribe({
    error: () => undefined,
  });


}

changerModePaiement(
commande: Commande,
evenement: Event,
): void {
const modePaiement = (evenement.target as HTMLSelectElement)
.value as ModePaiement;


this.commandeService
  .modifierPaiement(commande.id, {
    modePaiement,
    statutPaiement: commande.statutPaiement,
  })
  .subscribe({
    error: () => undefined,
  });


}

changerStatutPaiement(
commande: Commande,
evenement: Event,
): void {
const statutPaiement = (
evenement.target as HTMLSelectElement
).value as StatutPaiement;


this.commandeService
  .modifierPaiement(commande.id, {
    modePaiement: commande.modePaiement,
    statutPaiement,
  })
  .subscribe({
    error: () => undefined,
  });


}
annulerCommande(commande: Commande): void {
if (commande.statut === 'ANNULEE') {
return;
}

const confirmation = window.confirm(
`Voulez-vous vraiment annuler la commande ${commande.numeroCommande} ?`,
);

if (!confirmation) {
return;
}

this.commandeService.effacerErreur();

this.commandeService
.modifierStatut(commande.id, {
statut: 'ANNULEE',
})
.subscribe({
error: () => undefined,
});
}

private reinitialiserFormulaire(): void {
this.details.clear();


this.formulaire.reset({
  clientId: null,
  dateLivraisonPrevue: '',
  typeCommande: 'A_EMPORTER',
  modePaiement: 'EN_ATTENTE',
  notesClient: '',
  notesCuisine: '',
  demandesSpeciales: '',
});

this.ajouterLigne();


}

private nettoyerTexte(
valeur: string | null | undefined,
): string | null {
const texte = valeur?.trim() ?? '';
return texte.length === 0 ? null : texte;
}
}
