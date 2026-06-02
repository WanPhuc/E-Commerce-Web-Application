import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth/auth.service';
import { LanguageService } from '../../core/services/i18n/language.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-header-home',
  standalone: true,
  imports: [CommonModule,RouterLink,TranslateModule],
  templateUrl: './header-home.html',
  styleUrl: './header-home.scss',
})
export class HeaderHome implements OnInit {
  menuopen=false;
  currentLanguage = signal<'en' | 'vi' | 'jp'>('en');

  constructor(
    public auth: AuthService,
    private route: Router,
    private languageService: LanguageService
  ){
  }

  toogleMenu(){
    this.menuopen=true;
  }

  ngOnInit(): void {
    // Đồng bộ nhãn ngôn ngữ trên header với ngôn ngữ đang dùng trong app.
    this.currentLanguage.set(this.languageService.getCurrentLanguage());
  }

  async changeLanguage(language: 'en' | 'vi' | 'jp'): Promise<void> {
    // Đổi ngôn ngữ xong thì cập nhật luôn nút hiển thị cho người dùng thấy.
    await this.languageService.setLanguage(language);
    this.currentLanguage.set(language);
  }

  get languageLabel(): string {
    const language = this.currentLanguage();
    return language === 'vi' ? 'VI' : language === 'jp' ? 'JP' : 'EN';
  }

  logout(){
    this.auth.signout().subscribe(()=>{
      this.route.navigate(['/']);
    })
  }

}
