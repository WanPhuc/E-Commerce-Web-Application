import { APP_INITIALIZER, ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { AuthService } from './core/services/auth/auth';
import { lastValueFrom } from 'rxjs';
import { credentialsInterceptor } from './core/interceptors/credentials.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        credentialsInterceptor,
        AuthInterceptor
      ])
    ),
    {
      provide: APP_INITIALIZER,
      useFactory: (authService: AuthService) => {
        return async () => {
          console.log('🚀 APP_INITIALIZER started');
          
          try {
            // ✅ Restore session
            await lastValueFrom(authService.refreshMe());
            console.log('✅ Session restored, me:', authService.me);
          } catch (error) {
            console.error('❌ APP_INITIALIZER error:', error);
          }
        };
      },
      deps: [AuthService],
      multi: true
    }
  ]
};
