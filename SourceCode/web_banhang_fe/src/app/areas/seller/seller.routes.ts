import { Routes } from "@angular/router";

export const sellerRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import('./layout/seller-layout.component').then(m => m.SellerLayoutComponent),
        children: [
            {
                path: '',
                loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.SellerDashboardComponent)
            },
            {
                path: 'products',
                loadComponent: () => import('./pages/sellerProduct/sellerProduct.component').then(m => m.SellerProductComponent)
            },
            {
                path: 'inventory',
                loadComponent: () => import('./pages/coming-soon/coming-soon.component').then(m => m.SellerComingSoonComponent)
            },
            {
                path: 'orders',
                loadComponent: () => import('./pages/coming-soon/coming-soon.component').then(m => m.SellerComingSoonComponent)
            },
            {
                path: 'revenue',
                loadComponent: () => import('./pages/coming-soon/coming-soon.component').then(m => m.SellerComingSoonComponent)
            },
            {
                path: 'chat',
                loadComponent: () => import('./pages/coming-soon/coming-soon.component').then(m => m.SellerComingSoonComponent)
            },
            {
                path: 'notification',
                loadComponent: () => import('./pages/coming-soon/coming-soon.component').then(m => m.SellerComingSoonComponent)
            },
            {
                path: 'settings',
                loadComponent: () => import('./pages/coming-soon/coming-soon.component').then(m => m.SellerComingSoonComponent)
            }
        ]
    }
]