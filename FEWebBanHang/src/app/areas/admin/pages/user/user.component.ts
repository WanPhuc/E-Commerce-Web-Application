import { ToastService } from './../../../../shared/toast/toast.service';
import { Component, inject, OnInit, signal } from '@angular/core';
import { UserDto } from '../../../../core/models/admin/user.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UserService } from '../../../../core/services/admin/user/user';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [CommonModule,RouterLink,],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserComponent implements OnInit{
  private userService=inject(UserService);
  private toast =inject(ToastService);
  loading=signal(true);
  users=signal<UserDto[]>([]);

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
        this.toast.show(err?.error?.message||'Không thể tải dữ liệu','error');
        this.loading.set(false);
      }
    }) 
  }
  //createUser()
}
