import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import {
Client,
CreateClientDto,
UpdateClientDto,
} from '../models/client.model';

@Injectable({
providedIn: 'root',
})
export class ClientService {
private readonly http = inject(HttpClient);
private readonly apiUrl = `${environment.apiUrl}/clients`;

private readonly _clients = signal<Client[]>([]);
private readonly _chargement = signal(false);
private readonly _erreur = signal('');
private readonly _clientEnEditionId = signal<number | null>(null);

readonly clients = this._clients.asReadonly();
readonly chargement = this._chargement.asReadonly();
readonly erreur = this._erreur.asReadonly();
readonly clientEnEditionId = this._clientEnEditionId.asReadonly();

readonly clientsActifs = computed(() =>
this._clients().filter((client) => client.actif),
);

readonly nombreClientsActifs = computed(
() => this.clientsActifs().length,
);

readonly clientEnEdition = computed(() => {
const id = this._clientEnEditionId();


if (id === null) {
  return null;
}

return this._clients().find((client) => client.id === id) ?? null;


});

charger(): Observable<Client[]> {
this._chargement.set(true);
this._erreur.set('');


return this.http.get<Client[]>(this.apiUrl).pipe(
  tap((clients) => this._clients.set(clients)),
  catchError((erreur: HttpErrorResponse) => {
    this._erreur.set(this.extraireMessage(erreur));
    return throwError(() => erreur);
  }),
  finalize(() => this._chargement.set(false)),
);


}

creer(dto: CreateClientDto): Observable<Client> {
this._chargement.set(true);
this._erreur.set('');


return this.http.post<Client>(this.apiUrl, dto).pipe(
  tap((clientCree) => {
    this._clients.update((clients) =>
      [...clients, clientCree].sort((a, b) =>
        a.nom.localeCompare(b.nom),
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

modifier(id: number, dto: UpdateClientDto): Observable<Client> {
this._chargement.set(true);
this._erreur.set('');


return this.http.put<Client>(`${this.apiUrl}/${id}`, dto).pipe(
  tap((clientModifie) => {
    this._clients.update((clients) =>
      clients
        .map((client) =>
          client.id === id ? clientModifie : client,
        )
        .sort((a, b) => a.nom.localeCompare(b.nom)),
    );

    this._clientEnEditionId.set(null);
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
    this._clients.update((clients) =>
      clients.map((client) =>
        client.id === id
          ? { ...client, actif: false }
          : client,
      ),
    );

    if (this._clientEnEditionId() === id) {
      this._clientEnEditionId.set(null);
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
    this._clients.update((clients) =>
      clients.map((client) =>
        client.id === id
          ? { ...client, actif: true }
          : client,
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
    this._clients.update((clients) =>
      clients.filter((client) => client.id !== id),
    );

    if (this._clientEnEditionId() === id) {
      this._clientEnEditionId.set(null);
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
this._clientEnEditionId.set(id);
}

annulerEdition(): void {
this._clientEnEditionId.set(null);
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
