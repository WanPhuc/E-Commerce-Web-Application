import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SellerService } from '../../../../../../core/services/admin/seller/seller';
import { SellerApplicationDetailDto } from '../../../../../../core/models/admin';
import { CommonModule } from '@angular/common';
import { StatusPipe } from '../../../../../../shared/pipes/status.pipe';

@Component({
  selector: 'app-application-seller-detail',
  imports: [RouterLink,CommonModule,StatusPipe],
  templateUrl: './application-seller-detail.component.html',
  styleUrl: './application-seller-detail.component.scss',
})
export class ApplicationSellerDetailComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private sellerService=inject(SellerService);

  loading = signal(true);
  detail = signal<SellerApplicationDetailDto | null>(null);

  ngOnInit(): void {
    const id=this.route.snapshot.paramMap.get('id');
    if(id){
      this.loadDetail(id);
    }else{
      this.router.navigate(['/admin/seller']);
    }
  }
  loadDetail(id:string){
    this.loading.set(true);
    this.sellerService.getSellerApplicationDetail(id).subscribe({
      next:(data)=>{
        this.detail.set(data);
        this.loading.set(false);
      },
      error:(err)=>{
        console.error('Error loading seller detail:', err);
        this.loading.set(false);
      }
    });
  }
  approve(id:string){
    this.sellerService.approveSeller(id).subscribe({
      next:()=>{
        this.router.navigate(['/admin/seller']);
      },
      error:(err)=>{
        console.error('Error approving seller:', err);
      }
    })
  }
  reject(id:string){
    this.sellerService.rejectSeller(id).subscribe({
      next:()=>{
        this.router.navigate(['/admin/seller']);
      },
      error:(err)=>{
        console.error('Error rejecting seller:', err);
      }
    })
  }
}
