namespace WebBanHang.Models.Common;

public static class SuccessCodes
{
    public static class Common
    {
        public const string Ok = "SUCCESS_OK";
        public const string Created = "SUCCESS_CREATED";
        public const string Retrieved = "SUCCESS_RETRIEVED";
        public const string Updated = "SUCCESS_UPDATED";
        public const string Deleted = "SUCCESS_DELETED";
    }

    public static class Auth
    {
        public const string CurrentUserRetrieved = "AUTH_CURRENT_USER_RETRIEVED";
        public const string SignInSuccess = "AUTH_SIGN_IN_SUCCESS";
        public const string SignUpSuccess = "AUTH_SIGN_UP_SUCCESS";
        public const string SignOutSuccess = "AUTH_SIGN_OUT_SUCCESS";
        public const string TokenRefreshed = "AUTH_TOKEN_REFRESHED";
        public const string ExternalSignInSuccess = "AUTH_EXTERNAL_SIGN_IN_SUCCESS";
        public const string LoginFinalized = "AUTH_LOGIN_FINALIZED";
    }

    public static class User
    {
        public const string Retrieved = "USER_RETRIEVED";
        public const string ListRetrieved = "USER_LIST_RETRIEVED";
        public const string Created = "USER_CREATED";
    }

    public static class Admin
    {
        public const string DashboardRetrieved = "ADMIN_DASHBOARD_RETRIEVED";
        public const string ChartRetrieved = "ADMIN_CHART_RETRIEVED";
        public const string SellerListRetrieved = "ADMIN_SELLER_LIST_RETRIEVED";
    }

    public static class Seller
    {
        public const string ManagementRetrieved = "SELLER_MANAGEMENT_RETRIEVED";
        public const string DetailRetrieved = "SELLER_DETAIL_RETRIEVED";
        public const string ApplicationRetrieved = "SELLER_APPLICATION_RETRIEVED";
        public const string ApplicationApproved = "SELLER_APPLICATION_APPROVED";
        public const string ApplicationRejected = "SELLER_APPLICATION_REJECTED";
        public const string DashboardRetrieved = "SELLER_DASHBOARD_RETRIEVED";
        public const string ChartRetrieved = "SELLER_CHART_RETRIEVED";
        public const string SettingsRetrieved = "SELLER_SETTINGS_RETRIEVED";
        public const string SettingsUpdated = "SELLER_SETTINGS_UPDATED";
        public const string RevenueRetrieved = "SELLER_REVENUE_RETRIEVED";
        public const string RevenueChartRetrieved = "SELLER_REVENUE_CHART_RETRIEVED";
        public const string InventoryRetrieved = "SELLER_INVENTORY_RETRIEVED";
        public const string InventoryUpdated = "SELLER_INVENTORY_UPDATED";
        public const string OrdersRetrieved = "SELLER_ORDERS_RETRIEVED";
        public const string OrderDetailRetrieved = "SELLER_ORDER_DETAIL_RETRIEVED";
        public const string OrderStatusUpdated = "SELLER_ORDER_STATUS_UPDATED";
    }

    public static class Product
    {
        public const string ListRetrieved = "PRODUCT_LIST_RETRIEVED";
        public const string DetailRetrieved = "PRODUCT_DETAIL_RETRIEVED";
        public const string Created = "PRODUCT_CREATED";
        public const string Updated = "PRODUCT_UPDATED";
        public const string Deleted = "PRODUCT_DELETED";
        public const string StatusChanged = "PRODUCT_STATUS_CHANGED";
        public const string ImageAdded = "PRODUCT_IMAGE_ADDED";
        public const string ImageUpdated = "PRODUCT_IMAGE_UPDATED";
        public const string ImageDeleted = "PRODUCT_IMAGE_DELETED";
        public const string MainImageSet = "PRODUCT_MAIN_IMAGE_SET";
    }

    public static class Category
    {
        public const string ListRetrieved = "CATEGORY_LIST_RETRIEVED";
        public const string DetailRetrieved = "CATEGORY_DETAIL_RETRIEVED";
        public const string Created = "CATEGORY_CREATED";
        public const string Updated = "CATEGORY_UPDATED";
        public const string Deleted = "CATEGORY_DELETED";
    }

    public static class Notification
    {
        public const string ListRetrieved = "NOTIFICATION_LIST_RETRIEVED";
        public const string CountRetrieved = "NOTIFICATION_COUNT_RETRIEVED";
        public const string MarkedAsRead = "NOTIFICATION_MARKED_AS_READ";
    }

    public static class Order
    {
        public const string ListRetrieved = "ORDER_LIST_RETRIEVED";
        public const string DetailRetrieved = "ORDER_DETAIL_RETRIEVED";
        public const string StatusUpdated = "ORDER_STATUS_UPDATED";
    }
}