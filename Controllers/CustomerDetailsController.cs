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
        [HttpPost("UploadTempVideo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadTempVideo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "לא נבחר קובץ" });
                }

                var driveService = GoogleDriveHelper.GetDriveService();
                var uploadedFile = await GoogleDriveHelper.UploadFileAsync(driveService, file, "TemporaryUploads");

                if (uploadedFile == null)
                {
                    return StatusCode(500, new { success = false, message = "שגיאה בהעלאה לדרייב" });
                }

                return Ok(new { success = true, videoUrl = uploadedFile.WebViewLink });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading temp video: {ex}");
                return StatusCode(500, new { success = false, message = "שגיאה בשרת" });
            }
        }

        [HttpGet("GetCustomerVideos/{customerID}")]
        public ActionResult GetCustomerVideos(int customerID)
        {
            try
            {
                var videos = _customerBL.GetCustomerVideos(customerID);
                if (videos == null || videos.Count == 0)
                {
                    return NotFound(new { success = false, message = "לא נמצאו סרטונים ללקוח" });
                }

                return Ok(new { success = true, videos = videos });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCustomerVideos: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בקבלת סרטונים" });
            }
        }




        [HttpPost("UpdateSpaceVideoLink")]
        public ActionResult UpdateSpaceVideoLink([FromBody] UpdateSpaceVideoRequest request)
        {
            if (request == null || request.SpaceID <= 0 || string.IsNullOrWhiteSpace(request.VideoLink))
            {
                return BadRequest(new { success = false, message = "נתונים לא תקינים" });
            }

            bool success = _customerBL.UpdateSpaceVideo(request.SpaceID, request.VideoLink);

            if (success)
            {
                return Ok(new { success = true, message = "הלינק עודכן בהצלחה" });
            }
            else
            {
                return BadRequest(new { success = false, message = "שגיאה בעדכון הלינק" });
            }
        }

        // מחלקת עזר לבקשה
        public class UpdateSpaceVideoRequest
        {
            public int SpaceID { get; set; }
            public string VideoLink { get; set; }
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