import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, of, switchMap, tap } from 'rxjs';

export interface MeDto {
  id: string;
  email: string;
  fullName: string;
  role: string;
}

export interface ApiResponse<T> {
  status: number;
  message: string;
  data: T;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'http://localhost:5144/api/v1/auth';
  private _me$ = new BehaviorSubject<MeDto | null>(null);
  me$ = this._me$.asObservable();

  private readonly LAST_ROUTE_KEY = 'lastRoute';

  constructor(private http: HttpClient) {
    console.log('🔧 AuthService constructed');
  }

  get me(): MeDto | null {
    return this._me$.value;
  }

  get isAuthenticated(): boolean {
    return !!this._me$.value;
  }

  /** ✅ Lưu route hiện tại */
  saveCurrentRoute(route: string): void {
    // ✅ Chỉ bỏ qua trang auth, KHÔNG check isAuthenticated
    // Vì có thể session đang được restore và chưa set me$
    if (route !== '/signin' && 
        route !== '/signup' && 
        route !== '/' &&
        !route.includes('returnUrl')) {
      
      console.log('💾 Saving route to localStorage:', route);
      localStorage.setItem(this.LAST_ROUTE_KEY, route);
    } else {
      console.log('⏭️ Skipping route save:', route);
    }
  }

  /** ✅ Lấy route đã lưu */
  getAndClearLastRoute(): string | null {
    const route = localStorage.getItem(this.LAST_ROUTE_KEY);
    console.log('📖 Retrieved lastRoute from localStorage:', route);
    
    if (route) {
      localStorage.removeItem(this.LAST_ROUTE_KEY);
      console.log('🗑️ Cleared lastRoute from localStorage');
    }
    
    return route;
  }

  /** ✅ Lấy /me và update BehaviorSubject */
  refreshMe() {
    console.log('🔄 Calling /me API...');
    return this.http
      .get<ApiResponse<MeDto>>(`${this.baseUrl}/me`, { withCredentials: true })
      .pipe(
        map(res => res.data),
        tap(me => {
          console.log('✅ /me success:', me);
          this._me$.next(me);
        }),
        catchError((err) => {
          console.log('❌ /me failed:', err.status);
          this._me$.next(null);
          localStorage.removeItem(this.LAST_ROUTE_KEY);
          return of(null);
        })
      );
  }

  /** ✅ Signin */
  signin(email: string, password: string) {
    return this.http
      .post<ApiResponse<MeDto>>(
        `${this.baseUrl}/signin`,
        { email, password },
        { withCredentials: true }
      )
      .pipe(
        map(res => res.data),
        tap(me => this._me$.next(me))
      );
  }

  /** ✅ Signup */
  signup(fullName: string, email: string, password: string) {
    return this.http
      .post<ApiResponse<MeDto>>(
        `${this.baseUrl}/signup`,
        { fullName, email, password },
        { withCredentials: true }
      )
      .pipe(
        map(res => res.data),
        tap(me => this._me$.next(me))
      );
  }

  signout() {
    return this.http
      .post<ApiResponse<null>>(`${this.baseUrl}/signout`, {}, { withCredentials: true })
      .pipe(
        tap(() => {
          this._me$.next(null);
          localStorage.removeItem(this.LAST_ROUTE_KEY);
          console.log('👋 Signed out, cleared lastRoute');
        })
      );
  }
}