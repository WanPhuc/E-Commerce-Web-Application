import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { ApiResponse } from "../../types/ApiResponse";
import { NotificationDto } from "../../../core/models/publish/notification.model";
import { environment } from "../../../../environments/environment";

@Injectable({providedIn:'root'})
export class NotificationService{
    private baseUrl = `${environment.apiUrl}/global/notifications`;
    private http =inject(HttpClient);

    getNotifications():Observable<NotificationDto[]>{
        return this.http.get<ApiResponse<NotificationDto[]>>(this.baseUrl).pipe(
            map(res=>res.data)
        );
    }
    getUnreadNotificationCount():Observable<{unreadCount:number}>{
        return this.http.get<{unreadCount:number}>(`${this.baseUrl}/unread-count`);   
    }
    markAsRead(notiId:string):Observable<void>{
        return this.http.patch<void>(`${this.baseUrl}/${notiId}/mark-as-read`,{});
    }
}