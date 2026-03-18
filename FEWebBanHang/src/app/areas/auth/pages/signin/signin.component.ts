import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth/auth';
import { Router, RouterLink } from '@angular/router';
import { email } from '@angular/forms/signals';
import { finalize, switchMap } from 'rxjs';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.scss',
})
export class SigninComponent {

  constructor() {
    console.log('🔵 SigninComponent constructor');
  }

  ngOnInit() {
    console.log('🔵 SigninComponent ngOnInit');
  }

  ngOnDestroy() {
    console.log('🔴 SigninComponent ngOnDestroy - Component bị destroy!');
  }

  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  errorMsg=signal('');
  showPassword=signal(false);
  loading=signal(false);
  remember=signal(false);

  form=this.fb.group({
    email:['',[Validators.required,Validators.email]],
    password:['',[Validators.required,Validators.minLength(6)]],
    remember:[false],
  })
  // Trong component
  togglePassword() {
    this.showPassword.update(v => !v);
  }
  submit(){
    console.log('🟢 Submit called')
    this.errorMsg.set('');
    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }
    this.loading.set(true);
    const {email,password}=this.form.getRawValue();
    
    this.auth.signin(email!, password!).pipe(
    finalize(() => this.loading.set(false))
  ).subscribe({
    next: () => {
      const me = this.auth.me;
      if (me?.role === 'Admin') this.router.navigate(['/admin']);
      else this.router.navigate(['/']);
    },
    error: (err) => {
      this.errorMsg.set(err?.error?.message||err?.message || 'Đăng nhập thất bại'); // ✅
    }
  });

  }

  get f(){
    return this.form.controls;
  }
}
