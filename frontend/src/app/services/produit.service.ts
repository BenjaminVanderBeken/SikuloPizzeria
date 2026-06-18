import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import {
CreateProduitDto,
Produit,
UpdateProduitDto,
} from '../models/produit.model';

@Injectable({
providedIn: 'root',
})
export class ProduitService {
private readonly http = inject(HttpClient);
private readonly apiUrl = `${environment.apiUrl}/produits`;

private readonly _produits = signal<Produit[]>([]);
private readonly _chargement = signal(false);
private readonly _erreur = signal('');
private readonly _produitEnEditionId = signal<number | null>(null);

readonly produits = this._produits.asReadonly();
readonly chargement = this._chargement.asReadonly();
readonly erreur = this._erreur.asReadonly();
readonly produitEnEditionId =
this._produitEnEditionId.asReadonly();

readonly produitsActifs = computed(() =>
this._produits().filter((produit) => produit.actif),
);

readonly nombreProduitsActifs = computed(
() => this.produitsActifs().length,
);

readonly produitEnEdition = computed(() => {
const id = this._produitEnEditionId();


if (id === null) {
  return null;
}

return (
  this._produits().find((produit) => produit.id === id) ?? null
);


});

charger(): Observable<Produit[]> {
this._chargement.set(true);
this._erreur.set('');


return this.http.get<Produit[]>(this.apiUrl).pipe(
  tap((produits) => {
    this._produits.set(produits);
  }),
  catchError((erreur: HttpErrorResponse) => {
    this._erreur.set(this.extraireMessage(erreur));
    return throwError(() => erreur);
  }),
  finalize(() => {
    this._chargement.set(false);
  }),
);


}

obtenirParId(id: number): Observable<Produit> {
this._chargement.set(true);
this._erreur.set('');


return this.http.get<Produit>(`${this.apiUrl}/${id}`).pipe(
  catchError((erreur: HttpErrorResponse) => {
    this._erreur.set(this.extraireMessage(erreur));
    return throwError(() => erreur);
  }),
  finalize(() => {
    this._chargement.set(false);
  }),
);


}

creer(dto: CreateProduitDto): Observable<Produit> {
this._chargement.set(true);
this._erreur.set('');


return this.http.post<Produit>(this.apiUrl, dto).pipe(
  tap((produitCree) => {
    this._produits.update((produits) => [
      ...produits,
      produitCree,
    ]);
  }),
  catchError((erreur: HttpErrorResponse) => {
    this._erreur.set(this.extraireMessage(erreur));
    return throwError(() => erreur);
  }),
  finalize(() => {
    this._chargement.set(false);
  }),
);


}

modifier(
id: number,
dto: UpdateProduitDto,
): Observable<Produit> {
this._chargement.set(true);
this._erreur.set('');


return this.http
  .put<Produit>(`${this.apiUrl}/${id}`, dto)
  .pipe(
    tap((produitModifie) => {
      this._produits.update((produits) =>
        produits.map((produit) =>
          produit.id === id ? produitModifie : produit,
        ),
      );

      this._produitEnEditionId.set(null);
    }),
    catchError((erreur: HttpErrorResponse) => {
      this._erreur.set(this.extraireMessage(erreur));
      return throwError(() => erreur);
    }),
    finalize(() => {
      this._chargement.set(false);
    }),
  );


}

desactiver(id: number): Observable<void> {
this._chargement.set(true);
this._erreur.set('');


return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
  tap(() => {
    this._produits.update((produits) =>
      produits.map((produit) =>
        produit.id === id
          ? { ...produit, actif: false }
          : produit,
      ),
    );

    if (this._produitEnEditionId() === id) {
      this._produitEnEditionId.set(null);
    }
  }),
  catchError((erreur: HttpErrorResponse) => {
    this._erreur.set(this.extraireMessage(erreur));
    return throwError(() => erreur);
  }),
  finalize(() => {
    this._chargement.set(false);
  }),
);


}

reactiver(id: number): Observable<void> {
this._chargement.set(true);
this._erreur.set('');


return this.http
  .patch<void>(`${this.apiUrl}/${id}/reactiver`, {})
  .pipe(
    tap(() => {
      this._produits.update((produits) =>
        produits.map((produit) =>
          produit.id === id
            ? { ...produit, actif: true }
            : produit,
        ),
      );
    }),
    catchError((erreur: HttpErrorResponse) => {
      this._erreur.set(this.extraireMessage(erreur));
      return throwError(() => erreur);
    }),
    finalize(() => {
      this._chargement.set(false);
    }),
  );


}

supprimerDefinitivement(id: number): Observable<void> {
this._chargement.set(true);
this._erreur.set('');


return this.http
  .delete<void>(`${this.apiUrl}/${id}/definitif`)
  .pipe(
    tap(() => {
      this._produits.update((produits) =>
        produits.filter((produit) => produit.id !== id),
      );

      if (this._produitEnEditionId() === id) {
        this._produitEnEditionId.set(null);
      }
    }),
    catchError((erreur: HttpErrorResponse) => {
      this._erreur.set(this.extraireMessage(erreur));
      return throwError(() => erreur);
    }),
    finalize(() => {
      this._chargement.set(false);
    }),
  );


}

selectionnerPourEdition(id: number): void {
this._produitEnEditionId.set(id);
}

annulerEdition(): void {
this._produitEnEditionId.set(null);
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
