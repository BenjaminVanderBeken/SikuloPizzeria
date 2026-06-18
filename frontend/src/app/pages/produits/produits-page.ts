import { Component, inject, OnInit } from '@angular/core';
import {
FormArray,
FormBuilder,
FormGroup,
ReactiveFormsModule,
Validators,
} from '@angular/forms';
import {
CreateProduitDto,
Produit,
UpdateProduitDto,
VarianteProduit,
VarianteProduitRequest,
} from '../../models/produit.model';
import { CategorieService } from '../../services/categorie.service';
import { ProduitService } from '../../services/produit.service';

@Component({
selector: 'app-produits-page',
standalone: true,
imports: [ReactiveFormsModule],
templateUrl: './produits-page.html',
styleUrl: './produits-page.css',
})
export class ProduitsPage implements OnInit {
private readonly formBuilder = inject(FormBuilder);

readonly produitService = inject(ProduitService);
readonly categorieService = inject(CategorieService);

readonly formulaire = this.formBuilder.group({
categorieId: [
0,
[
Validators.required,
Validators.min(1),
],
],
nom: [
'',
[
Validators.required,
Validators.maxLength(100),
],
],
description: [''],
prixBase: [
0,
[
Validators.required,
Validators.min(0),
],
],
prixPromotion: [
null as number | null,
[
Validators.min(0),
],
],
imageUrl: [''],
permetSupplement: [false],
actif: [true],
populaire: [false],
ordreAffichage: [
0,
[
Validators.required,
Validators.min(0),
],
],
variantes: this.formBuilder.array<FormGroup>([]),
});

get variantes(): FormArray<FormGroup> {
return this.formulaire.controls.variantes;
}

ngOnInit(): void {
this.categorieService.charger();


this.produitService.charger().subscribe({
  error: () => undefined,
});


}

ajouterVariante(variante?: VarianteProduit): void {
this.variantes.push(
this.formBuilder.group({
id: [
variante?.id ?? null,
],
nom: [
variante?.nom ?? '',
[
Validators.required,
Validators.maxLength(100),
],
],
prix: [
variante?.prix ?? 0,
[
Validators.required,
Validators.min(0),
],
],
actif: [
variante?.actif ?? true,
],
ordreAffichage: [
variante?.ordreAffichage ?? this.variantes.length,
[
Validators.required,
Validators.min(0),
],
],
}),
);
}

supprimerVariante(index: number): void {
this.variantes.removeAt(index);
}

enregistrer(): void {
this.produitService.effacerErreur();


if (this.formulaire.invalid) {
  this.formulaire.markAllAsTouched();
  return;
}

const valeur = this.formulaire.getRawValue();

const variantes: VarianteProduitRequest[] =
  valeur.variantes.map((variante) => ({
    id: variante['id'] ?? null,
    nom: String(variante['nom'] ?? '').trim(),
    prix: Number(variante['prix'] ?? 0),
    actif: Boolean(variante['actif']),
    ordreAffichage: Number(
      variante['ordreAffichage'] ?? 0,
    ),
  }));

const dtoCreation: CreateProduitDto = {
  categorieId: Number(valeur.categorieId),
  nom: String(valeur.nom ?? '').trim(),
  description: this.nettoyerTexte(
    valeur.description,
  ),
  prixBase: Number(valeur.prixBase),
  prixPromotion:
    valeur.prixPromotion === null
      ? null
      : Number(valeur.prixPromotion),
  imageUrl: this.nettoyerTexte(
    valeur.imageUrl,
  ),
  permetSupplement: Boolean(
    valeur.permetSupplement,
  ),
  populaire: Boolean(valeur.populaire),
  ordreAffichage: Number(
    valeur.ordreAffichage,
  ),
  variantes,
};

const produitId =
  this.produitService.produitEnEditionId();

if (produitId === null) {
  this.produitService
    .creer(dtoCreation)
    .subscribe({
      next: () =>
        this.reinitialiserFormulaire(),
      error: () => undefined,
    });

  return;
}

const dtoModification: UpdateProduitDto = {
  ...dtoCreation,
  actif: Boolean(valeur.actif),
};

this.produitService
  .modifier(produitId, dtoModification)
  .subscribe({
    next: () =>
      this.reinitialiserFormulaire(),
    error: () => undefined,
  });


}

modifier(produit: Produit): void {
this.produitService.selectionnerPourEdition(
produit.id,
);


this.formulaire.patchValue({
  categorieId: produit.categorieId,
  nom: produit.nom,
  description: produit.description ?? '',
  prixBase: produit.prixBase,
  prixPromotion: produit.prixPromotion,
  imageUrl: produit.imageUrl ?? '',
  permetSupplement:
    produit.permetSupplement,
  actif: produit.actif,
  populaire: produit.populaire,
  ordreAffichage:
    produit.ordreAffichage,
});

this.variantes.clear();

for (const variante of produit.variantes) {
  this.ajouterVariante(variante);
}

window.scrollTo({
  top: 0,
  behavior: 'smooth',
});


}

annulerEdition(): void {
this.reinitialiserFormulaire();
}

desactiver(produit: Produit): void {
const confirmation = window.confirm(
`Desactiver le produit "${produit.nom}" ?`,
);


if (!confirmation) {
  return;
}

this.produitService
  .desactiver(produit.id)
  .subscribe({
    error: () => undefined,
  });


}

reactiver(produit: Produit): void {
const confirmation = window.confirm(
`Reactiver le produit "${produit.nom}" ?`,
);


if (!confirmation) {
  return;
}

this.produitService
  .reactiver(produit.id)
  .subscribe({
    error: () => undefined,
  });


}

supprimerDefinitivement(
produit: Produit,
): void {
const confirmation = window.confirm(
`Supprimer definitivement le produit "${produit.nom}" ? Cette action est irreversible.`,
);

if (!confirmation) {
  return;
}

this.produitService
  .supprimerDefinitivement(produit.id)
  .subscribe({
    error: () => undefined,
  });


}

private reinitialiserFormulaire(): void {
this.produitService.annulerEdition();
this.variantes.clear();


this.formulaire.reset({
  categorieId: 0,
  nom: '',
  description: '',
  prixBase: 0,
  prixPromotion: null,
  imageUrl: '',
  permetSupplement: false,
  actif: true,
  populaire: false,
  ordreAffichage: 0,
});


}

private nettoyerTexte(
valeur: string | null | undefined,
): string | null {
const texte = valeur?.trim() ?? '';


return texte.length === 0
  ? null
  : texte;


}
}
