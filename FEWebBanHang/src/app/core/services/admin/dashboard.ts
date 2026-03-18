import { DashboardChartDto, DashboardChartPointDto, DashboardDto, DashboardRanger } from './../../models/admin/dashboard.model';
import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { ApiResponse } from '../auth/auth';

@Injectable({ providedIn: 'root' })
export class DashboardService {
    private baseUrl = 'http://localhost:5144/api/v1/admin';
    private http =inject(HttpClient);

    getDashboardStats():Observable<DashboardDto>{
        return this.http.get<ApiResponse<DashboardDto>>(this.baseUrl).pipe(
            map(res =>res.data)
        );
    
    }
    getChart(range:DashboardRanger):Observable<DashboardChartDto>{
        return this.http.get<ApiResponse<DashboardChartDto>>(`${this.baseUrl}/chart?range=${range}`).pipe(
            map(res =>res.data)
        );
    
    }
}