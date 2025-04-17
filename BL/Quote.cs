namespace FinalProject.BL
{
    public class Quote
    {
        public int QuoteID { get; set; }
        public int RequestID { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        

        public static int InsertQuote(Quote quote)
        {
            DBservices db = new DBservices();
            try
            {
                return db.InsertQuote(quote);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting Quote: {ex.Message}");
                return 0;
            }
        }
    }


}
