namespace VanFucVN.Core.Common.Constants;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Seller = "Seller";
    public const string Buyer = "Buyer";

    public static readonly Dictionary<string, int> RoleWeights = new()
    {
        { Admin, 100 },
        { Seller, 50 },
        { Buyer, 10 }
    };
}
