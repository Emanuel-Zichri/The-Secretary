namespace FinalProject.BL
{
    public class RecentActivity
    {
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string Location { get; set; }
        public DateTime ActivityDate { get; set; }
        public string RelativeTime { get; set; }
        public string IconType { get; set; }
        public string ColorClass { get; set; }

        public static List<RecentActivity> GetRecentActivity(int limit = 10)
        {
            DBservices dbs = new DBservices();
            return dbs.GetRecentActivitySafe(limit);
        }
    }
} 