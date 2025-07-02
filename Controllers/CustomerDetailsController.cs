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
        /// העלאת קובץ וידאו זמני לשרת המקומי
        /// </summary>
        [HttpPost("UploadTempVideo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadTempVideo(IFormFile file)
        {
            try
            {
                Console.WriteLine($"🎬 Starting local video upload. File: {file?.FileName}, Size: {file?.Length}");
                
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine("❌ No file provided");
                    return BadRequest(new { success = false, message = "לא נבחר קובץ" });
                }

                // בדיקת גודל קובץ (מקסימום 50MB)
                if (file.Length > 50 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "הקובץ גדול מדי. מקסימום 50MB" });
                }

                // בדיקת סוג קובץ
                var allowedTypes = new[] { "video/mp4", "video/avi", "video/mov", "video/wmv", "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new { success = false, message = "סוג קובץ לא נתמך. רק וידאו ותמונות" });
                }

                // יצירת תיקיית uploads אם לא קיימת
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                    Console.WriteLine($"✅ Created uploads directory: {uploadsPath}");
                }

                // יצירת שם קובץ ייחודי
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"temp_{timestamp}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                Console.WriteLine($"💾 Saving file to: {filePath}");

                // שמירת הקובץ
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // יצירת URL לקובץ
                var videoUrl = $"/uploads/videos/{fileName}";
                
                Console.WriteLine($"✅ File uploaded successfully. URL: {videoUrl}");
                
                return Ok(new { 
                    success = true, 
                    videoUrl = videoUrl,
                    fileName = file.FileName,
                    size = file.Length
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error uploading video: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בהעלאת הקובץ" });
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
                    return Ok(new { success = true, videos = new List<string>() });
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
                if (request == null || request.RequestID <= 0 || string.IsNullOrWhiteSpace(request.NewStatus))
                {
                    return BadRequest(new { success = false, message = "נתונים לא תקינים" });
                }

                bool success = _customerBL.UpdateWorkRequestStatus(request.RequestID, request.NewStatus);
                
                if (success)
                {
                    return Ok(new { success = true, message = "הסטטוס עודכן בהצלחה" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "שגיאה בעדכון הסטטוס" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בעדכון הסטטוס" });
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

        [HttpGet("GetVideo/{fileName}")]
        public IActionResult GetVideo(string fileName)
        {
            try
            {
                Console.WriteLine($"🎥 בקשה לקובץ וידאו: {fileName}");
                
                if (string.IsNullOrEmpty(fileName))
                {
                    return BadRequest("שם קובץ לא תקין");
                }

                // בדיקת אבטחה - רק קבצים מתיקיית videos
                if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest("שם קובץ לא מותר");
                }

                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");
                var filePath = Path.Combine(uploadsPath, fileName);

                Console.WriteLine($"🔍 מחפש קובץ ב: {filePath}");

                if (!System.IO.File.Exists(filePath))
                {
                    Console.WriteLine($"❌ קובץ לא נמצא: {filePath}");
                    return NotFound($"קובץ {fileName} לא נמצא");
                }

                Console.WriteLine($"✅ קובץ נמצא, מגיש: {filePath}");

                var fileExtension = Path.GetExtension(fileName).ToLower();
                string contentType = fileExtension switch
                {
                    ".mp4" => "video/mp4",
                    ".avi" => "video/avi",
                    ".mov" => "video/quicktime",
                    ".wmv" => "video/x-ms-wmv",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(fileStream, contentType, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאה בהגשת קובץ {fileName}: {ex.Message}");
                return StatusCode(500, "שגיאה בהגשת הקובץ");
            }
        }
    }
} 