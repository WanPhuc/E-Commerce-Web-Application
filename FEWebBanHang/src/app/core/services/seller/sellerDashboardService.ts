import { ApiResponse } from './../../../shared/types/ApiResponse';
import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { SellerDashboardChartDto, SellerDashboardDto } from "../../models/seller/sellerDashboard.model";

@Injectable({providedIn:'root'})
export class SellerDashboardService{
    private baseUrl = 'http://localhost:5144/api/v1/rseller/dashboard';
    private http = inject(HttpClient);

    getDashboardStats():Observable<SellerDashboardDto>{
        return this.http.get<ApiResponse<SellerDashboardDto>>(this.baseUrl).pipe(
            map(res=>res.data) 
        );
    }
    getChart(ranger:string):Observable<SellerDashboardChartDto>{
        return this.http.get<ApiResponse<SellerDashboardChartDto>>(`${this.baseUrl}/chart?ranger=${ranger}`).pipe(
            map(res=>res.data)
        );
    }
}