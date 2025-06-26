using FinalProject.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerFeedbackController : ControllerBase
    {
        /// <summary>
        /// שליחת משוב לקוח
        /// </summary>
        [HttpPost("SendFeedback")]
        public ActionResult<int> SendFeedback([FromBody] CustomerFeedbackRequest request)
        {
            try
            {
                if (request == null || request.CustomerID <= 0 || request.RequestID <= 0)
                {
                    return BadRequest("נתוני משוב לא תקינים");
                }

                var feedback = new CustomerFeedback
                {
                    CustomerID = request.CustomerID,
                    RequestID = request.RequestID,
                    Sent = true,
                    SentAt = DateTime.Now
                };

                var db = new DBservices();
                int result = db.InsertCustomerFeedback(feedback);
                
                if (result > 0)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "שגיאה בשליחת המשוב");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending feedback: {ex.Message}");
                return StatusCode(500, "שגיאה בשרת");
            }
        }

        /// <summary>
        /// קבלת משוב לקוח
        /// </summary>
        [HttpGet("GetFeedback/{customerID}")]
        public ActionResult<CustomerFeedback> GetFeedback(int customerID)
        {
            try
            {
                if (customerID <= 0)
                {
                    return BadRequest("מזהה לקוח לא תקין");
                }

                var db = new DBservices();
                var feedback = db.GetCustomerFeedback(customerID);
                
                if (feedback == null)
                {
                    return NotFound("משוב לא נמצא");
                }

                return Ok(feedback);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting feedback: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת המשוב");
            }
        }

        /// <summary>
        /// סימון משוב כנשלח
        /// </summary>
        [HttpPost("MarkAsSent")]
        public ActionResult<int> MarkAsSent([FromBody] int feedbackID)
        {
            try
            {
                if (feedbackID <= 0)
                {
                    return BadRequest("מזהה משוב לא תקין");
                }

                var db = new DBservices();
                int result = db.MarkFeedbackAsSent(feedbackID);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking feedback as sent: {ex.Message}");
                return StatusCode(500, "שגיאה בעדכון המשוב");
            }
        }
    }

    // מחלקות עזר
    public class CustomerFeedbackRequest
    {
        public int CustomerID { get; set; }
        public int RequestID { get; set; }
        public string Notes { get; set; }
    }
} 