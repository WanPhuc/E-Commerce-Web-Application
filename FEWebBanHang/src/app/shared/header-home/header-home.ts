import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth/auth';

@Component({
  selector: 'app-header-home',
  standalone: true,
  imports: [CommonModule,RouterLink],
  templateUrl: './header-home.html',
  styleUrl: './header-home.scss',
})
export class HeaderHome {
  menuopen=false;
  constructor(public auth:AuthService,private route : Router){
  }

  toogleMenu(){
    this.menuopen=true;
  }

  logout(){
    this.auth.signout().subscribe(()=>{
      this.route.navigate(['/']);
    })
  }

}
