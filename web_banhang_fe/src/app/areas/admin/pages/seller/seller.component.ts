import { ToastService } from './../../../../shared/toast/toast.service';
import { Component, inject, OnInit, signal } from '@angular/core';
import { SellerApplicationDto, SellerDto } from '../../../../core/models/admin';
import { CommonModule } from '@angular/common';
import { StatusPipe } from '../../../../shared/pipes/status.pipe';
import { RouterLink } from "@angular/router";
import { SellerService } from '../../../../core/services/admin/seller/seller';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-seller',
  imports: [CommonModule, StatusPipe, RouterLink, TranslateModule],
  templateUrl: './seller.component.html',
  styleUrl: './seller.component.scss',
})
export class SellerComponent implements OnInit{
  private sellerService = inject(SellerService);
  private toast =inject(ToastService)
  private translate = inject(TranslateService)

  activeTab=signal<'seller'|'application'>('seller');
  loading=signal(true);
  pendingCount=signal(0);

  sellers=signal<SellerDto[]>([]);
  applications=signal<SellerApplicationDto[]>([]);

  ngOnInit(): void {
    this.loadData();
  }

  get sellerCount(): number {
    return this.sellers().length;
  }

  get applicationCount(): number {
    return this.applications().length;
  }

  getStatusBadgeClass(status: unknown): string {
    const normalized = typeof status === 'string' ? status.trim().toLowerCase() : status;

    switch (normalized) {
      case 'approved':
      case 1:
        return 'bg-success-subtle text-success border border-success-subtle';
      case 'pending':
      case 0:
        return 'bg-warning-subtle text-warning border border-warning-subtle';
      case 'rejected':
      case 2:
        return 'bg-danger-subtle text-danger border border-danger-subtle';
      default:
        return 'bg-secondary-subtle text-secondary border border-secondary-subtle';
    }
  }

  loadData(){
    this.loading.set(true);
    this.sellerService.getSellerManagement().subscribe({
      next:(data)=>{
        this.sellers.set(data.approvedSellers.data);
        this.applications.set(data.sellerApplications.data);
        this.pendingCount.set(data.pendingSellerApplications);
        this.loading.set(false);
      },error: (err) => {
        const message =
          err?.error?.message || this.translate.instant('ui.admin.loadSellerDataFailed');

        this.toast.show(message, 'error');
        this.loading.set(false);
      }

    });
  }
  switchTab(tab:'seller'|'application'){
    this.activeTab.set(tab);
  }
  approve(id: string) {
    this.sellerService.approveSeller(id).subscribe({
        next: (response) => {
            this.toast.show(this.translate.instant('ui.admin.approveSellerSuccess'), 'success');
            this.loadData();
        },
        error: (err) => {
            this.toast.show(
                err?.error?.message ?? this.translate.instant('ui.admin.approveSellerFailed'),
                'error'
            );
        }
    });
  }
  
  reject(id:string){
    this.sellerService.rejectSeller(id).subscribe({
      next:()=>{
        this.toast.show(this.translate.instant('ui.admin.rejectSellerSuccess'), 'success');
        this.loadData();
      },error:(err)=>{
        this.toast.show(
          err?.error?.message ?? this.translate.instant('ui.admin.rejectSellerFailed'),
          'error'
        ); 
      }
    })
  }

}
