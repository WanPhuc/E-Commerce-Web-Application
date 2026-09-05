using AuraMart.Shared.Constants;

namespace AuraMart.Shared.Seed;

/// <summary>
/// Dữ liệu mẫu mở rộng (Categories, Products, Addresses, Reviews, Orders) dùng chung cho các Service.
/// </summary>
public static class DemoSeedData
{
    public static class Categories
    {
        public static readonly (Guid Id, string Name) Electronics = (DemoIds.ElectronicsCategoryId, "Điện Tử & Công Nghệ");
        public static readonly (Guid Id, string Name) Fashion = (DemoIds.FashionCategoryId, "Thời Trang & Phụ Kiện");
        public static readonly (Guid Id, string Name) Home = (DemoIds.HomeCategoryId, "Nhà Cửa & Đời Sống");
        public static readonly (Guid Id, string Name) Beauty = (DemoIds.BeautyCategoryId, "Sức Khỏe & Sắc Đẹp");
        public static readonly (Guid Id, string Name) Sports = (DemoIds.SportsCategoryId, "Thể Thao & Dã Ngoại");
    }

    public static class Addresses
    {
        public static readonly (Guid Id, Guid UserId, string Name, string Phone, string Line, string Ward, string District, string City) Buyer1Address =
            (DemoIds.BuyerAddressId, DemoIds.BuyerUserId, "Demo Buyer", "0900000001", "12 Nguyễn Trãi", "Phường Bến Thành", "Quận 1", "TP. Hồ Chí Minh");

        public static readonly (Guid Id, Guid UserId, string Name, string Phone, string Line, string Ward, string District, string City) Buyer2Address =
            (DemoIds.Buyer2AddressId, DemoIds.Buyer2UserId, "Demo Buyer 2", "0900000003", "456 Lê Duẩn", "Phường Bến Nghé", "Quận 1", "TP. Hồ Chí Minh");

        public static readonly (Guid Id, Guid UserId, string Name, string Phone, string Line, string Ward, string District, string City) Seller1Address =
            (DemoIds.SellerAddressId, DemoIds.SellerUserId, "AuraMart Official Store", "0900000002", "88 Lê Lợi", "Phường Bến Nghé", "Quận 1", "TP. Hồ Chí Minh");

        public static readonly (Guid Id, Guid UserId, string Name, string Phone, string Line, string Ward, string District, string City) Seller2Address =
            (DemoIds.Seller2AddressId, DemoIds.Seller2UserId, "TechZone & Fashion Hub", "0900000004", "102 Hai Bà Trưng", "Phường Đa Kao", "Quận 1", "TP. Hồ Chí Minh");
    }
}
