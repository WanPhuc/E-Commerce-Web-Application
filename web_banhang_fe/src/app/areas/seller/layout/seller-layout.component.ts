import { NotificationDto } from './../../../core/models/publish/notification.model';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref, Router, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth/auth.service';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../../shared/global/notification/notification.service';
import { NotificationType } from '../../../shared/types/enum/NotificationType';
import { TimeAgoPipe } from '../../../shared/pipes/timeAgoPipe.pipe';
import { TranslateModule } from '@ngx-translate/core';
import { LanguageService } from '../../../core/services/i18n/language.service';

@Component({
  selector: 'app-seller-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLinkWithHref, CommonModule, RouterLinkActive,TimeAgoPipe,TranslateModule],
  templateUrl: './seller-layout.component.html',
  styleUrl: './seller-layout.component.scss',
})
export class SellerLayoutComponent implements OnInit{
  public auth = inject(AuthService);
  private router=inject(Router);
  private notiService = inject(NotificationService);
  private languageService = inject(LanguageService);
  protected notiType=NotificationType;

  isSidebarOpen = signal(true);
  unreadCount = signal(0);
  loading =signal(false);
  currentLanguage = signal<'en' | 'vi' | 'jp'>('en');

  notificationsList=signal<NotificationDto[]>([]);

  ngOnInit(): void {
      this.currentLanguage.set(this.languageService.getCurrentLanguage());
      this.notiService.getUnreadNotificationCount().subscribe({
        next: res=>{
          this.unreadCount.set(res.unreadCount);
        }
      })
  }

  async changeLanguage(language: 'en' | 'vi' | 'jp'): Promise<void> {
    await this.languageService.setLanguage(language);
    this.currentLanguage.set(language);
  }

  get languageLabel(): string {
    const language = this.currentLanguage();
    return language === 'vi' ? 'VI' : language === 'jp' ? 'JP' : 'EN';
  }

  toggleSidebar(){
    this.isSidebarOpen.update(v=>!v);
  }
  loadNotifications(){
    this.loading.set(true);
    this.notiService.getNotifications().subscribe({
      next:res=>{
        this.notificationsList.set(res);
        this.loading.set(false);
      },error:err=>{
        this.loading.set(false);
      }
    })
  }
  logout(){
    this.auth.signout().subscribe(()=>{
          this.router.navigate(['/']);
    });
  }
  
}
