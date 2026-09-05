import { ApiResponse, PagedResult } from './../../../shared/types/ApiResponse';
import { HttpClient } from '@angular/common/http';
import { environment } from './../../../../environments/environment';
import { inject, Injectable } from "@angular/core";
import { map, Observable } from 'rxjs';
import { CreateSellerProductDto, ProductImageCreateDto, ProductImageDto, SellerProductDto, UpdateSellerProductDto } from '../../models/seller/sellerProduct/sellerProduct.model';
import { ProductStatus } from '../../../shared/types/enum/ProductStatus';

@Injectable({providedIn:'root'})
export class SellerProductService{
    private baseUrl=`${environment.apiUrl}/rseller/products`;
    private http = inject(HttpClient);

    getSellerProducts():Observable<SellerProductDto[]>{
        return this.http.get<ApiResponse<PagedResult<SellerProductDto>>>(`${this.baseUrl}`).pipe(
            map(res=>res.data.data)
        );
    }
    getSellerProductById(productId:string):Observable<SellerProductDto>{
        return this.http.get<ApiResponse<SellerProductDto>>(`${this.baseUrl}/detail/${productId}`).pipe(
            map(res=>res.data)
        );
    }
    createProduct(product:CreateSellerProductDto):Observable<SellerProductDto>{
        return this.http.post<ApiResponse<SellerProductDto>>(`${this.baseUrl}/create`,product).pipe(
            map(res=>res.data)
        );
    }
    updateProduct(productId:string,product:UpdateSellerProductDto):Observable<SellerProductDto>{
        return this.http.put<ApiResponse<SellerProductDto>>(`${this.baseUrl}/update/${productId}`,product).pipe(
            map(res=>res.data)
        )
    }
    deleteProduct(productId:string):Observable<string>{
        return this.http.delete<ApiResponse<string>>(`${this.baseUrl}/delete/${productId}`).pipe(
            map(res=>res.data)
        )
    };
    changeStatus(productId:string,status:ProductStatus):Observable<string>{
        return this.http.put<ApiResponse<string>>(`${this.baseUrl}/change-status/${productId}`,null,{params:{newStatus:status.toString()}}).pipe(
            map(res=>res.data)
        )
    }

    // Image
    addProductImage(productId:string,image:ProductImageCreateDto):Observable<ProductImageDto>{
        return this.http.post<ApiResponse<ProductImageDto>>(`${this.baseUrl}/${productId}/images`,image).pipe(
            map(res=>res.data)
        )
    }
    updateProductImage(productId:string,imageId:string,image:ProductImageCreateDto):Observable<ProductImageDto>{
        return this.http.put<ApiResponse<ProductImageDto>>(`${this.baseUrl}/${productId}/images/${imageId}`,image).pipe(
            map(res=>res.data)
        )
    }
    deleteProductImage(productId:string,imageId:string):Observable<string>{
        return this.http.delete<ApiResponse<string>>(`${this.baseUrl}/${productId}/images/${imageId}`).pipe(
            map(res=>res.data)
        )
    }
    setMainImage(productId:string,imageId:string):Observable<boolean>{
        return this.http.put<ApiResponse<boolean>>(`${this.baseUrl}/${productId}/images/${imageId}/set-main`,null).pipe(
            map(res=>res.data)
        )
    }
}
