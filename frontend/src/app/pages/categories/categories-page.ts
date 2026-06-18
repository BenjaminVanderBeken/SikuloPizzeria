import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { Categorie } from '../../models/categorie.model';
import { CategorieService } from '../../services/categorie.service';

@Component({
  selector: 'app-categories-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './categories-page.html',
  styleUrl: './categories-page.css'
})
export class CategoriesPage implements OnInit {
  readonly categorieService = inject(CategorieService);

  private readonly formBuilder = inject(FormBuilder);

  readonly formulaire = this.formBuilder.nonNullable.group({
    nom: [
      '',
      [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(100)
      ]
    ],
    description: ['', Validators.maxLength(500)],
    ordreAffichage: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],
    actif: [true]
  });

  ngOnInit(): void {
    this.categorieService.charger();
  }

  enregistrer(): void {
    if (this.formulaire.invalid) {
      this.formulaire.markAllAsTouched();
      return;
    }

    const valeur = this.formulaire.getRawValue();
    const editionId = this.categorieService.editionId();

    if (editionId === null) {
      this.categorieService
        .creer({
          nom: valeur.nom,
          description: valeur.description || null,
          ordreAffichage: valeur.ordreAffichage
        })
        .subscribe({
          next: () => this.reinitialiserFormulaire()
        });

      return;
    }

    this.categorieService
      .modifier(editionId, {
        nom: valeur.nom,
        description: valeur.description || null,
        ordreAffichage: valeur.ordreAffichage,
        actif: valeur.actif
      })
      .subscribe({
        next: () => this.reinitialiserFormulaire()
      });
  }

  preparerModification(categorie: Categorie): void {
    this.categorieService.commencerEdition(categorie.id);

    this.formulaire.setValue({
      nom: categorie.nom,
      description: categorie.description ?? '',
      ordreAffichage: categorie.ordreAffichage,
      actif: categorie.actif
    });
  }

  annulerModification(): void {
    this.categorieService.terminerEdition();
    this.reinitialiserFormulaire();
  }

  desactiver(id: number): void {
    this.categorieService.desactiver(id).subscribe();
  }

  private reinitialiserFormulaire(): void {
    this.formulaire.reset({
      nom: '',
      description: '',
      ordreAffichage: 0,
      actif: true
    });
  }
  reactiver(categorie: Categorie): void {
const confirmation = window.confirm(
`Réactiver la catégorie "${categorie.nom}" ?`
);

if (!confirmation) {
return;
}

this.categorieService.reactiver(categorie.id).subscribe({
error: () => undefined,
});
}

supprimerDefinitivement(categorie: Categorie): void {
const confirmation = window.confirm(
`Supprimer définitivement la catégorie "${categorie.nom}" ? Cette action est irréversible.`
);

if (!confirmation) {
return;
}

this.categorieService.supprimerDefinitivement(categorie.id).subscribe({
error: () => undefined,
});
}

}