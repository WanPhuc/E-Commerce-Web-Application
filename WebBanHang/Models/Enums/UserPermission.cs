using System;

[Flags]
public enum UserPermission : ulong
{
    // ===== BASIC =====
    None = 0,

    // ===== USER MANAGEMENT =====
    ViewUsers         = 1UL << 0,
    CreateUser        = 1UL << 1,
    EditUser          = 1UL << 2,
    DeleteUser        = 1UL << 3,

    // ===== ROLE MANAGEMENT =====
    ViewRoles         = 1UL << 4,
    CreateRole        = 1UL << 5,
    EditRole          = 1UL << 6,
    DeleteRole        = 1UL << 7,

    // ===== CATEGORY MANAGEMENT =====
    ViewCategories    = 1UL << 8,
    CreateCategory    = 1UL << 9,
    EditCategory      = 1UL << 10,
    DeleteCategory    = 1UL << 11,

    // ===== PRODUCT MANAGEMENT =====
    ViewProducts      = 1UL << 12,
    CreateProduct     = 1UL << 13,
    EditProduct       = 1UL << 14,
    DeleteProduct     = 1UL << 15,

    // ===== SELLER MANAGEMENT =====
    ViewSeller        = 1UL << 16,
    EditSeller        = 1UL << 17,

    // ===== SELLER APPLICATION =====
    CreateSellerApplication  = 1UL << 18,  // user apply seller
    ViewSellerApplications   = 1UL << 19,  // admin view
    ReviewSellerApplications = 1UL << 20,  // admin approve/deny

    // ===== ORDER MANAGEMENT =====
    ViewOrders        = 1UL << 21,
    CreateOrder       = 1UL << 22,  // user place order
    UpdateOrder       = 1UL << 23,  // seller updates
    CancelOrder       = 1UL << 24,  // only user if pending
    ViewOrderItems    = 1UL << 25,

    // ===== PAYMENT MANAGEMENT =====
    ViewPayments      = 1UL << 26,
    RefundPayment     = 1UL << 27,

    // ===== CART (USER) =====
    ViewCart          = 1UL << 28,
    AddToCart         = 1UL << 29,
    UpdateCart        = 1UL << 30,
    RemoveFromCart    = 1UL << 31,

    // ===== ADDRESS (USER) =====
    ViewAddresses     = 1UL << 32,
    CreateAddress     = 1UL << 33,
    EditAddress       = 1UL << 34,
    DeleteAddress     = 1UL << 35,

    // ===== DASHBOARD =====
    ViewAdminDashboard = 1UL << 36,
    ViewSellerDashboard = 1UL << 37,

    // =======================
    // ROLE TAGS (không phải quyền)
    // =======================
    AdminUser  = 1UL << 48,
    SellerUser = 1UL << 49,
    NormalUser = 1UL << 50,

    // =======================
    // ROLE PACKAGES
    // =======================

    // ADMIN
    Admin =
        AdminUser |
        ViewAdminDashboard |
        ViewUsers | CreateUser | EditUser | DeleteUser |
        ViewRoles | CreateRole | EditRole | DeleteRole |
        ViewCategories | CreateCategory | EditCategory | DeleteCategory |
        ViewProducts | CreateProduct | EditProduct | DeleteProduct |
        ViewSeller | EditSeller |
        ViewSellerApplications | ReviewSellerApplications |
        ViewOrders | UpdateOrder | CancelOrder | ViewOrderItems |
        ViewPayments | RefundPayment,

    // SELLER
    Seller =
        SellerUser |
        ViewSellerDashboard |
        ViewProducts | CreateProduct | EditProduct | DeleteProduct |
        ViewCategories |
        ViewOrders | UpdateOrder | ViewOrderItems,

    // NORMAL USER (KHÁCH)
    User =
        NormalUser |
        ViewProducts |
        ViewCategories |
        AddToCart | UpdateCart | RemoveFromCart |
        CreateOrder | ViewOrders | CancelOrder |
        ViewAddresses | CreateAddress | EditAddress | DeleteAddress |
        CreateSellerApplication
}
