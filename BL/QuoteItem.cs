namespace FinalProject.BL
{
    public class QuoteItem
    {
        public int QuoteItemID { get; set; }
        public int QuoteID { get; set; }
        public int? CalculatorItemID { get; set; }
        public string CustomItemName { get; set; }
        public decimal PriceForItem { get; set; }
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
        public string Notes { get; set; }

        public QuoteItem() { }
        public int AddQuoteItem(QuoteItem item)
        {
            DBservices db = new DBservices();
            try
            {
                return db.AddQuoteItem(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting QuoteItem: {ex.Message}");
                return 0;
            }
        }
    }
}
