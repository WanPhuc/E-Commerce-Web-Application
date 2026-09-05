import { environment } from './../../../../environments/environment';
import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { CategoryDto, CreateCategoryDto } from "../../models/admin/category.model";
import { ApiResponse, PagedResult } from "../../../shared/types/ApiResponse";

@Injectable({providedIn: 'root'})
export class CategoryService {
    private baseUrl = `${environment.apiUrl}/admin/categories`;
    private http = inject(HttpClient);

    getAllCategories():Observable<CategoryDto[]>{
        return this.http.get<ApiResponse<PagedResult<CategoryDto>>>(this.baseUrl).pipe(
            map(res =>res.data.data)
        );
    }
    detailCategory(id:string):Observable<CategoryDto>{
        return this.http.get<ApiResponse<CategoryDto>>(`${this.baseUrl}/detail/${id}`).pipe(
            map(res =>res.data)
        );
    }
    createCategory(category:CreateCategoryDto):Observable<CategoryDto>{
        return this.http.post<ApiResponse<CategoryDto>>(`${this.baseUrl}/create`,category).pipe(
            map(res =>res.data)
        );
    }
    updateCategory(id:string,category:CreateCategoryDto):Observable<CategoryDto>{
        return this.http.put<ApiResponse<CategoryDto>>(`${this.baseUrl}/update/${id}`,category).pipe(
            map(res =>res.data)
        );
    }
    deleteCategory(id:string):Observable<void>{
        return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/delete/${id}`).pipe(
            map(res =>res.data)
        );
    }
}

