import { CreateUserDto } from '../../../models/admin/user.model';
import { ApiResponse } from '../../../../shared/types/ApiResponse';
import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { UserDto } from "../../../models/admin/user.model";

@Injectable({providedIn: 'root'})
export class UserService {
    private http = inject(HttpClient);
    private baseUrl = 'http://localhost:5144/api/v1/admin/users';

    getAllUsers():Observable<UserDto[]>{
        return this.http.get<ApiResponse<UserDto[]>>(this.baseUrl).pipe(map (res=>res.data));
        
    }
    getUserById(id:string):Observable<UserDto>{
        return this.http.get<ApiResponse<UserDto>>(`${this.baseUrl}/detail/${id}`).pipe(map (res=>res.data));
    
    }
    createUser(createUserDto:CreateUserDto):Observable<UserDto>{
        return this.http.post<ApiResponse<UserDto>>(`${this.baseUrl}/create`,createUserDto).pipe(map (res=>res.data));
    }
}