import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth/auth.service';
import { CommonModule } from '@angular/common';
import { Toast } from '../../../shared/toast/toast.component';
import { TranslateModule } from '@ngx-translate/core';
import { LanguageService } from '../../../core/services/i18n/language.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLinkWithHref,CommonModule,Toast,TranslateModule],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
})
export class AdminLayoutComponent implements OnInit {
  isSidebarCollapsed = signal(false);
  currentLanguage = signal<'en' | 'vi' | 'jp'>('en');

  constructor(
    public auth: AuthService,
    private route: Router,
    private languageService: LanguageService
  ) {
  }

  ngOnInit(): void {
    this.currentLanguage.set(this.languageService.getCurrentLanguage());
  }

  toggleSidebar(): void {
    this.isSidebarCollapsed.update((value) => !value);
  }

  async changeLanguage(language: 'en' | 'vi' | 'jp'): Promise<void> {
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
