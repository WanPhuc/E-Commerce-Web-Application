// auth.interceptor.ts
import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);

    return next(req).pipe(
        catchError((err: HttpErrorResponse) => {
            // ✅ Chỉ log error quan trọng
            if (err.status === 401 || err.status === 403 || err.status === 500) {
                console.error('❌ HTTP Error:', {
                    status: err.status,
                    statusText: err.statusText,
                    url: req.url,
                    currentRoute: router.url
                });
            }

            // ✅ Xử lý 401 - Unauthorized
            if (err.status === 401) {
                const isAuthApi = req.url.includes('/api/v1/auth');
                const currentUrl = router.url;
                
                // Chỉ redirect nếu KHÔNG phải auth API và KHÔNG phải đang ở trang auth
                if (!isAuthApi && !currentUrl.includes('/signin') && !currentUrl.includes('/signup')) {
                    console.log('🔀 Redirecting to /signin due to 401');
                    router.navigate(['/signin'], { 
                        queryParams: { returnUrl: currentUrl } 
                    });
                }
            }

            // ✅ Xử lý 403 - Forbidden
            if (err.status === 403) {
                console.log('🚫 Access denied, redirecting to /accessdenied');
                router.navigate(['/accessdenied']);
            }

            // ✅ Xử lý 500 - Internal Server Error (Backend issue)
            if (err.status === 500) {
                console.error('🔥 Server Error 500:', err.error);
                // Không redirect, chỉ log để debug
            }

            return throwError(() => err);
        })
    );
};