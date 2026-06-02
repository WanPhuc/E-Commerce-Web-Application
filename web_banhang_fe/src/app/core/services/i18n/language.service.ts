import { DOCUMENT } from '@angular/common';
import { Injectable, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

// Định nghĩa các ngôn ngữ mà hệ thống Shoppy hỗ trợ (en: Anh, vi: Việt, jp: Nhật)
type SupportedLanguage = 'en' | 'vi' | 'jp';

@Injectable({ providedIn: 'root' }) // Khai báo Service này tồn tại dưới dạng Singleton (duy nhất) trong toàn bộ app
export class LanguageService {
  private readonly translate = inject(TranslateService); // Nhúng thư viện dịch thuật của Angular
  private readonly document = inject(DOCUMENT);         // Nhúng đối tượng DOM Document để thao tác với thẻ html
  private readonly storageKey = 'shoppy.language';     // Key dùng để lưu cấu hình ngôn ngữ xuống trình duyệt
  private readonly supportedLanguages: SupportedLanguage[] = ['en', 'vi', 'jp']; // Mảng các ngôn ngữ hợp lệ

  // Hàm khởi tạo ngôn ngữ, được gọi ngay lúc ứng dụng vừa khởi động (từ APP_INITIALIZER)
  async init(): Promise<void> {
    // Khai báo cho thư viện dịch biết danh sách các ngôn ngữ đang được hệ thống hỗ trợ
    this.translate.addLangs(this.supportedLanguages);

    // Tìm kiếm ngôn ngữ tối ưu nhất (Xem ở localStorage hoặc cài đặt của trình duyệt)
    const preferredLanguage = this.resolveLanguage();
    
    // Kích hoạt ngôn ngữ đó cho toàn bộ hệ thống web
    await this.setLanguage(preferredLanguage);
  }

  // Lấy ra ngôn ngữ hiện tại đang được áp dụng trên giao diện
  getCurrentLanguage(): SupportedLanguage {
    // Ưu tiên lấy ngôn ngữ đang dùng, nếu không có thì lấy ngôn ngữ mặc định, bí quá thì trả về 'en'
    return this.normalizeLanguage(this.translate.currentLang || this.translate.defaultLang || 'en');
  }

  // Hàm thay đổi ngôn ngữ (sẽ gọi hàm này khi người dùng bấm nút đổi ngôn ngữ trên Header)
  async setLanguage(language: string): Promise<void> {
    // Chuẩn hóa chuỗi ngôn ngữ đầu vào để đảm bảo nó thuộc nhóm hệ thống hỗ trợ
    const resolvedLanguage = this.normalizeLanguage(language);

    // Lưu lại ngôn ngữ vừa chọn vào bộ nhớ trình duyệt để lần sau mở web tự động nhớ, không cần đoán lại
    this.safeStorageSet(this.storageKey, resolvedLanguage);
    
    // Thay đổi thuộc tính lang trên thẻ <html> (ví dụ: <html lang="vi">), việc này cực kỳ tốt cho SEO
    this.document.documentElement.lang = resolvedLanguage;

    // Ra lệnh cho thư viện tải file JSON dịch thuật tương ứng về và áp dụng cấu hình mới
    await firstValueFrom(this.translate.use(resolvedLanguage));

    // Một số ngôn ngữ có thể được chọn nhưng chưa thật sự nạp vào store nội bộ của ngx-translate.
    // Nếu các key đại diện vẫn trả về raw key thì tải lại để đồng bộ UI ngay lập tức.
    const translationProbeKeys = ['ui.loading', 'ui.signIn', 'ui.footer.customerService'];
    const needsReload = translationProbeKeys.some((key) => this.translate.instant(key) === key);

    if (needsReload) {
      await firstValueFrom(this.translate.reloadLang(resolvedLanguage));
    }
  }

  // Hàm phân tích và tìm ra ngôn ngữ phù hợp nhất để hiển thị ban đầu
  private resolveLanguage(): SupportedLanguage {
    // Ưu tiên 1: Kiểm tra xem trước đây người dùng từng chọn ngôn ngữ nào và lưu ở localStorage chưa
    const storedLanguage = this.safeStorageGet(this.storageKey);
    if (storedLanguage) {
      return this.normalizeLanguage(storedLanguage); // Nếu có, chuẩn hóa và dùng luôn
    }

    // Ưu tiên 2: Nếu chưa từng chọn, mò vào cài đặt ngôn ngữ mặc định của chính Trình duyệt máy họ
    const browserLanguage = navigator.languages?.[0] || navigator.language || 'en';
    return this.normalizeLanguage(browserLanguage); // Chuẩn hóa ngôn ngữ của trình duyệt
  }

  // Hàm chuẩn hóa chuỗi ngôn ngữ, đưa về đúng 1 trong 3 mã định danh: 'en', 'vi', 'jp'
  private normalizeLanguage(language: string): SupportedLanguage {
    const lowerLanguage = language.toLowerCase(); // Chuyển hết sang chữ thường để so sánh tránh sai sót

    // Nếu chuỗi bắt đầu bằng 'vi' (ví dụ: vi, vi-VN) -> Quy về tiếng Việt
    if (lowerLanguage.startsWith('vi')) {
      return 'vi';
    }

    // Nếu chuỗi bắt đầu bằng 'ja' hoặc 'jp' (ví dụ: ja, jp, ja-JP) -> Quy về tiếng Nhật
    if (lowerLanguage.startsWith('ja') || lowerLanguage.startsWith('jp')) {
      return 'jp';
    }

    // Nếu chuỗi bắt đầu bằng 'en' (ví dụ: en, en-US, en-GB) -> Quy về tiếng Anh
    if (lowerLanguage.startsWith('en')) {
      return 'en';
    }

    // Trường hợp trình duyệt trả về ngôn ngữ lạ hoắc không hỗ trợ (ví dụ: tiếng Pháp 'fr') -> Trả về mặc định tiếng Anh
    return 'en';
  }

  // Hàm đọc localStorage an toàn, bọc trong try/catch để tránh lỗi sập ứng dụng ở một số môi trường bị chặn cookie
  private safeStorageGet(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch {
      return null; // Nếu bị lỗi (môi trường ẩn danh nghiêm ngặt chặn storage), trả về null
    }
  }

  // Hàm ghi localStorage an toàn
  private safeStorageSet(key: string, value: string): void {
    try {
      localStorage.setItem(key, value);
    } catch {
      // Âm thầm bỏ qua lỗi lưu trữ, ứng dụng vẫn tiếp tục chạy bình thường mà không bị vỡ giao diện
    }
  }
}