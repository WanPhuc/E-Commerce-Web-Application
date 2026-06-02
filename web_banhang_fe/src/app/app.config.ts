import { APP_INITIALIZER, ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { AuthService } from './core/services/auth/auth.service';
import { lastValueFrom } from 'rxjs';
import { credentialsInterceptor } from './core/interceptors/credentials.interceptor';
import { ResponseCodeInterceptor } from './core/interceptors/response-code.interceptor';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { LanguageService } from './core/services/i18n/language.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(), // Lắng nghe và xử lý lỗi toàn cục của trình duyệt
    provideRouter(routes),                 // Cấu hình các tuyến đường (routes) chuyển trang cho ứng dụng

    // Cấu hình dịch thuật: Đặt ngôn ngữ phòng hờ (fallback) là tiếng Anh 'en' nếu không tìm thấy file dịch phù hợp
    ...provideTranslateService({
      fallbackLang: 'en',
      loader: provideTranslateHttpLoader({
        prefix: 'assets/i18n/', // Thư mục chứa các file ngôn ngữ (ví dụ: assets/i18n/vi.json)
        suffix: '.json'         // Đuôi file là .json
      })
    }),

    // Cấu hình HttpClient để gọi API, tích hợp các bộ gác cổng (Interceptors) theo thứ tự từ trên xuống dưới
    provideHttpClient(
      withInterceptors([
        credentialsInterceptor, // Thêm các cấu hình xác thực cookie/credentials nếu có
        AuthInterceptor,        // Tự động đính kèm Token (như Bearer Token) vào các request gửi lên Backend
        ResponseCodeInterceptor // Bộ lọc thông minh: Dịch mã code từ Backend trả về thành ngôn ngữ hiển thị
      ])
    ),

    // APP_INITIALIZER: Chốt chặn khởi động ứng dụng. 
    // Hệ thống buộc phải hoàn thành các tác vụ async trong này xong thì người dùng mới nhìn thấy giao diện Web.
    {
      provide: APP_INITIALIZER,
      useFactory: (languageService: LanguageService, authService: AuthService) => {
        return async () => {
          // BƯỚC 1: Khởi tạo và đồng bộ ngôn ngữ trước (Để lỡ các request sau bị lỗi thì có sẵn từ điển để dịch ngay)
          await languageService.init();

          // BƯỚC 2: Khôi phục phiên đăng nhập (Auto Login bằng cách gọi thông tin cá nhân hiện tại)
          try {
            // Chuyển đổi Observable từ hàm refreshMe() thành một Promise để dùng await chặn tiến trình
            await lastValueFrom(authService.refreshMe());
          } catch (error) {
            // Việc khôi phục phiên đăng nhập thất bại (như hết hạn token) là việc bình thường, 
            // ta chỉ ghi log lỗi ra console và vẫn cho ứng dụng tiếp tục chạy để người dùng vào trang đăng nhập.
            console.error('APP_INITIALIZER session restore failed:', error);
          }
        };
      },
      deps: [LanguageService, AuthService], // Khai báo các service phụ thuộc cần Inject vào factory
      multi: true // Cho phép Angular đăng ký nhiều cấu hình APP_INITIALIZER khác nhau nếu cần
    }
  ]
};
