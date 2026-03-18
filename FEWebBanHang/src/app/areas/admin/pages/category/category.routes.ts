import { Routes } from '@angular/router';

export const categoryRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./category.component').then(m => m.CategoryComponent),
  },

  {
    path: 'detail/:id',
    loadComponent: () =>
      import('./detail/detail.component').then(m => m.DetailComponent),
  },
  {
    path: 'create',
    loadComponent: () =>
      import('./create/create.component').then(m => m.CreateComponent),
  },
  {
    path: 'update/:id',
    loadComponent: () =>
      import('./update/update.component').then(m => m.UpdateComponent),
  },

  
];
