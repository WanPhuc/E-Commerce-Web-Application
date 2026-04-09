import { Component, inject, Inject, OnInit, signal } from "@angular/core";
import { SellerProductService } from "../../../../core/services/seller/sellerProductService.service";
import { ToastService } from "../../../../shared/toast/toast.service";
import { SellerProductDto } from "../../../../core/models/seller/sellerProduct/sellerProduct.model";

@Component({
    selector:'app-seller-product',
    imports:[],
    standalone:true,
    templateUrl:'./sellerProduct.component.html',
    styleUrl:'./sellerProduct.component.scss'
})
export class SellerProductComponent implements OnInit{
    private productService = inject(SellerProductService);
    private toast = inject(ToastService);

    loading = signal(false);
    products=signal<SellerProductDto[]>([]);

    deleteOpen=signal(false);
    deletingId=signal<string | null>(null);
    deletingName=signal<string>('');
    deletingBusy=signal(false);


    ngOnInit(): void {
        this.loadProducts();
    }
    loadProducts(){
        this.loading.set(true);
        this.productService.getSellerProducts().subscribe({
            next:(data)=>{
                this.products.set(data);
                this.loading.set(false);
            },error:(err)=>{
                this.loading.set(false);
                this.toast.show(err?.error?.message||'Không thể tải dữ liệu','error');
            }
        })
    };
    openDelete(node:SellerProductDto){
        this.deletingId.set(node.id);
        this.deletingName.set(node.name);
        this.deleteOpen.set(true);
    }
    closeDelete(){
        this.deletingId.set(null);
        this.deletingName.set('');
        this.deleteOpen.set(false);
    }
    confirmDelete(){
        const id=this.deletingId();
        if(!id) return;
        this.deletingBusy.set(true);
        this.productService.deleteProduct(id).subscribe({
            next:(msg)=>{
                this.toast.show(msg,'success');
                this.loadProducts();
                this.closeDelete();
                this.deletingBusy.set(false);
            },error:(err)=>{
                this.deletingBusy.set(false);
                this.toast.show(err?.error?.message||'Delete product failed','error');
            }
        })
    }


}