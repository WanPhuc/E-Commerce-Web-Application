import { Component } from '@angular/core';

@Component({
  selector: 'app-seller-coming-soon',
  standalone: true,
  template: `
    <div class="d-flex flex-column align-items-center justify-content-center" style="height: calc(100vh - 80px);">
      <div class="text-center">
        <i class="fa-solid fa-hammer fa-3x text-primary mb-4 opacity-50"></i>
        <h4 class="fw-bold text-dark mb-2">Tính năng đang phát triển</h4>
        <p class="text-muted mb-0">Chúng tôi đang xây dựng tính năng này, vui lòng quay lại sau.</p>
      </div>
    </div>
  `
})
export class SellerComingSoonComponent {}
