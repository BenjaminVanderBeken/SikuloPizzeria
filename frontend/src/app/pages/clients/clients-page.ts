import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
Client,
CreateClientDto,
UpdateClientDto,
} from '../../models/client.model';
import { ClientService } from '../../services/client.service';

@Component({
selector: 'app-clients-page',
standalone: true,
imports: [ReactiveFormsModule],
templateUrl: './clients-page.html',
styleUrl: './clients-page.css',
})
export class ClientsPage implements OnInit {
private readonly formBuilder = inject(FormBuilder);
readonly clientService = inject(ClientService);

readonly formulaire = this.formBuilder.group({
nom: ['', [Validators.required, Validators.maxLength(100)]],
prenom: ['', [Validators.maxLength(100)]],
email: ['', [Validators.email, Validators.maxLength(150)]],
telephone: ['', [Validators.maxLength(25)]],
adresseRue: ['', [Validators.maxLength(200)]],
adresseNumero: ['', [Validators.maxLength(20)]],
adresseBoite: ['', [Validators.maxLength(10)]],
adresseCodePostal: ['', [Validators.maxLength(10)]],
adresseVille: ['', [Validators.maxLength(100)]],
adressePays: ['Belgique', [Validators.required, Validators.maxLength(50)]],
notes: [''],
clientVip: [false],
actif: [true],
});

ngOnInit(): void {
this.clientService.charger().subscribe({
error: () => undefined,
});
}

enregistrer(): void {
this.clientService.effacerErreur();


if (this.formulaire.invalid) {
  this.formulaire.markAllAsTouched();
  return;
}

const valeur = this.formulaire.getRawValue();

const dtoCreation: CreateClientDto = {
  nom: valeur.nom?.trim() ?? '',
  prenom: this.nettoyerTexte(valeur.prenom),
  email: this.nettoyerTexte(valeur.email),
  telephone: this.nettoyerTexte(valeur.telephone),
  adresseRue: this.nettoyerTexte(valeur.adresseRue),
  adresseNumero: this.nettoyerTexte(valeur.adresseNumero),
  adresseBoite: this.nettoyerTexte(valeur.adresseBoite),
  adresseCodePostal: this.nettoyerTexte(valeur.adresseCodePostal),
  adresseVille: this.nettoyerTexte(valeur.adresseVille),
  adressePays: valeur.adressePays?.trim() || 'Belgique',
  notes: this.nettoyerTexte(valeur.notes),
  clientVip: Boolean(valeur.clientVip),
};

const clientId = this.clientService.clientEnEditionId();

if (clientId === null) {
  this.clientService.creer(dtoCreation).subscribe({
    next: () => this.reinitialiserFormulaire(),
    error: () => undefined,
  });
  return;
}

const dtoModification: UpdateClientDto = {
  ...dtoCreation,
  actif: Boolean(valeur.actif),
};

this.clientService.modifier(clientId, dtoModification).subscribe({
  next: () => this.reinitialiserFormulaire(),
  error: () => undefined,
});


}

modifier(client: Client): void {
this.clientService.selectionnerPourEdition(client.id);


this.formulaire.patchValue({
  nom: client.nom,
  prenom: client.prenom ?? '',
  email: client.email ?? '',
  telephone: client.telephone ?? '',
  adresseRue: client.adresseRue ?? '',
  adresseNumero: client.adresseNumero ?? '',
  adresseBoite: client.adresseBoite ?? '',
  adresseCodePostal: client.adresseCodePostal ?? '',
  adresseVille: client.adresseVille ?? '',
  adressePays: client.adressePays,
  notes: client.notes ?? '',
  clientVip: client.clientVip,
  actif: client.actif,
});

window.scrollTo({ top: 0, behavior: 'smooth' });


}

annulerEdition(): void {
this.reinitialiserFormulaire();
}

desactiver(client: Client): void {
if (!window.confirm(`Désactiver le client "${client.nom}" ?`)) {
return;
}


this.clientService.desactiver(client.id).subscribe({
  error: () => undefined,
});


}

reactiver(client: Client): void {
if (!window.confirm(`Réactiver le client "${client.nom}" ?`)) {
return;
}


this.clientService.reactiver(client.id).subscribe({
  error: () => undefined,
});


}

supprimerDefinitivement(client: Client): void {
const confirmation = window.confirm(
`Supprimer définitivement le client "${client.nom}" ? Cette action est irréversible.`,
);


if (!confirmation) {
  return;
}

this.clientService.supprimerDefinitivement(client.id).subscribe({
  error: () => undefined,
});


}

private reinitialiserFormulaire(): void {
this.clientService.annulerEdition();


this.formulaire.reset({
  nom: '',
  prenom: '',
  email: '',
  telephone: '',
  adresseRue: '',
  adresseNumero: '',
  adresseBoite: '',
  adresseCodePostal: '',
  adresseVille: '',
  adressePays: 'Belgique',
  notes: '',
  clientVip: false,
  actif: true,
});


}

private nettoyerTexte(
valeur: string | null | undefined,
): string | null {
const texte = valeur?.trim() ?? '';
return texte.length === 0 ? null : texte;
}
}
