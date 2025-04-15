namespace FinalProject.BL
{
    public class DashboardResult
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public DateTime CustomerCreatedAt { get; set; }
        public int RequestID { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; }
        public int? SpaceCount { get; set; }
        public decimal? TotalSpaceSize { get; set; }

        // במצב של לקוח בודד עם פירוט חללים
        public int? SpaceID { get; set; }
        public decimal? Size { get; set; }
        public string FloorType { get; set; }
        public string Parquet { get; set; }
        public string SpaceNotes { get; set; }
    }

}
