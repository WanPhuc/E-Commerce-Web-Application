import { Routes } from '@angular/router';

export const sellerRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./seller.component').then(m => m.SellerComponent),
  },

  {
    path: 'seller/:id',
    loadComponent: () =>
      import('./detail/seller-detail/seller-detail.component').then(m => m.SellerDetailComponent),
  },

  {
    path: 'application-seller/:id',
    loadComponent: () =>
      import('./detail/application-seller-detail/application-seller-detail.component')
        .then(m => m.ApplicationSellerDetailComponent),
  },
];
