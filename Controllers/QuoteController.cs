using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalProject.BL;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        [HttpPost("InsertQuote")]
        public int InsertQuote([FromBody] Quote quote)
        {
            try
            {
                return Quote.InsertQuote(quote);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting quote: {ex.Message}");
                return 0;
            }
        }
        [HttpPost("AddQuoteItem")]
        public int AddQuoteItem([FromBody] List<QuoteItem> items)
        {
            int counter = 0;
            foreach (var item in items)
            {
                QuoteItem quoteItem = new QuoteItem();
                int result = quoteItem.AddQuoteItem(item);
                if (result > 0)
                {
                    counter++;
                }
            }
            return counter;
        }
        
        [HttpGet("GetFullQuoteByCustomerID/{customerID}")]
        public List<QuoteItemExtended> GetFullQuoteByCustomerID(int customerID)
        {
            try
            {
                DBservices db = new DBservices();
                return db.GetQuoteItemExtendedByCustomerID(customerID);
            }
            catch
            {
                return new List<QuoteItemExtended>(); 
            }
        }

    } 
}
