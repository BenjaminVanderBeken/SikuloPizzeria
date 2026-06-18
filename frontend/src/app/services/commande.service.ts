import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import {
Commande,
CreateCommandeDto,
UpdateCommandePaymentDto,
UpdateCommandeStatusDto,
} from '../models/commande.model';

@Injectable({
providedIn: 'root',
})
export class CommandeService {
private readonly http = inject(HttpClient);
private readonly apiUrl = `${environment.apiUrl}/commandes`;

private readonly _commandes = signal<Commande[]>([]);
private readonly _chargement = signal(false);
private readonly _erreur = signal('');

readonly commandes = this._commandes.asReadonly();
readonly chargement = this._chargement.asReadonly();
readonly erreur = this._erreur.asReadonly();

readonly nombreCommandes = computed(() => this._commandes().length);

readonly chiffreAffaires = computed(() =>
this._commandes()
.filter((commande) => commande.statutPaiement === 'PAYEE')
.reduce((total, commande) => total + commande.montantTotal, 0),
);

charger(): Observable<Commande[]> {
this._chargement.set(true);
this._erreur.set('');


return this.http.get<Commande[]>(this.apiUrl).pipe(
  tap((commandes) => this._commandes.set(commandes)),
  catchError((erreur: HttpErrorResponse) => {
    this._erreur.set(this.extraireMessage(erreur));
    return throwError(() => erreur);
  }),
  finalize(() => this._chargement.set(false)),
);


}

obtenirParId(id: number): Observable<Commande> {
this._chargement.set(true);
this._erreur.set('');


return this.http.get<Commande>(`${this.apiUrl}/${id}`).pipe(
  catchError((erreur: HttpErrorResponse) => {
    this._erreur.set(this.extraireMessage(erreur));
    return throwError(() => erreur);
  }),
  finalize(() => this._chargement.set(false)),
);


}

creer(dto: CreateCommandeDto): Observable<Commande> {
this._chargement.set(true);
this._erreur.set('');


return this.http.post<Commande>(this.apiUrl, dto).pipe(
  tap((commandeCreee) => {
    this._commandes.update((commandes) => [
      commandeCreee,
      ...commandes,
    ]);
  }),
  catchError((erreur: HttpErrorResponse) => {
    this._erreur.set(this.extraireMessage(erreur));
    return throwError(() => erreur);
  }),
  finalize(() => this._chargement.set(false)),
);


}

modifierStatut(
id: number,
dto: UpdateCommandeStatusDto,
): Observable<void> {
this._chargement.set(true);
this._erreur.set('');


return this.http.patch<void>(`${this.apiUrl}/${id}/statut`, dto).pipe(
  tap(() => {
    this._commandes.update((commandes) =>
      commandes.map((commande) =>
        commande.id === id
          ? { ...commande, statut: dto.statut }
          : commande,
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

modifierPaiement(
id: number,
dto: UpdateCommandePaymentDto,
): Observable<void> {
this._chargement.set(true);
this._erreur.set('');


return this.http.patch<void>(`${this.apiUrl}/${id}/paiement`, dto).pipe(
  tap(() => {
    this._commandes.update((commandes) =>
      commandes.map((commande) =>
        commande.id === id
          ? {
              ...commande,
              modePaiement: dto.modePaiement,
              statutPaiement: dto.statutPaiement,
            }
          : commande,
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
