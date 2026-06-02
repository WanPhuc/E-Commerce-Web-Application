import { SellerSummaryDto } from '../admin/seller.model';
export interface UserDto{
    id:string;
    fullName:string;
    email:string;
    createdAt:string;
    isActive:boolean;
    role:string;
    seller:SellerSummaryDto|null;
}
export interface CreateUserDto{
    fullName:string;
    email:string;
    password:string;
}