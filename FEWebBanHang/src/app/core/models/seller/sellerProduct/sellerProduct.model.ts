export interface SellerProductDto{
    id:string;
    name:string;
    description?:string;
    sku:string;
    price:number;
    stock:number;
    lowStockThreshold:number;
    status:string;
    discountPercent:number;
    rating:number;
    reviewCount:number;
    soldCount:number;
    categoryId:string;
    categoryName:string;
    createAt:string;
    mainImageUrl:string;
    images:ProductImageDto[];
}
export interface ProductImageDto{
    id:string;
    imageUrl:string;
    isMainImage:boolean;
}

export interface CreateSellerProductDto{
    name:string;
    description?:string;
    sku:string;
    price:number;
    stock:number;
    lowStockThreshold:number;
    discountPercent:number;
    categoryId:string;

}
export interface UpdateSellerProductDto{
    name:string;
    description?:string;
    sku:string;
    price:number;
    stock:number;
    lowStockThreshold:number;
    discountPercent:number;
    status:string;
    categoryId:string;
}

//Image 
export interface ProductImageCreateDto{
    imageUrl:string;
    isMainImage:boolean;
}