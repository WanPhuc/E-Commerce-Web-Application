namespace WebBanHang.Models.Common;

public static class ErrorCodes
{
    public static class Auth
    {
        public const string InvalidEmail = "AUTH_INVALID_EMAIL";
        public const string InvalidPassword = "AUTH_INVALID_PASSWORD";
        public const string MissingCredentials = "AUTH_MISSING_CREDENTIALS";
        public const string EmailInUse = "AUTH_EMAIL_IN_USE";
        public const string RoleNotFound = "AUTH_ROLE_NOT_FOUND";
        public const string InvalidRefreshToken = "AUTH_INVALID_REFRESH_TOKEN";
        public const string RefreshTokenRevoked = "AUTH_REFRESH_TOKEN_REVOKED";
        public const string RefreshTokenExpired = "AUTH_REFRESH_TOKEN_EXPIRED";
        public const string InvalidAccessToken = "AUTH_INVALID_ACCESS_TOKEN";
        public const string UserNotFound = "AUTH_USER_NOT_FOUND";
        public const string ProviderRequired = "AUTH_PROVIDER_REQUIRED";
        public const string ProviderDataInvalid = "AUTH_PROVIDER_DATA_INVALID";
        public const string LoginFailed = "AUTH_LOGIN_FAILED";
        public const string TokenNotFound = "AUTH_TOKEN_NOT_FOUND";
        public const string ExternalUserInvalid = "AUTH_EXTERNAL_USER_INVALID";
        public const string ExternalLoginFailed = "AUTH_EXTERNAL_LOGIN_FAILED";
    }

    public static class Common
    {
        public const string Success = "SUCCESS_OK";
        public const string Created = "CREATED";
        public const string BadRequest = "BAD_REQUEST";
        public const string Conflict = "CONFLICT";
        public const string Unauthorized = "COMMON_UNAUTHORIZED";
        public const string Forbidden = "COMMON_FORBIDDEN";
        public const string NotFound = "COMMON_NOT_FOUND";
        public const string InternalServerError = "COMMON_INTERNAL_SERVER_ERROR";
        public const string UnprocessableEntity = "UNPROCESSABLE_ENTITY";
    }

    public static class Validation
    {
        public const string RequiredFieldMissing = "VALIDATION_REQUIRED_FIELD_MISSING";
        public const string InvalidFormat = "VALIDATION_INVALID_FORMAT";
        public const string InvalidValue = "VALIDATION_INVALID_VALUE";
    }

    public static class User
    {
        public const string NotFound = "USER_NOT_FOUND";
        public const string EmailInUse = "USER_EMAIL_IN_USE";
        public const string DefaultRoleNotFound = "USER_DEFAULT_ROLE_NOT_FOUND";
        public const string AlreadyExists = "USER_ALREADY_EXISTS";
        public const string CannotDelete = "USER_CANNOT_DELETE";
    }

    public static class Admin
    {
        public const string NotFound = "ADMIN_NOT_FOUND";
        public const string PermissionDenied = "ADMIN_PERMISSION_DENIED";
        public const string InvalidAction = "ADMIN_INVALID_ACTION";
    }

    public static class Seller
    {
        public const string NotFound = "SELLER_NOT_FOUND";
        public const string ApplicationNotFound = "SELLER_APPLICATION_NOT_FOUND";
        public const string AlreadySeller = "SELLER_ALREADY_EXISTS";
        public const string PermissionDenied = "SELLER_PERMISSION_DENIED";
        public const string CannotUpdateBlocked = "SELLER_CANNOT_UPDATE_BLOCKED";
    }

    public static class Product
    {
        public const string NotFound = "PRODUCT_NOT_FOUND";
        public const string SkuExists = "PRODUCT_SKU_EXISTS";
        public const string CategoryNotFound = "PRODUCT_CATEGORY_NOT_FOUND";
        public const string ImageNotFound = "PRODUCT_IMAGE_NOT_FOUND";
        public const string InvalidStatusTransition = "PRODUCT_INVALID_STATUS_TRANSITION";
        public const string CannotDeleteHasOrders = "PRODUCT_CANNOT_DELETE_HAS_ORDERS";
        public const string Blocked = "PRODUCT_BLOCKED";
    }

    public static class Category
    {
        public const string NotFound = "CATEGORY_NOT_FOUND";
        public const string ParentNotFound = "CATEGORY_PARENT_NOT_FOUND";
        public const string DuplicateName = "CATEGORY_DUPLICATE_NAME";
        public const string InvalidParent = "CATEGORY_INVALID_PARENT";
        public const string CannotDeleteHasChildren = "CATEGORY_CANNOT_DELETE_HAS_CHILDREN";
    }

    public static class Order
    {
        public const string NotFound = "ORDER_NOT_FOUND";
        public const string PermissionDenied = "ORDER_PERMISSION_DENIED";
        public const string InvalidStatusTransition = "ORDER_INVALID_STATUS_TRANSITION";
        public const string CannotModifyCompleted = "ORDER_CANNOT_MODIFY_COMPLETED";
    }

    public static class Notification
    {
        public const string NotFound = "NOTIFICATION_NOT_FOUND";
        public const string Unauthorized = "NOTIFICATION_UNAUTHORIZED";
    }

    public static class System
    {
        public const string ExternalLoginFailed = "SYSTEM_EXTERNAL_LOGIN_FAILED";
        public const string TokenGenerationFailed = "SYSTEM_TOKEN_GENERATION_FAILED";
        public const string UnexpectedError = "SYSTEM_UNEXPECTED_ERROR";
    }
}