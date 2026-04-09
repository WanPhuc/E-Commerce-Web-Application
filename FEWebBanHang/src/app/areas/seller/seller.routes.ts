import { SellerLayoutComponent } from './layout/seller-layout.component';
import { Routes } from "@angular/router";

export const sellerRoutes:Routes=[
    {
        path:'',
        loadComponent:()=>import('./layout/seller-layout.component').then(m=>m.SellerLayoutComponent),
        children:[
            {
                path:'',loadComponent:()=>import('./pages/dashboard/dashboard.component').then(m=>m.SellerDashboardComponent)
            },
            {
                path:'products',loadComponent:()=>import('./pages/sellerProduct/sellerProduct.component').then(m=>m.SellerProductComponent)
            }
            
        ]
    },
    
]