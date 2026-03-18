import { inject } from "@angular/core";
import { CanActivate, CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../services/auth/auth";
import { catchError, map, of, switchMap, take } from "rxjs";

export const roleGuard=(roles:String[]):CanActivateFn=>{
    return ()=>{
        const auth=inject(AuthService);
        const route=inject(Router);

        return auth.me$.pipe(
            take(1),
            switchMap(me=>(me?of(me):auth.refreshMe().pipe(take(1)))),
            map(me=>{
                    if(me && roles.includes(me.role)){
                        return true;
                    }
                        
                    route.navigateByUrl('/accessdenied');
                    return false;
                    
                }
            ),catchError(()=>{
                route.navigateByUrl('/signin');
                return of(false);
            })
        )

    }
}