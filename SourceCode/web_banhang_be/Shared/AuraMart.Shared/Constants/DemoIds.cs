namespace AuraMart.Shared.Constants;

/// <summary>
/// GUIDs và thông tin tài khoản demo dùng chung giữa các microservices của AuraMart.
/// </summary>
public static class DemoIds
{
    // ===== Auth Credentials =====
    public const string AdminEmail = "admin@gmail.com";
    public const string AdminPassword = "admin123";

    public const string SellerEmail = "seller.demo@auramart.local";
    public const string Seller2Email = "seller2.demo@auramart.local";

    public const string BuyerEmail = "buyer.demo@auramart.local";
    public const string Buyer2Email = "buyer2.demo@auramart.local";

    public const string DemoPassword = "demo123456";

    // ===== Users =====
    public static readonly Guid AdminUserId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    public static readonly Guid SellerUserId = Guid.Parse("88888888-8888-8888-8888-888888888888");
    public static readonly Guid Seller2UserId = Guid.Parse("88888888-8888-8888-8888-888888888882");
    public static readonly Guid BuyerUserId = Guid.Parse("77777777-7777-7777-7777-777777777777");
    public static readonly Guid Buyer2UserId = Guid.Parse("77777777-7777-7777-7777-777777777772");

    // ===== Addresses =====
    public static readonly Guid SellerAddressId = Guid.Parse("88888888-8888-8888-8888-888888888881");
    public static readonly Guid Seller2AddressId = Guid.Parse("88888888-8888-8888-8888-888888888883");
    public static readonly Guid BuyerAddressId = Guid.Parse("77777777-7777-7777-7777-777777777771");
    public static readonly Guid Buyer2AddressId = Guid.Parse("77777777-7777-7777-7777-777777777773");

    // ===== Sellers & Applications =====
    public static readonly Guid SellerId = Guid.Parse("99999999-9999-9999-9999-999999999999");
    public static readonly Guid Seller2Id = Guid.Parse("99999999-9999-9999-9999-999999999992");
    public static readonly Guid PendingApplicationId = Guid.Parse("99999999-9999-9999-9999-999999999991");
    public static readonly Guid ApprovedApplicationId = Guid.Parse("99999999-9999-9999-9999-999999999993");

    // ===== Categories =====
    public static readonly Guid ElectronicsCategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid FashionCategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid HomeCategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid BeautyCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid SportsCategoryId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    // ===== Products =====
    public static readonly Guid IphoneProductId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid KeyboardProductId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-bbbbbbbbbbbb");
    public static readonly Guid MouseProductId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-cccccccccccc");
    public static readonly Guid TshirtProductId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid HoodieProductId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-cccccccccccc");
    public static readonly Guid AirFryerProductId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid DeskLampProductId = Guid.Parse("cccccccc-cccc-cccc-cccc-dddddddddddd");
    public static readonly Guid ThermosProductId = Guid.Parse("dddddddd-dddd-dddd-dddd-eeeeeeeeeeee");

    // ===== Product Images & Reviews =====
    public static readonly Guid IphoneImageId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    public static readonly Guid KeyboardImageId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-bbbbbbbbbbb1");
    public static readonly Guid MouseImageId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-ccccccccccc1");
    public static readonly Guid TshirtImageId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
    public static readonly Guid HoodieImageId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-ccccccccccc1");
    public static readonly Guid AirFryerImageId = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc1");
    public static readonly Guid DeskLampImageId = Guid.Parse("cccccccc-cccc-cccc-cccc-ddddddddddd1");
    public static readonly Guid ThermosImageId = Guid.Parse("dddddddd-dddd-dddd-dddd-eeeeeeeeeee1");

    public static readonly Guid IphoneReviewId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
    public static readonly Guid KeyboardReviewId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-bbbbbbbbbbb2");
    public static readonly Guid TshirtReviewId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");

    // ===== Carts & Cart Items =====
    public static readonly Guid BuyerCartId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static readonly Guid BuyerCartItemId = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd1");
    public static readonly Guid BuyerCartItemId2 = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd2");

    public static readonly Guid Buyer2CartId = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd3");
    public static readonly Guid Buyer2CartItemId1 = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd4");

    // ===== Orders & Order Items =====
    public static readonly Guid PaidOrderId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    public static readonly Guid PaidOrderItemId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee1");

    public static readonly Guid ProcessingOrderId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee4");
    public static readonly Guid ProcessingOrderItemId1 = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee5");
    public static readonly Guid ProcessingOrderItemId2 = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee6");

    public static readonly Guid PendingOrderId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee2");
    public static readonly Guid PendingOrderItemId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee3");
    public static readonly Guid PendingOrderItemId2 = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee7");

    public static readonly Guid CancelledOrderId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee8");
    public static readonly Guid CancelledOrderItemId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee9");

    // ===== Payments =====
    public static readonly Guid PaidPaymentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    public static readonly Guid ProcessingPaymentId = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff2");
    public static readonly Guid PendingPaymentId = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff3");

    // ===== Notifications =====
    public static readonly Guid BuyerNotificationId = Guid.Parse("12121212-1212-1212-1212-121212121212");
    public static readonly Guid BuyerNotification2Id = Guid.Parse("12121212-1212-1212-1212-121212121213");
    public static readonly Guid Buyer2NotificationId = Guid.Parse("12121212-1212-1212-1212-121212121214");

    public static readonly Guid SellerNotificationId = Guid.Parse("13131313-1313-1313-1313-131313131313");
    public static readonly Guid SellerNotification2Id = Guid.Parse("13131313-1313-1313-1313-131313131314");
    public static readonly Guid Seller2NotificationId = Guid.Parse("13131313-1313-1313-1313-131313131315");
}
