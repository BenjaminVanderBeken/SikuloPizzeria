import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CompositionProduit,
  CreateCompositionProduitDto,
} from '../models/composition-produit.model';

@Injectable({
  providedIn: 'root',
})
export class CompositionProduitService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/composition-produits`;

  readonly compositions = signal<CompositionProduit[]>([]);
  readonly chargement = signal<boolean>(false);
  readonly erreur = signal<string>('');

  readonly nombreCompositions = computed(() => this.compositions().length);

  charger(): Observable<CompositionProduit[]> {
    this.chargement.set(true);
    this.erreur.set('');

    return this.http.get<CompositionProduit[]>(this.apiUrl).pipe(
      tap({
        next: (compositions) => {
          this.compositions.set(compositions);
          this.chargement.set(false);
        },
        error: () => {
          this.erreur.set('Impossible de charger les compositions.');
          this.chargement.set(false);
        },
      }),
    );
  }

  chargerParProduit(produitId: number): Observable<CompositionProduit[]> {
    this.chargement.set(true);
    this.erreur.set('');

    return this.http.get<CompositionProduit[]>(`${this.apiUrl}/produit/${produitId}`).pipe(
      tap({
        next: (compositions) => {
          this.compositions.set(compositions);
          this.chargement.set(false);
        },
        error: () => {
          this.erreur.set('Impossible de charger la composition du produit.');
          this.chargement.set(false);
        },
      }),
    );
  }

  creer(dto: CreateCompositionProduitDto): Observable<CompositionProduit> {
    this.chargement.set(true);
    this.erreur.set('');

    return this.http.post<CompositionProduit>(this.apiUrl, dto).pipe(
      tap({
        next: (composition) => {
          this.compositions.update((compositions) => [...compositions, composition]);
          this.chargement.set(false);
        },
        error: (erreur) => {
          this.erreur.set(erreur.error?.message ?? 'Impossible de créer la composition.');
          this.chargement.set(false);
        },
      }),
    );
  }

  supprimer(id: number): Observable<void> {
    this.chargement.set(true);
    this.erreur.set('');

    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap({
        next: () => {
          this.compositions.update((compositions) =>
            compositions.filter((composition) => composition.id !== id),
          );
          this.chargement.set(false);
        },
        error: () => {
          this.erreur.set('Impossible de supprimer cette composition.');
          this.chargement.set(false);
        },
      }),
    );
  }

  effacerErreur(): void {
    this.erreur.set('');
  }
}
