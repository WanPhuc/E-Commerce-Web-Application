import { ToastService } from './../../../../shared/toast/toast.service';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { UserDto } from '../../../../core/models/admin/user.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UserService } from '../../../../core/services/admin/user/user';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [CommonModule,RouterLink,TranslateModule],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserComponent implements OnInit{
  private userService=inject(UserService);
  private toast =inject(ToastService);
  private translate = inject(TranslateService);
  loading=signal(true);
  users=signal<UserDto[]>([]);
  totalUsers = computed(() => this.users().length);
  activeUsers = computed(() => this.users().filter(user => user.isActive).length);
  sellerUsers = computed(() => this.users().filter(user => !!user.seller).length);
  blockedUsers = computed(() => this.totalUsers() - this.activeUsers());

  ngOnInit(): void {
    this.loadData();
  }
  loadData(){
    this.loading.set(true);
    this.userService.getAllUsers().subscribe({
      next:(data)=>{
        this.users.set(data);
        this.loading.set(false);
      },error:(err)=>{
        // Nếu BE chưa trả message đã dịch thì dùng fallback theo ngôn ngữ hiện tại.
        this.toast.show(err?.error?.message||this.translate.instant('ui.loadDataFailed'),'error');
        this.loading.set(false);
      }
    }) 
  }
  //createUser()
}
