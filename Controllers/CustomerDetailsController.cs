using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalProject.BL;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDetailsController : ControllerBase
    {
        private readonly CustomerDetailsBL _customerBL;

        public CustomerDetailsController()
        {
            _customerBL = new CustomerDetailsBL();
        }

        /// <summary>
        /// קבלת כל פרטי הלקוח
        /// </summary>
        [HttpGet("GetCustomerDetails/{customerID}")]
        public ActionResult<CustomerDetailsResponse> GetCustomerDetails(int customerID)
        {
            try
            {
                if (customerID <= 0)
                {
                    return BadRequest(new { success = false, message = "מזהה לקוח לא תקין" });
                }

                var customerDetails = _customerBL.GetCustomerDetails(customerID);
                
                if (customerDetails == null)
                {
                    return NotFound(new { success = false, message = "לקוח לא נמצא" });
                }

                return Ok(customerDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting customer details: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בקבלת פרטי הלקוח" });
            }
        }

        /// <summary>
        /// עדכון פרטי לקוח
        /// </summary>
        [HttpPost("UpdateCustomerDetails/{customerID}")]
        public ActionResult UpdateCustomerDetails(int customerID, [FromBody] UpdateCustomerDetailsRequest request)
        {
            try
            {
                if (customerID <= 0 || request == null)
                {
                    return BadRequest(new { success = false, message = "נתונים לא תקינים" });
                }

                bool success = _customerBL.UpdateCustomerDetails(customerID, request.Customer, request.SpaceDetails);
                
                if (success)
                {
                    return Ok(new { success = true, message = "פרטי הלקוח עודכנו בהצלחה" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "שגיאה בעדכון פרטי הלקוח" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer details: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בעדכון פרטי הלקוח" });
            }
        }

        /// <summary>
        /// עדכון סטטוס בקשת עבודה
        /// </summary>
        [HttpPost("UpdateWorkRequestStatus")]
        public ActionResult UpdateWorkRequestStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                if (request == null || request.RequestID <= 0 || string.IsNullOrEmpty(request.NewStatus))
                {
                    return BadRequest(new { success = false, message = "נתוני בקשה לא תקינים" });
                }

                bool success = _customerBL.UpdateWorkRequestStatus(request.RequestID, request.NewStatus);
                
                if (success)
                {
                    return Ok(new { success = true, message = "סטטוס עודכן בהצלחה" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "שגיאה בעדכון הסטטוס" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating work request status: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בעדכון סטטוס בקשת העבודה" });
            }
        }
    }

    // מחלקות עזר
    public class UpdateCustomerDetailsRequest
    {
        public Customer Customer { get; set; }
        public List<SpaceDetails> SpaceDetails { get; set; }
    }

    public class UpdateStatusRequest
    {
        public int RequestID { get; set; }
        public string NewStatus { get; set; }
    }
} 