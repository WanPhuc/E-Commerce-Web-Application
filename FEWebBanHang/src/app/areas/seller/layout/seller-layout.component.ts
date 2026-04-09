import { NotificationDto } from './../../../core/models/publish/notification.model';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref, Router, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth/auth';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../../shared/global/notification/notification.service';
import { NotificationType } from '../../../shared/types/enum/NotificationType';
import { TimeAgoPipe } from '../../../shared/pipes/timeAgoPipe.pipe';

@Component({
  selector: 'app-seller-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLinkWithHref, CommonModule, RouterLinkActive,TimeAgoPipe],
  templateUrl: './seller-layout.component.html',
  styleUrl: './seller-layout.component.scss',
})
export class SellerLayoutComponent implements OnInit{
  public auth = inject(AuthService);
  private router=inject(Router);
  private notiService = inject(NotificationService);
  protected notiType=NotificationType;

  isSidebarOpen = signal(true);
  unreadCount = signal(0);
  loading =signal(false);

  notificationsList=signal<NotificationDto[]>([]);

  ngOnInit(): void {
      this.notiService.getUnreadNotificationCount().subscribe({
        next: res=>{
          this.unreadCount.set(res.unreadCount);
        }
      })
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
