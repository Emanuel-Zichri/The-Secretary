using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalProject.BL;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly QuoteBL _quoteBL;

        public QuoteController()
        {
            _quoteBL = new QuoteBL();
        }

        /// <summary>
        /// יצירת הצעת מחיר חדשה
        /// </summary>
        [HttpPost("CreateQuote")]
        public ActionResult<QuoteCreationResult> CreateQuote([FromBody] CreateQuoteRequest request)
        {
            try
            {
                if (request == null || request.RequestID <= 0)
                {
                    return BadRequest(new { success = false, message = "נתוני בקשה לא תקינים" });
                }

                var result = _quoteBL.CreateQuote(request);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating quote: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה ביצירת הצעת המחיר" });
            }
        }

        /// <summary>
        /// חישוב הצעת מחיר (ללא שמירה)
        /// </summary>
        [HttpPost("CalculateQuote")]
        public ActionResult<CalculatedQuote> CalculateQuote([FromBody] CreateQuoteRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "נתוני בקשה לא תקינים" });
                }

                var result = _quoteBL.CalculateQuote(request);
                
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(new { success = false, message = "שגיאה בחישוב הצעת המחיר" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating quote: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בחישוב הצעת המחיר" });
            }
        }

        /// <summary>
        /// קבלת הצעות מחיר של לקוח
        /// </summary>
        [HttpGet("GetCustomerQuotes/{customerID}")]
        public ActionResult<List<QuoteSummary>> GetCustomerQuotes(int customerID)
        {
            try
            {
                if (customerID <= 0)
                {
                    return BadRequest(new { success = false, message = "מזהה לקוח לא תקין" });
                }

                var quotes = _quoteBL.GetCustomerQuotes(customerID);
                return Ok(quotes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting customer quotes: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בקבלת הצעות מחיר" });
            }
        }

        /// <summary>
        /// קבלת פריטי מחשבון זמינים
        /// </summary>
        [HttpGet("GetCalculatorItems")]
        public ActionResult<List<CalculatorItem>> GetCalculatorItems()
        {
            try
            {
                var items = _quoteBL.GetCalculatorItems();
                return Ok(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting calculator items: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בקבלת פריטי מחשבון" });
            }
        }

        /// <summary>
        /// מחיקת הצעת מחיר
        /// </summary>
        [HttpDelete("DeleteQuote/{quoteID}")]
        public ActionResult DeleteQuote(int quoteID)
        {
            try
            {
                if (quoteID <= 0)
                {
                    return BadRequest(new { success = false, message = "מזהה הצעת מחיר לא תקין" });
                }

                var result = _quoteBL.DeleteQuote(quoteID);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting quote: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה במחיקת הצעת המחיר" });
            }
        }

        // שמירה על backward compatibility
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
