import { Component } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth/auth';
import { CommonModule } from '@angular/common';
import { Toast } from '../../../shared/toast/toast.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLinkWithHref,CommonModule,Toast],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
})
export class AdminLayoutComponent {
  constructor(public auth:AuthService,private route : Router){
  }

  

  logout(){
    this.auth.signout().subscribe(()=>{
      this.route.navigate(['/']);
    })
  }

}
