// auth.interceptor.ts
import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, switchMap, throwError } from "rxjs";
import { AuthService } from "../services/auth/auth.service";

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);
    const authService = inject(AuthService);
    const AUTH_URLS = ['/auth/signin', '/auth/signup', '/auth/refresh-token', '/auth/logout','/auth/me'];


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
                // ✅ SỬA: Dùng mảng AUTH_URLS thay vì check từng URL riêng lẻ
                // Lý do: nếu các request auth bị 401 mà vẫn cố refresh → vòng lặp vô tận
                const isAuthCall = AUTH_URLS.some(path => req.url.includes(path));
                if (isAuthCall) {
                // Nếu /signin, /refresh-token... bị 401 thì hết cách rồi, clear session và dừng
                authService.clearSession(); // ✅ Dùng clearSession() thay vì signout()
                return throwError(() => err);
                }

                console.log('🔑 Access token hết hạn, thử refresh...');

                return authService.refreshToken().pipe(
                switchMap(() => {
                    console.log('✨ Refresh xong, retry:', req.url);
                    return next(req); 
                }),
                catchError((refreshErr) => {
                    console.error('💀 Refresh thất bại, về trang đăng nhập');
                    const currentUrl = router.url;
                    if (!currentUrl.includes('/auth/signin') && !currentUrl.includes('/auth/signup')) {
                    
                    router.navigate(['/auth/signin'], { queryParams: { returnUrl: currentUrl } });
                    }
                    return throwError(() => refreshErr);
                })
                );
            }

            // ✅ Xử lý 403 - Forbidden
            if (err.status === 403) {
                console.log('🚫 Access denied, redirecting to /accessdenied');
                router.navigate(['/auth/accessdenied']);
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