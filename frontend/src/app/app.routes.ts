import { Routes } from '@angular/router';
import { CategoriesPage } from './pages/categories/categories-page';
import { ClientsPage } from './pages/clients/clients-page';
import { CommandesPage } from './pages/commandes/commandes-page';
import { CompositionsPage } from './pages/compositions/compositions-page';
import { DashboardPage } from './pages/dashboard/dashboard-page';
import { IngredientsPage } from './pages/ingredients/ingredients-page';
import { ProduitsPage } from './pages/produits/produits-page';

export const routes: Routes = [
{
path: 'dashboard',
component: DashboardPage,
},
{
path: 'categories',
component: CategoriesPage,
},
{
path: 'produits',
component: ProduitsPage,
},
{
path: 'ingredients',
component: IngredientsPage,
},
{
path: 'compositions',
component: CompositionsPage,
},
{
path: 'clients',
component: ClientsPage,
},
{
path: 'commandes',
component: CommandesPage,
},
{
path: '',
redirectTo: 'dashboard',
pathMatch: 'full',
},
{
path: '**',
redirectTo: 'dashboard',
},
];
