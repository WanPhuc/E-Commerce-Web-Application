import { ApiResponse } from '../../../../shared/types/ApiResponse';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { SellerApplicationDetailDto } from '../../../models/admin/seller.model';
import { SellerDetailDto, SellerManagementResponse } from '../../../models/admin';

@Injectable({ providedIn: 'root' })
export class SellerService {
  private baseUrl = 'http://localhost:5144/api/v1/admin/sellers';
  private http = inject(HttpClient);

  getSellerManagement(): Observable<SellerManagementResponse> {
    return this.http
      .get<ApiResponse<SellerManagementResponse>>(this.baseUrl)
      .pipe(map(res => res.data));
  }

  approveSeller(id: string): Observable<string> {
    return this.http
        .post<ApiResponse<null>>(`${this.baseUrl}/${id}/approved`, {})
        .pipe(map(res => res.message));
    }


  rejectSeller(id: string): Observable<string> {
    return this.http
      .post<ApiResponse<null>>(`${this.baseUrl}/${id}/rejected`, {})
      .pipe(map(res => res.message));
  }

  getSellerDetail(id: string): Observable<SellerDetailDto> {
    return this.http
      .get<ApiResponse<SellerDetailDto>>(`${this.baseUrl}/sellers/${id}`)
      .pipe(map(res => res.data));
  }

  getSellerApplicationDetail(id: string): Observable<SellerApplicationDetailDto> {
    return this.http
      .get<ApiResponse<SellerApplicationDetailDto>>(`${this.baseUrl}/application-seller/${id}`)
      .pipe(map(res => res.data));
  }
}
