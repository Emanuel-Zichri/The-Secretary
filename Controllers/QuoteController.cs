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
    }
}
