public class AdminDashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalSellersThisMonth { get; set; }
    public int TotalSellersLastMonth { get; set; }
    public int TotalUsersThisMonth { get; set; }
    public int TotalUsersLastMonth { get; set; }
    public int TotalOrdersThisMonth { get; set; }
    public int TotalOrdersLastMonth { get; set; }
    

    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }

}
public enum DashboardRanger
{
    Week,
    Month,
    Year
}
public class DashboardChartPoinDto
{
    public string Label{ get; set; }="";
    public int Orders{ get; set;}
    public decimal Revenue{ get; set; }
    public int Users{ get; set; }
    public int Sellers{ get; set; }
}
public class DashboardChartDto
{
    public DashboardRanger Ranger{ get; set; }
    public List<DashboardChartPoinDto> Points{ get; set; }=new List<DashboardChartPoinDto>();
}