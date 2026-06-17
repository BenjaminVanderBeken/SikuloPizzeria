import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import {
  catchError,
  finalize,
  Observable,
  tap,
  throwError
} from 'rxjs';

import { environment } from '../../environments/environment';
import {
  Categorie,
  CreateCategorieRequest,
  UpdateCategorieRequest
} from '../models/categorie.model';

@Injectable({
  providedIn: 'root'
})
export class CategorieService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/categories`;

  private readonly categoriesSignal = signal<Categorie[]>([]);
  private readonly chargementSignal = signal(false);
  private readonly erreurSignal = signal<string | null>(null);
  private readonly editionIdSignal = signal<number | null>(null);

  readonly categories = this.categoriesSignal.asReadonly();
  readonly chargement = this.chargementSignal.asReadonly();
  readonly erreur = this.erreurSignal.asReadonly();
  readonly editionId = this.editionIdSignal.asReadonly();

  charger(): void {
    this.chargementSignal.set(true);
    this.erreurSignal.set(null);

    this.http
      .get<Categorie[]>(this.apiUrl)
      .pipe(finalize(() => this.chargementSignal.set(false)))
      .subscribe({
        next: categories => {
          this.categoriesSignal.set(categories);
        },
        error: erreur => {
          this.erreurSignal.set(this.extraireMessageErreur(erreur));
        }
      });
  }

  creer(
    donnees: CreateCategorieRequest
  ): Observable<Categorie> {
    this.chargementSignal.set(true);
    this.erreurSignal.set(null);

    return this.http
      .post<Categorie>(this.apiUrl, donnees)
      .pipe(
        tap(categorie => {
          this.categoriesSignal.update(categories =>
            [...categories, categorie].sort(
              (a, b) =>
                a.ordreAffichage - b.ordreAffichage ||
                a.nom.localeCompare(b.nom)
            )
          );
        }),
        catchError(erreur => {
          this.erreurSignal.set(
            this.extraireMessageErreur(erreur)
          );

          return throwError(() => erreur);
        }),
        finalize(() => this.chargementSignal.set(false))
      );
  }

  modifier(
    id: number,
    donnees: UpdateCategorieRequest
  ): Observable<Categorie> {
    this.chargementSignal.set(true);
    this.erreurSignal.set(null);

    return this.http
      .put<Categorie>(`${this.apiUrl}/${id}`, donnees)
      .pipe(
        tap(categorieModifiee => {
          this.categoriesSignal.update(categories =>
            categories
              .map(categorie =>
                categorie.id === id
                  ? categorieModifiee
                  : categorie
              )
              .sort(
                (a, b) =>
                  a.ordreAffichage - b.ordreAffichage ||
                  a.nom.localeCompare(b.nom)
              )
          );

          this.terminerEdition();
        }),
        catchError(erreur => {
          this.erreurSignal.set(
            this.extraireMessageErreur(erreur)
          );

          return throwError(() => erreur);
        }),
        finalize(() => this.chargementSignal.set(false))
      );
  }

  desactiver(id: number): Observable<void> {
    this.chargementSignal.set(true);
    this.erreurSignal.set(null);

    return this.http
      .delete<void>(`${this.apiUrl}/${id}`)
      .pipe(
        tap(() => {
          this.categoriesSignal.update(categories =>
            categories.map(categorie =>
              categorie.id === id
                ? { ...categorie, actif: false }
                : categorie
            )
          );
        }),
        catchError(erreur => {
          this.erreurSignal.set(
            this.extraireMessageErreur(erreur)
          );

          return throwError(() => erreur);
        }),
        finalize(() => this.chargementSignal.set(false))
      );
  }

  commencerEdition(id: number): void {
    this.editionIdSignal.set(id);
  }

  terminerEdition(): void {
    this.editionIdSignal.set(null);
  }

  effacerErreur(): void {
    this.erreurSignal.set(null);
  }

  private extraireMessageErreur(
    erreur: HttpErrorResponse
  ): string {
    if (typeof erreur.error?.message === 'string') {
      return erreur.error.message;
    }

    if (erreur.status === 0) {
      return "Impossible de joindre l'API.";
    }

    return `Une erreur HTTP ${erreur.status} est survenue.`;
  }
}