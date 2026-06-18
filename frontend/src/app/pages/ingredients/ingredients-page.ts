import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  CreateIngredientDto,
  Ingredient,
  TYPES_INGREDIENT,
  UpdateIngredientDto,
} from '../../models/ingredient.model';
import { IngredientService } from '../../services/ingredient.service';

@Component({
  selector: 'app-ingredients-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './ingredients-page.html',
  styleUrls: ['./ingredients-page.css'],
})
export class IngredientsPage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);

  readonly ingredientService = inject(IngredientService);
  readonly typesIngredient = TYPES_INGREDIENT;

  readonly formulaire = this.formBuilder.group({
    nom: ['', [Validators.required, Validators.maxLength(100)]],
    type: ['', [Validators.required]],
    stockActuel: [0, [Validators.required, Validators.min(0)]],
    uniteMesure: ['', [Validators.required, Validators.maxLength(30)]],
    prixUnitaire: [null as number | null, [Validators.min(0)]],
    allergenes: ['', [Validators.maxLength(255)]],
    actif: [true],
  });

  ngOnInit(): void {
    this.ingredientService.charger().subscribe({
      error: () => undefined,
    });
  }

  enregistrer(): void {
    this.ingredientService.effacerErreur();

    if (this.formulaire.invalid) {
      this.formulaire.markAllAsTouched();
      return;
    }

    const valeur = this.formulaire.getRawValue();

    const dtoCreation: CreateIngredientDto = {
      nom: String(valeur.nom ?? '').trim(),
      type: String(valeur.type ?? '').trim(),
      stockActuel: Number(valeur.stockActuel),
      uniteMesure: String(valeur.uniteMesure ?? '').trim(),
      prixUnitaire:
        valeur.prixUnitaire === null
          ? null
          : Number(valeur.prixUnitaire),
      allergenes: this.nettoyerTexte(valeur.allergenes),
    };

    const ingredientId =
      this.ingredientService.ingredientEnEditionId();

    if (ingredientId === null) {
      this.ingredientService.creer(dtoCreation).subscribe({
        next: () => this.reinitialiserFormulaire(),
        error: () => undefined,
      });

      return;
    }

    const dtoModification: UpdateIngredientDto = {
      ...dtoCreation,
      actif: Boolean(valeur.actif),
    };

    this.ingredientService
      .modifier(ingredientId, dtoModification)
      .subscribe({
        next: () => this.reinitialiserFormulaire(),
        error: () => undefined,
      });
  }

  modifier(ingredient: Ingredient): void {
    this.ingredientService.selectionnerPourEdition(
      ingredient.id,
    );

    this.formulaire.patchValue({
      nom: ingredient.nom,
      type: ingredient.type,
      stockActuel: ingredient.stockActuel,
      uniteMesure: ingredient.uniteMesure,
      prixUnitaire: ingredient.prixUnitaire,
      allergenes: ingredient.allergenes ?? '',
      actif: ingredient.actif,
    });

    window.scrollTo({
      top: 0,
      behavior: 'smooth',
    });
  }

  annulerEdition(): void {
    this.reinitialiserFormulaire();
  }

  desactiver(ingredient: Ingredient): void {
    const confirmation = window.confirm(
      `Desactiver l'ingredient "${ingredient.nom}" ?`,
    );

    if (!confirmation) {
      return;
    }

    this.ingredientService
      .desactiver(ingredient.id)
      .subscribe({
        error: () => undefined,
      });
  }

  private reinitialiserFormulaire(): void {
    this.ingredientService.annulerEdition();

    this.formulaire.reset({
      nom: '',
      type: '',
      stockActuel: 0,
      uniteMesure: '',
      prixUnitaire: null,
      allergenes: '',
      actif: true,
    });
  }

  private nettoyerTexte(
    valeur: string | null | undefined,
  ): string | null {
    const texte = valeur?.trim() ?? '';

    return texte.length === 0 ? null : texte;
  
}
reactiver(ingredient: Ingredient): void {
const confirmation = window.confirm(
`Réactiver l'ingrédient "${ingredient.nom}" ?`
);

if (!confirmation) {
return;
}

this.ingredientService.reactiver(ingredient.id).subscribe({
error: () => undefined,
});
}

supprimerDefinitivement(ingredient: Ingredient): void {
const confirmation = window.confirm(
`Supprimer définitivement l'ingrédient "${ingredient.nom}" ? Cette action est irréversible.`
);

if (!confirmation) {
return;
}

this.ingredientService.supprimerDefinitivement(ingredient.id).subscribe({
error: () => undefined,
});
}

}
