import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth/auth';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit, OnDestroy {
  private router = inject(Router);
  private auth = inject(AuthService);
  loading = signal(true);
  
  private saveInterval?: number;
  
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

    // ✅ LƯU ROUTE MỖI 2 GIÂY (để catch được hot reload)
    this.saveInterval = window.setInterval(() => {
      const currentUrl = this.router.url;
      if (currentUrl && currentUrl !== '/') {
        this.auth.saveCurrentRoute(currentUrl);
      }
    }, 1000);
  }

  ngOnDestroy(): void {
    if (this.saveInterval) {
      clearInterval(this.saveInterval);
    }
  }
}