import { Routes } from '@angular/router';
import { CategoriesPage } from './pages/categories/categories-page';
import { ClientsPage } from './pages/clients/clients-page';
import { CommandesPage } from './pages/commandes/commandes-page';
import { IngredientsPage } from './pages/ingredients/ingredients-page';
import { ProduitsPage } from './pages/produits/produits-page';

export const routes: Routes = [
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
path: 'clients',
component: ClientsPage,
},
{
path: 'commandes',
component: CommandesPage,
},
{
path: '',
redirectTo: 'categories',
pathMatch: 'full',
},
{
path: '**',
redirectTo: 'categories',
},
];
