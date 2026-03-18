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
    productName:string;
    quantitySold:number;
    totalRevenue:number;
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
}
export interface RecentRatingDashboardDto{
    productId:string;
    productName:string;
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