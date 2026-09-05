export interface DashboardDto{
    totalProducts:number 
    totalSellersThisMonth:number 
    totalSellersLastMonth:number 
    totalUsersThisMonth:number 
    totalUsersLastMonth:number 
    totalOrdersThisMonth:number 
    totalOrdersLastMonth:number 
    revenueThisMonth:number 
    revenueLastMonth:number 
}

export type DashboardRanger='Week'|'Month'|'Year';
export type DashboardMetric='All'|'Orders'|'Revenue'|'Users'|'Sellers';
export interface DashboardChartPointDto{
    label:string;
    orders:number;
    revenue:number;
    users:number;
    sellers:number;
}
export interface DashboardChartDto{
    range:DashboardRanger;
    points:DashboardChartPointDto[];
}