import { Component, inject, OnInit } from '@angular/core';
import {
FormBuilder,
ReactiveFormsModule,
Validators,
} from '@angular/forms';
import {
CreateCompositionProduitDto,
} from '../../models/composition-produit.model';
import { CompositionProduitService } from '../../services/composition-produit.service';
import { IngredientService } from '../../services/ingredient.service';
import { ProduitService } from '../../services/produit.service';

@Component({
selector: 'app-compositions-page',
standalone: true,
imports: [ReactiveFormsModule],
templateUrl: './compositions-page.html',
styleUrl: './compositions-page.css',
})
export class CompositionsPage implements OnInit {
private readonly formBuilder = inject(FormBuilder);

readonly compositionService = inject(CompositionProduitService);
readonly produitService = inject(ProduitService);
readonly ingredientService = inject(IngredientService);

readonly formulaire = this.formBuilder.group({
produitId: [0, [Validators.required, Validators.min(1)]],
ingredientId: [0, [Validators.required, Validators.min(1)]],
quantite: [1, [Validators.required, Validators.min(0.01)]],
unite: ['g', [Validators.required, Validators.maxLength(20)]],
ordreAffichage: [1, [Validators.required, Validators.min(0)]],
});

ngOnInit(): void {
this.produitService.charger().subscribe({
error: () => undefined,
});


this.ingredientService.charger().subscribe({
  error: () => undefined,
});

this.compositionService.charger().subscribe({
  error: () => undefined,
});


}

enregistrer(): void {
this.compositionService.effacerErreur();


if (this.formulaire.invalid) {
  this.formulaire.markAllAsTouched();
  return;
}

const valeur = this.formulaire.getRawValue();

const dto: CreateCompositionProduitDto = {
  produitId: Number(valeur.produitId),
  ingredientId: Number(valeur.ingredientId),
  quantite: Number(valeur.quantite),
  unite: valeur.unite?.trim() ?? 'g',
  ordreAffichage: Number(valeur.ordreAffichage),
};

this.compositionService.creer(dto).subscribe({
  next: () => this.reinitialiserFormulaire(),
  error: () => undefined,
});


}

supprimer(id: number): void {
const confirmation = window.confirm(
'Voulez-vous vraiment retirer cet ingrédient de la composition ?',
);


if (!confirmation) {
  return;
}

this.compositionService.supprimer(id).subscribe({
  error: () => undefined,
});


}

private reinitialiserFormulaire(): void {
this.formulaire.reset({
produitId: 0,
ingredientId: 0,
quantite: 1,
unite: 'g',
ordreAffichage: 1,
});
}
}
