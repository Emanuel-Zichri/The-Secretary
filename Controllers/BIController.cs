using FinalProject.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BIController : ControllerBase
    {
        /// <summary>
        /// קבלת סטטיסטיקות BI מתקדמות
        /// </summary>
        [HttpPost("GetStatistics")]
        public ActionResult<List<BIStatistic>> GetStatistics([FromBody] BIFilterRequest request)
        {
            try
            {
                var db = new DBservices();
                var statistics = db.GetBIStatisticsSafe(request?.FromDate, request?.ToDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting BI statistics: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת סטטיסטיקות");
            }
        }

        /// <summary>
        /// קבלת סיכום דשבורד למנהלים
        /// </summary>
        [HttpGet("GetDashboardSummary")]
        public ActionResult<DashboardSummary> GetDashboardSummary()
        {
            try
            {
                var db = new DBservices();
                var summary = db.GetDashboardSummarySafe();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting dashboard summary: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת סיכום דשבורד");
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
                var db = new DBservices();
                var statistics = db.GetBIStatisticsSafe(request?.FromDate, request?.ToDate);

                // עיבוד נתונים לפורמט גרפים
                var chartData = new
                {
                    StatusChart = statistics
                        .Where(s => s.StatType == "StatusCount")
                        .Select(s => new { Label = s.StatText, Value = s.StatCount })
                        .ToList(),

                    CityDistribution = statistics
                        .Where(s => s.StatType == "CityDistribution")
                        .Select(s => new { Label = s.StatText, Value = s.StatCount })
                        .ToList(),

                    ParquetTypeDistribution = statistics
                        .Where(s => s.StatType == "ParquetTypeDistribution")
                        .Select(s => new { Label = s.StatText, Value = s.StatCount })
                        .ToList(),

                    TotalQuotes = statistics.FirstOrDefault(s => s.StatType == "TotalQuotes")?.StatValue ?? 0,
                    TotalCustomers = statistics.FirstOrDefault(s => s.StatType == "TotalCustomers")?.StatCount ?? 0,
                    CompletedInstalls = statistics.FirstOrDefault(s => s.StatType == "CompletedInstalls")?.StatCount ?? 0,
                    TotalArea = statistics.FirstOrDefault(s => s.StatType == "TotalArea")?.StatValue ?? 0
                };

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
        public ActionResult<object> GetMonthlySalesReport()
        {
            try
            {
                var db = new DBservices();
                var monthlyData = db.GetMonthlySalesReportSafe(12);
                return Ok(monthlyData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting monthly sales report: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת דוח מכירות חודשי");
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
                var db = new DBservices();
                var statuses = db.GetWorkRequestStatusesSafe();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting work request statuses: {ex.Message}");
                return StatusCode(500, new { success = false, message = "שגיאה בקבלת סטטוסי בקשות" });
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