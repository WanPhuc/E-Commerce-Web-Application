import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, of, Subject, switchMap, take, tap, throwError } from 'rxjs';

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

export interface AuthResponseDto {
  me: MeDto;
  tokens: {
    accessToken: string;
    refreshToken: string;
    accessTokenExpiresAt: string;
  };
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = `${environment.apiUrl}/auth`;
  private _me$ = new BehaviorSubject<MeDto | null>(null);
  me$ = this._me$.asObservable();

  private readonly LAST_ROUTE_KEY = 'lastRoute';
  private isRefreshing = false;
  private refreshDone$ = new Subject<boolean>();

  constructor(private http: HttpClient) {}

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
    if (route !== '/auth/signin' && 
        route !== '/auth/signup' && 
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
    
    if (route) {
      localStorage.removeItem(this.LAST_ROUTE_KEY);
    }
    
    return route;
  }
  // ✅ THÊM MỚI: Tách ra để dùng nội bộ, KHÔNG gọi HTTP
  // Lý do: nếu signout() gọi HTTP mà bị 401 → interceptor bắt → gọi refresh → vòng lặp vô tận
  clearSession(): void {
    this._me$.next(null);
    localStorage.removeItem(this.LAST_ROUTE_KEY);
    // Reset trạng thái refresh để lần sau còn dùng được
    this.isRefreshing = false;
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
      .post<ApiResponse<AuthResponseDto>>(
        `${this.baseUrl}/signin`,
        { email, password },
        { withCredentials: true }
      )
      .pipe(
        map(res => res.data.me),
        tap(me => this._me$.next(me))
      );
  }

  /** ✅ Signup */
  signup(fullName: string, email: string, password: string) {
    return this.http
      .post<ApiResponse<AuthResponseDto>>(
        `${this.baseUrl}/signup`,
        { fullName, email, password },
        { withCredentials: true }
      )
      .pipe(
        map(res => res.data.me),
        tap(me => this._me$.next(me))
      );
  }

  signout() {
    return this.http
      .post<ApiResponse<null>>(`${this.baseUrl}/logout`, {}, { withCredentials: true })
      .pipe(
        tap(() => {
          this.clearSession();
        })
      );
  }
  refreshToken() {
  console.log('🔄 Requesting new tokens...');
  if(this.isRefreshing){
    console.log('⏳ Refresh already in progress, waiting...');
    return this.refreshDone$.pipe(
    take(1),
    switchMap(success => {
        if (success) return of(void 0);      // request đang chờ sẽ retry
        return throwError(() => new Error('Refresh failed'));
      })
    );
  }
  this.isRefreshing = true;
  console.log('🚀 Starting token refresh process...');
  return this.http
    .post<ApiResponse<any>>(
      `${this.baseUrl}/refresh-token`, 
      {}, // Body rỗng vì BE lấy token từ Cookie
      { withCredentials: true }
    )
    .pipe(
      map(()=>void 0),
      tap((res) => {
        console.log('✨ Tokens refreshed successfully');
        this.isRefreshing = false;
        this.refreshDone$.next(true);
      }),
      catchError((err) => {
        console.error('❌ Refresh token failed', err);
        this.clearSession();
        this.refreshDone$.next(false);
        return throwError(() => err);
      })
    );
}
}
