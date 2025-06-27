namespace FinalProject.BL
{
    public class UpcomingInstall
    {
        public int RequestID { get; set; }
        public string CustomerName { get; set; }
        public string Location { get; set; }
        public DateTime InstallDate { get; set; }
        public string TimeSlot { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
        public string FormattedDate { get; set; }

        public static List<UpcomingInstall> GetUpcomingInstalls(int days = 7)
        {
            DBservices dbs = new DBservices();
            return dbs.GetUpcomingInstallsSafe(days);
        }
    }
} 