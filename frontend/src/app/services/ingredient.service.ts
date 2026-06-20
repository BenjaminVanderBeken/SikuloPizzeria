import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateIngredientDto, Ingredient, UpdateIngredientDto } from '../models/ingredient.model';

@Injectable({
  providedIn: 'root',
})
export class IngredientService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/ingredients`;

  private readonly _ingredients = signal<Ingredient[]>([]);
  private readonly _chargement = signal(false);
  private readonly _erreur = signal('');
  private readonly _ingredientEnEditionId = signal<number | null>(null);

  readonly ingredients = this._ingredients.asReadonly();
  readonly chargement = this._chargement.asReadonly();
  readonly erreur = this._erreur.asReadonly();
  readonly ingredientEnEditionId = this._ingredientEnEditionId.asReadonly();

  readonly ingredientsActifs = computed(() =>
    this._ingredients().filter((ingredient) => ingredient.actif),
  );

  readonly nombreIngredientsActifs = computed(() => this.ingredientsActifs().length);

  readonly ingredientEnEdition = computed(() => {
    const id = this._ingredientEnEditionId();

    if (id === null) {
      return null;
    }

    return this._ingredients().find((ingredient) => ingredient.id === id) ?? null;
  });

  charger(): Observable<Ingredient[]> {
    this._chargement.set(true);
    this._erreur.set('');

    return this.http.get<Ingredient[]>(this.apiUrl).pipe(
      tap((ingredients) => this._ingredients.set(ingredients)),
      catchError((erreur: HttpErrorResponse) => {
        this._erreur.set(this.extraireMessage(erreur));
        return throwError(() => erreur);
      }),
      finalize(() => this._chargement.set(false)),
    );
  }

  creer(dto: CreateIngredientDto): Observable<Ingredient> {
    this._chargement.set(true);
    this._erreur.set('');

    return this.http.post<Ingredient>(this.apiUrl, dto).pipe(
      tap((ingredientCree) => {
        this._ingredients.update((ingredients) => [...ingredients, ingredientCree]);
      }),
      catchError((erreur: HttpErrorResponse) => {
        this._erreur.set(this.extraireMessage(erreur));
        return throwError(() => erreur);
      }),
      finalize(() => this._chargement.set(false)),
    );
  }

  modifier(id: number, dto: UpdateIngredientDto): Observable<Ingredient> {
    this._chargement.set(true);
    this._erreur.set('');

    return this.http.put<Ingredient>(`${this.apiUrl}/${id}`, dto).pipe(
      tap((ingredientModifie) => {
        this._ingredients.update((ingredients) =>
          ingredients.map((ingredient) => (ingredient.id === id ? ingredientModifie : ingredient)),
        );

        this._ingredientEnEditionId.set(null);
      }),
      catchError((erreur: HttpErrorResponse) => {
        this._erreur.set(this.extraireMessage(erreur));
        return throwError(() => erreur);
      }),
      finalize(() => this._chargement.set(false)),
    );
  }

  desactiver(id: number): Observable<void> {
    this._chargement.set(true);
    this._erreur.set('');

    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        this._ingredients.update((ingredients) =>
          ingredients.map((ingredient) =>
            ingredient.id === id ? { ...ingredient, actif: false } : ingredient,
          ),
        );

        if (this._ingredientEnEditionId() === id) {
          this._ingredientEnEditionId.set(null);
        }
      }),
      catchError((erreur: HttpErrorResponse) => {
        this._erreur.set(this.extraireMessage(erreur));
        return throwError(() => erreur);
      }),
      finalize(() => this._chargement.set(false)),
    );
  }

  reactiver(id: number): Observable<void> {
    this._chargement.set(true);
    this._erreur.set('');

    return this.http.patch<void>(`${this.apiUrl}/${id}/reactiver`, {}).pipe(
      tap(() => {
        this._ingredients.update((ingredients) =>
          ingredients.map((ingredient) =>
            ingredient.id === id ? { ...ingredient, actif: true } : ingredient,
          ),
        );
      }),
      catchError((erreur: HttpErrorResponse) => {
        this._erreur.set(this.extraireMessage(erreur));
        return throwError(() => erreur);
      }),
      finalize(() => this._chargement.set(false)),
    );
  }

  supprimerDefinitivement(id: number): Observable<void> {
    this._chargement.set(true);
    this._erreur.set('');

    return this.http.delete<void>(`${this.apiUrl}/${id}/definitif`).pipe(
      tap(() => {
        this._ingredients.update((ingredients) =>
          ingredients.filter((ingredient) => ingredient.id !== id),
        );

        if (this._ingredientEnEditionId() === id) {
          this._ingredientEnEditionId.set(null);
        }
      }),
      catchError((erreur: HttpErrorResponse) => {
        this._erreur.set(this.extraireMessage(erreur));
        return throwError(() => erreur);
      }),
      finalize(() => this._chargement.set(false)),
    );
  }

  selectionnerPourEdition(id: number): void {
    this._ingredientEnEditionId.set(id);
  }

  annulerEdition(): void {
    this._ingredientEnEditionId.set(null);
  }

  effacerErreur(): void {
    this._erreur.set('');
  }

  private extraireMessage(erreur: HttpErrorResponse): string {
    if (typeof erreur.error?.message === 'string') {
      return erreur.error.message;
    }

    if (erreur.status === 0) {
      return "Impossible de contacter l'API.";
    }

    return `Une erreur est survenue (${erreur.status}).`;
  }
}
