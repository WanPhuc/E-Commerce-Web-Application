import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth/auth.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-footer-home',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './footer-home.html',
  styleUrl: './footer-home.scss',
})
export class FooterHome {
  menuopen=false;
  constructor(public auth:AuthService,private route : Router){}

  toogleMenu(){
    this.menuopen=true;
  }

  logout(){
    this.auth.signout().subscribe(()=>{
      this.route.navigate(['/signin']);
    })
  }

}
