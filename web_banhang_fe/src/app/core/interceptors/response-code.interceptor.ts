import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { map } from 'rxjs';

// Bộ lọc chặn (Interceptor) đứng ở giữa kết nối mạng: Tự động dịch mã Code từ Backend gửi về trước khi đưa vào Component
export const ResponseCodeInterceptor: HttpInterceptorFn = (req, next) => {
  const translate = inject(TranslateService); // Nhúng dịch vụ dịch thuật vào để dùng trực tiếp trong hàm

  return next(req).pipe(
    map(event => {
      // Nếu sự kiện HTTP chạy qua không phải là một Response (kết quả trả về hoàn chỉnh, ví dụ như tiến trình Upload) 
      // thì không xử lý, cho nó đi qua luôn nguyên vẹn.
      if (!(event instanceof HttpResponse)) {
        return event;
      }

      // Ép kiểu dữ liệu phần body trả về thành một Object dạng Key-Value để dễ thao tác kiểm tra dữ liệu
      const body = event.body as Record<string, unknown> | null;
      
      // Kiểm tra điều kiện ngặt: Nếu không có body, hoặc body không phải object, hoặc thiếu thuộc tính "code" dạng chuỗi 
      // thì trả về kết quả gốc, không can thiệp gì thêm.
      if (!body || typeof body !== 'object' || typeof body['code'] !== 'string') {
        return event;
      }

      const code = body['code'];       // Lấy mã code nghiệp vụ từ Backend (ví dụ: 'AUTH_INVALID_PASSWORD')
      const message = body['message']; // Lấy câu thông báo mặc định bằng tiếng Anh kèm theo từ Backend

      // Tra từ điển bằng cách tìm từ khóa `codes.<mã_code>` trong file JSON ngôn ngữ hiện tại của ứng dụng
      const translatedMessage = translate.instant(`codes.${code}`);
      
      // Logic gán thông báo cuối cùng một cách an toàn (Cơ chế Phòng hờ - Fallback):
      const resolvedMessage = translatedMessage && translatedMessage !== `codes.${code}`
        ? translatedMessage // 1. Nếu tìm thấy bản dịch hợp lệ trong file JSON -> Lấy bản dịch đó
        : typeof message === 'string'
          ? message         // 2. Nếu không dịch được, kiểm tra nếu có message gốc từ Backend gửi về -> Dùng tạm message gốc
          : '';             // 3. Nếu cả hai đều không có -> Trả về chuỗi rỗng

      // Tiến hành nhân bản (clone) lại sự kiện HTTP Response cũ, nhưng ghi đè đè phần body mới với thông báo đã được dịch
      return event.clone({
        body: {
          ...body,            // Giữ nguyên toàn bộ các thuộc tính payload khác (như data, token, status...)
          message: resolvedMessage // Thay thế duy nhất giá trị hiển thị của trường message bằng câu đã dịch tiếng Việt/Nhật
        }
      });
    })
  );
};