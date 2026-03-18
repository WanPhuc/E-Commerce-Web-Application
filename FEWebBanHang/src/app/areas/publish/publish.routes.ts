import { Component } from '@angular/core';
import { Routes } from '@angular/router';

export const publishRoutes: Routes = [
    {path:'',
        loadComponent:()=>
            import('./layout/publish-layout.component').then(m => m.PublishLayoutComponent),
        children:[
            {
                path:'',
                loadComponent:()=>
                    import('./pages/home/home.component').then(m=>m.HomeComponent)
            },
            
        ]
    }
];
