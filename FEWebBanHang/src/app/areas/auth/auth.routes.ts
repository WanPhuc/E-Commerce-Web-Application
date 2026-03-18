import { Component } from '@angular/core';
import { Routes } from '@angular/router';

export const authRoutes: Routes = [
    {path:'',
        loadComponent:()=>
            import('./layout/auth-layout/auth-layout.component').then(m => m.AuthLayoutComponent),
        children:[
            {
                path:'signin',
                loadComponent:()=>
                    import('./pages/signin/signin.component').then(m=>m.SigninComponent)
            },
            {
                path:'signup',
                loadComponent:()=>
                    import('./pages/signup/signup.component').then(m=>m.SignupComponent)
            },
            {
                path:'accessdenied',
                loadComponent:()=>
                    import('./pages/access-denied/accessdenied.component').then(m=>m.AccessDeniedComponent)
            }
        ]
    }
];
