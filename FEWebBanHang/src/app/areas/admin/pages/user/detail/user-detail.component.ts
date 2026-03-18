import { ToastService } from './../../../../../shared/toast/toast.service';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { UserService } from '../../../../../core/services/admin/user/user';
import { UserDto } from '../../../../../core/models/admin/user.model';
import { CommonModule } from '@angular/common';
import { StatusPipe } from '../../../../../shared/pipes/status.pipe';

@Component({
  selector: 'app-user-detail',
  imports: [RouterLink,CommonModule,StatusPipe],
  templateUrl: './user-detail.component.html',
  styleUrl: './user-detail.component.scss',
})
export class UserDetailComponent implements OnInit {
  private route =inject(ActivatedRoute);
  private router=inject(Router);
  private userService=inject(UserService);
  private toast=inject(ToastService);

  loading=signal(true);
  userDetail =signal<UserDto | null>(null);

  ngOnInit(): void {
    const id=this.route.snapshot.paramMap.get('id');
    if(id){
      this.loadDetail(id);
    }else{
      this.router.navigate(['/admin/user']);
    }

  }
  loadDetail(id:string){
    this.loading.set(true);
    this.userService.getUserById(id).subscribe({
      next:(data)=>{
        this.userDetail.set(data);
        this.loading.set(false);
      },error:(err)=>{
        this.toast.show(err?.error?.message||'Không thể tải dữ liệu','error');
        this.loading.set(false);
      }
    });
  }
}
