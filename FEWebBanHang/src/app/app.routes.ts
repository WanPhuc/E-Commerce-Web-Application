import { Routes } from '@angular/router';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
    {path:'',
        loadChildren:()=>import('./areas/publish/publish.routes').then(m=>m.publishRoutes)
    },
    {path:'auth',
        loadChildren:()=>import('./areas/auth/auth.routes').then(m=>m.authRoutes)
    },
    {path:'admin',canActivate:[roleGuard(['Admin'])],
        loadChildren:()=>import('./areas/admin/admin.routes').then(m=>m.adminRoutes)
    }
];
