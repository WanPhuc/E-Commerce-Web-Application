import { ToastService } from './../../../../shared/toast/toast.service';
import { Component, inject, OnInit, signal } from '@angular/core';
import { SellerApplicationDto, SellerDto } from '../../../../core/models/admin';
import { CommonModule } from '@angular/common';
import { StatusPipe } from '../../../../shared/pipes/status.pipe';
import { RouterLink } from "@angular/router";
import { SellerService } from '../../../../core/services/admin/seller/seller';

@Component({
  selector: 'app-seller',
  imports: [CommonModule, StatusPipe, RouterLink],
  templateUrl: './seller.component.html',
  styleUrl: './seller.component.scss',
})
export class SellerComponent implements OnInit{
  private sellerService = inject(SellerService);
  private toast =inject(ToastService)

  activeTab=signal<'seller'|'application'>('seller');
  loading=signal(true);
  pendingCount=signal(0);

  sellers=signal<SellerDto[]>([]);
  applications=signal<SellerApplicationDto[]>([]);

  ngOnInit(): void {
    this.loadData();
  }
  loadData(){
    this.loading.set(true);
    this.sellerService.getSellerManagement().subscribe({
      next:(data)=>{
         console.log('Raw data from API:', data); // Debug
        console.log('Applications:', data.sellerApplications); // Debug
        console.log('Applications length:', data.sellerApplications?.length);
        this.sellers.set(data.approvedSellers);
        this.applications.set(data.sellerApplications);
        this.pendingCount.set(data.pendingSellerApplications);
        this.loading.set(false);
      },error: (err) => {
        const message =
          err?.error?.message || 'Không thể tải dữ liệu seller';

        this.toast.show(message, 'error');
        this.loading.set(false);
      }

    });
  }
  switchTab(tab:'seller'|'application'){
    this.activeTab.set(tab);
  }
  approve(id: string) {
    console.log('Approving seller with ID:', id); // Debug
    this.sellerService.approveSeller(id).subscribe({
        next: (response) => {
            console.log('Approve response:', response); // Debug
            this.toast.show('Duyệt seller thành công', 'success');
            this.loadData();
        },
        error: (err) => {
            console.error('Approve error:', err); // Debug
            this.toast.show(
                err?.error?.message ?? 'Không thể duyệt seller',
                'error'
            );
        }
    });
  }
  
  reject(id:string){
    this.sellerService.rejectSeller(id).subscribe({
      next:()=>{
        this.toast.show('Từ chối seller thành công', 'success');
        this.loadData();
      },error:(err)=>{
        this.toast.show(
          err?.error?.message ?? 'Không thể từ chối seller',
          'error'
        ); 
      }
    })
  }

}
