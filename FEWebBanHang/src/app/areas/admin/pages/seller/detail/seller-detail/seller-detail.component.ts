import { StatusPipe } from './../../../../../../shared/pipes/status.pipe';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SellerDetailDto } from '../../../../../../core/models/admin';
import { CommonModule } from '@angular/common';
import { SellerService } from '../../../../../../core/services/admin/seller/seller';

@Component({
  selector: 'app-seller-detail',
  imports: [RouterLink,CommonModule,StatusPipe],
  templateUrl: './seller-detail.component.html',
  styleUrl: './seller-detail.component.scss',
})
export class SellerDetailComponent implements OnInit{
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private sellerService=inject(SellerService);

  loading = signal(true);
  productCount = computed(() => this.detail()?.productCount ?? 0);
  detail = signal<SellerDetailDto | null>(null);

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
    this.sellerService.getSellerDetail(id).subscribe({
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
}
