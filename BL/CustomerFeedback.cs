namespace FinalProject.BL
{
    public class CustomerFeedback
    {
        public int FeedbackID { get; set; }
        public int CustomerID { get; set; }
        public int RequestID { get; set; }
        public bool Sent { get; set; }
        public DateTime? SentAt { get; set; }
    }

}
