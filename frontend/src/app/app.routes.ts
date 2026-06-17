import { Routes } from '@angular/router';
import { CategoriesPage } from './pages/categories/categories-page';
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
    path: '',
    redirectTo: 'categories',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'categories',
  },
];