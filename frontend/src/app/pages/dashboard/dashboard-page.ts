import { Component, inject, OnInit } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';

@Component({
selector: 'app-dashboard-page',
standalone: true,
templateUrl: './dashboard-page.html',
styleUrl: './dashboard-page.css',
})
export class DashboardPage implements OnInit {
readonly dashboardService = inject(DashboardService);

ngOnInit(): void {
this.dashboardService.charger().subscribe({
error: () => undefined,
});
}
}
