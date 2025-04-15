namespace FinalProject.BL
{
    public class DashboardBL
    {
        public static List<DashboardResult> GetDashboardData(DashboardFilterDto filter)
        {
            DBservices dbServices = new DBservices();
            return (List<DashboardResult>)dbServices.GetDashboardData(filter);
        }
    }
}
