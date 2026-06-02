import { Routes } from '@angular/router';

export const userRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./user.component').then(m => m.UserComponent),
  },

  {
    path: 'detail/:id',
    loadComponent: () =>
      import('./detail/user-detail.component').then(m => m.UserDetailComponent),
  },

  
];
