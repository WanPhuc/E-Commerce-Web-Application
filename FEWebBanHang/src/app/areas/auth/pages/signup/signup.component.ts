import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from "@angular/forms";
import { AuthService } from '../../../../core/services/auth/auth';
import { Router, RouterLink } from '@angular/router';
import { email } from '@angular/forms/signals';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-signup',
  imports: [FormsModule,CommonModule,ReactiveFormsModule,RouterLink],
  standalone: true,
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss',
})
export class SignupComponent {
  private fb=inject(FormBuilder);
  private auth=inject(AuthService);
  private router =inject(Router);

  errorMsg=signal('');
  loading=signal(false);
  showPassword=signal(false);
  showConfirmPassword=signal(false);
  form=this.fb.group({
    name:this.fb.nonNullable.control('',Validators.required),
    email:this.fb.nonNullable.control('',[Validators.required,Validators.email]),
    password:this.fb.nonNullable.control('',[Validators.required,Validators.minLength(6)]),
    confirmPassword:this.fb.nonNullable.control
    ('',Validators.required),
    },
  { validators:[matchPassword('password','confirmPassword')] })

  togglepassword(){
    this.showPassword.update(v=>!v);
  }
  toggleconfirmpassword(){
    this.showConfirmPassword.update(v=>!v);
  }

  submit(){
    this.errorMsg.set('');
    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }
    this.loading.set(true);
    const {name,email,password}=this.form.getRawValue();
    this.auth.signup(name!,email!,password!).pipe(
      finalize(()=>this.loading.set(false))
    ).subscribe({
      next:()=>{
        const me=this.auth.me;
        if(me?.role==='admin') this.router.navigate(['/admin']);
        else this.router.navigate(['/']);
      },
      error:(err)=>{
        this.errorMsg.set(err?.error?.message||err?.message || 'Đăng ký thất bại');
      }
    })
  }

  get f(){
    return this.form.controls;
  }
  
}
  export function matchPassword(
  passKey: string,
  confirmKey: string
): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const pass = group.get(passKey)?.value;
    const confirm = group.get(confirmKey)?.value;

    if (!pass || !confirm) return null;

    const confirmControl = group.get(confirmKey);
    if (pass !== confirm) {
      confirmControl?.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    } else {
      // Xóa lỗi nếu khớp
      const errors = confirmControl?.errors;
      if (errors) {
        delete errors['passwordMismatch'];
        confirmControl?.setErrors(Object.keys(errors).length ? errors : null);
      }
      return null;
    }
  };
}
