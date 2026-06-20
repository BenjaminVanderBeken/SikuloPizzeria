import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { DashboardStats } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/dashboard/stats`;

  readonly stats = signal<DashboardStats | null>(null);
  readonly chargement = signal<boolean>(false);
  readonly erreur = signal<string>('');

  readonly chiffreAffairesFormate = computed(() => {
    const stats = this.stats();

    if (stats === null) {
      return '0.00 EUR';
    }

    return `${stats.chiffreAffairesPaye.toFixed(2)} EUR`;
  });

  charger(): Observable<DashboardStats> {
    this.chargement.set(true);
    this.erreur.set('');

    return this.http.get<DashboardStats>(this.apiUrl).pipe(
      tap({
        next: (stats) => {
          this.stats.set(stats);
          this.chargement.set(false);
        },
        error: () => {
          this.erreur.set('Impossible de charger les statistiques.');
          this.chargement.set(false);
        },
      }),
    );
  }
}
