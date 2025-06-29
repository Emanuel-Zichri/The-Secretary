using FinalProject.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BIController : ControllerBase
    {
        private readonly BIBL _biBL;

        public BIController()
        {
            _biBL = new BIBL();
        }

        /// <summary>
        /// קבלת סטטיסטיקות BI מתקדמות
        /// </summary>
        [HttpPost("GetStatistics")]
        public ActionResult<object> GetStatistics([FromBody] object request)
        {
            try
            {
                // פרסי פרמטרי תאריך אם הם קיימים
                DateTime? fromDate = null;
                DateTime? toDate = null;

                Console.WriteLine("GetStatistics called");
                var statistics = _biBL.GetBIStatistics(fromDate, toDate);
                Console.WriteLine($"Statistics returned: {statistics?.Count} items");
                
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetStatistics: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// קבלת סיכום דשבורד למנהלים
        /// </summary>
        [HttpGet("GetDashboardSummary")]
        public ActionResult<object> GetDashboardSummary()
        {
            try
            {
                Console.WriteLine("GetDashboardSummary called");
                var summary = _biBL.GetDashboardSummary();
                Console.WriteLine($"Summary returned: {summary != null}");
                
                if (summary != null)
                {
                    Console.WriteLine($"Values: WaitingForDate={summary.WaitingForDate}, PendingInstalls={summary.PendingInstalls}");
                }
                
                return Ok(summary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDashboardSummary: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// קבלת נתונים מעובדים לגרפים
        /// </summary>
        [HttpPost("GetChartData")]
        public ActionResult<object> GetChartData([FromBody] BIFilterRequest request)
        {
            try
            {
                var chartData = _biBL.GetChartData(request?.FromDate, request?.ToDate);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting chart data: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת נתוני גרפים");
            }
        }

        /// <summary>
        /// קבלת דוח מכירות חודשי
        /// </summary>
        [HttpGet("GetMonthlySalesReport")]
        public ActionResult<object> GetMonthlySalesReport([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                Console.WriteLine($"GetMonthlySalesReport called with dates: {fromDate} - {toDate}");
                var monthlyData = _biBL.GetMonthlySalesReport(12, fromDate, toDate);
                Console.WriteLine($"Monthly sales data returned: {monthlyData?.Count} items");
                
                return Ok(monthlyData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMonthlySalesReport: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// קבלת דוח פניות לקוחות לפי חודשים
        /// </summary>
        [HttpGet("GetMonthlyRequestsReport")]
        public ActionResult<object> GetMonthlyRequestsReport([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                Console.WriteLine($"GetMonthlyRequestsReport called with dates: {fromDate} - {toDate}");
                var monthlyData = _biBL.GetMonthlyRequestsReport(12, fromDate, toDate);
                Console.WriteLine($"Monthly requests data returned: {monthlyData?.Count} items");
                
                return Ok(monthlyData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMonthlyRequestsReport: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// קבלת סטטוסי בקשות עבודה
        /// </summary>
        [HttpGet("GetWorkRequestStatuses")]
        public ActionResult<List<WorkRequestStatus>> GetWorkRequestStatuses()
        {
            try
            {
                var statuses = _biBL.GetWorkRequestStatuses();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting work request statuses: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בקבלת סטטוסי בקשות" });
            }
        }

        /// <summary>
        /// בדיקת תקינות נתוני המערכת
        /// </summary>
        [HttpGet("GetHealthCheck")]
        public ActionResult<object> GetHealthCheck()
        {
            try
            {
                var healthCheck = _biBL.GetDataHealthCheck();
                return Ok(healthCheck);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("GetRecentActivity")]
        public List<RecentActivity> GetRecentActivity([FromQuery] int limit = 10)
        {
            try
            {
                return RecentActivity.GetRecentActivity(limit);
            }
            catch
            {
                return new List<RecentActivity>();
            }
        }

        [HttpGet("GetUpcomingInstalls")]
        public List<UpcomingInstall> GetUpcomingInstalls([FromQuery] int days = 7)
        {
            try
            {
                return UpcomingInstall.GetUpcomingInstalls(days);
            }
            catch
            {
                return new List<UpcomingInstall>();
            }
        }
    }

    // מחלקות עזר
    public class BIFilterRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string FilterType { get; set; }
    }
} 