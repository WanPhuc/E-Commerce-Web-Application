import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth/auth.service';
import { filter } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TranslateModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit, OnDestroy {
  private router = inject(Router);
  private auth = inject(AuthService);
  loading = signal(true);
  
  
  ngOnInit(): void {
    // ✅ Theo dõi auth state
    this.auth.me$.subscribe({
      next: (me) => {
        this.loading.set(false);
        
        // ✅ CHỈ restore route nếu ĐÃ LOGIN
        if (me) {
          const lastRoute = this.auth.getAndClearLastRoute();
          if (lastRoute && this.router.url === '/') {
            console.log('🔄 Restoring to:', lastRoute);
            this.router.navigateByUrl(lastRoute);
          }
        } else {
          // ✅ Nếu KHÔNG login → xóa lastRoute
          this.auth.getAndClearLastRoute();
        }
      }
    });

    // ✅ Lưu route mỗi khi navigate
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.auth.saveCurrentRoute(event.urlAfterRedirects);
      });

    
  }

  ngOnDestroy(): void {
    
  }
}