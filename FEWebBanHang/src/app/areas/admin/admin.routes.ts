import { roleGuard } from './../../core/guards/role.guard';
import { Component } from '@angular/core';
import { Routes } from '@angular/router';

export const adminRoutes: Routes = [
    {path:'',canActivate:[roleGuard(['Admin'])],
        loadComponent:()=>
            import('./layout/admin-layout.component').then(m => m.AdminLayoutComponent),
        children:[
            {
                path:'',
                loadComponent:()=>
                    import('./pages/dashboard/dashboard.component').then(m=>m.DashboardComponent)
            },
            {
                path:'seller',
                loadChildren:()=>
                    import('./pages/seller/seller.routes').then(m=>m.sellerRoutes),
            },
            {
                path:'user',
                loadChildren:()=>
                    import('./pages/user/user.routes').then(m=>m.userRoutes),
            },
            {
                path:'category',
                loadChildren:()=>
                    import('./pages/category/category.routes').then(m=>m.categoryRoutes),
            },
           
        ]
    }
];