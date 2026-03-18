export interface SellerDto{
    id:string;
    storeName:string;
    email:string;
    createdAt:string;
    status:string;
}
export interface SellerApplicationDto{
    id:string;
    storeName:string;
    email:string;
    createdAt:string;
    status:string;
}
export interface SellerManagementResponse{
    pendingSellerApplications:number;
    sellerApplications:SellerApplicationDto[];
    approvedSellers:SellerDto[];
}
export interface SellerDetailDto extends SellerDto{
    userId:string;
    fullName:string;
    description?:string|null;
    productCount:number;
}
export interface SellerApplicationDetailDto extends SellerApplicationDto{
    userId:string;
    fullName:string;
    description?:string|null;
    reviewAt?:string|null;

}
export interface SellerSummaryDto {
  id: string;
  shopName: string;
  status: string; // enum → string
}
