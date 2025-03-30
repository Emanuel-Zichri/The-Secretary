namespace FinalProject.BL
{
    public class Quote
    {
        public int QuoteID { get; set; }
        public int RequestID { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string ApprovalStatus { get; set; }
    }

}
