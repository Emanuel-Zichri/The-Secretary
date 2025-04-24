namespace FinalProject.BL
{
    public class QuoteItemExtended
    {
        public int QuoteItemID { get; set; }
        public int QuoteID { get; set; }
        public int RequestID { get; set; }

        public int? CalculatorItemID { get; set; }
        public string CustomItemName { get; set; }
        public decimal PriceForItem { get; set; }
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
        public string Notes { get; set; }

        public DateTime? PlannedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
