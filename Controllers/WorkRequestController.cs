using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalProject.BL;
namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkRequestController : ControllerBase
    {
        [HttpPost("UpdateStatus")]
        public int UpdateStatus( int workRequestID, string workRequestNewStatus)
        {
            try
            {
                return WorkRequest.UpdateStatus(workRequestID, workRequestNewStatus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status: {ex.Message}");
                return 0;
            }
        }

        [HttpGet("GetLatestByCustomerID/{customerID}")]
        public ActionResult<WorkRequest> GetLatestByCustomerID(int customerID)
        {
            try
            {
                var workRequest = WorkRequest.GetLatestByCustomerID(customerID);
                if (workRequest == null)
                {
                    return NotFound("בקשת עבודה לא נמצאה");
                }
                return Ok(workRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting work request: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת בקשת העבודה");
            }
        }

        [HttpGet("GetByRequestID/{requestID}")]
        public ActionResult<WorkRequest> GetByRequestID(int requestID)
        {
            try
            {
                var workRequest = WorkRequest.GetByRequestID(requestID);
                if (workRequest == null)
                {
                    return NotFound("בקשת עבודה לא נמצאה");
                }
                return Ok(workRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting work request: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת בקשת העבודה");
            }
        }
    }  
}
