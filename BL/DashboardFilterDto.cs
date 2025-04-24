namespace FinalProject.BL
{
    public class DashboardFilterDto
    {
        public int? CustomerID { get; set; }
        public string? City { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? FloorType { get; set; }
        public string? Status { get; set; }
    }
}
