import { ChartRanger } from "../../../shared/types/chartRanger"

export interface SellerDashboardDto{
    totalProducts:number
    totalOrdersToDay:number
    totalRevenueToDay:number
    totalRevenueThisMonth:number
    totalRevenueLastMonth:number
    revenueGrowth:number
    averageRating:number
    totalReviews:number
    pendingOrders:number
    processingOrders:number
    shippedOrders:number
    cancelledOrders:number
    topSellingProducts:TopSellingProductDashboardDto[];
    ordersProcess:OrdersProcessDashboardDto[];
    errorInventory:ErrorInventoryDashboardDto[];
    recentRatings:RecentRatingDashboardDto[];
}
export interface TopSellingProductDashboardDto{
    productId:string;
    productName:string;
    quantitySold:number;
    totalRevenue:number;
    imageUrl:string;
    sku:string;
}
export interface OrdersProcessDashboardDto{
    orderId:string;
    orderDate:string;
    customerName:string;
    totalAmount:number;
    status:string;
    paymentStatus:string;
}
export interface ErrorInventoryDashboardDto{
    productId:string;
    productName:string;
    stockQuantity:number;
    sku:string;
}
export interface RecentRatingDashboardDto{
    reviewId:string;
    productId:string;
    productName:string;
    customerName:string;
    sku:string;
    imageUrl:string;
    rating:number;
    comment:string;
    reviewDate:string;
}
export type SellerDashboardMetric='All'|'Revenue'|'Orders';
export interface SellerDashboardChartPointDto{
    label:string;
    revenue:number;
    orders:number;
}
export interface SellerDashboardChartDto{
    range:ChartRanger;
    points:SellerDashboardChartPointDto[];
}