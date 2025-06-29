using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalProject.BL
{
    /// <summary>
    /// מחלקת Business Logic עבור Business Intelligence
    /// </summary>
    public class BIBL
    {
        private readonly DBservices _db;

        public BIBL()
        {
            _db = new DBservices();
        }

        /// <summary>
        /// קבלת סטטיסטיקות BI מתקדמות
        /// </summary>
        public List<BIStatistic> GetBIStatistics(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                return _db.GetBIStatisticsSafe(fromDate, toDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BIBL.GetBIStatistics: {ex.Message}");
                return new List<BIStatistic>();
            }
        }

        /// <summary>
        /// קבלת סיכום דשבורד למנהלים
        /// </summary>
        public DashboardSummary GetDashboardSummary()
        {
            try
            {
                return _db.GetDashboardSummarySafe();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BIBL.GetDashboardSummary: {ex.Message}");
                return new DashboardSummary();
            }
        }

        /// <summary>
        /// קבלת דוח מכירות חודשי
        /// </summary>
        public List<MonthlySalesData> GetMonthlySalesReport(int monthsBack = 12, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                return _db.GetMonthlySalesReportSafe(monthsBack, fromDate, toDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BIBL.GetMonthlySalesReport: {ex.Message}");
                return new List<MonthlySalesData>();
            }
        }

        /// <summary>
        /// קבלת דוח פניות לקוחות לפי חודשים
        /// </summary>
        public List<MonthlyRequestsData> GetMonthlyRequestsReport(int monthsBack = 12, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                return _db.GetMonthlyRequestsReportSafe(monthsBack, fromDate, toDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BIBL.GetMonthlyRequestsReport: {ex.Message}");
                return new List<MonthlyRequestsData>();
            }
        }

        /// <summary>
        /// קבלת סטטוסי בקשות עבודה
        /// </summary>
        public List<WorkRequestStatus> GetWorkRequestStatuses()
        {
            try
            {
                return _db.GetWorkRequestStatusesSafe();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BIBL.GetWorkRequestStatuses: {ex.Message}");
                return new List<WorkRequestStatus>();
            }
        }

        /// <summary>
        /// קבלת נתונים מעובדים לגרפים
        /// </summary>
        public object GetChartData(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var statistics = GetBIStatistics(fromDate, toDate);

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

                return chartData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BIBL.GetChartData: {ex.Message}");
                return new { };
            }
        }

        /// <summary>
        /// בדיקת תקינות נתונים ומחזיר אזהרות במידת הצורך
        /// </summary>
        public BIHealthCheck GetDataHealthCheck()
        {
            try
            {
                var summary = GetDashboardSummary();
                var statistics = GetBIStatistics();

                var healthCheck = new BIHealthCheck
                {
                    IsHealthy = true,
                    Warnings = new List<string>(),
                    LastUpdated = DateTime.Now
                };

                // בדיקות תקינות
                if (summary.TotalActiveCustomers == 0)
                {
                    healthCheck.Warnings.Add("אין לקוחות פעילים במערכת");
                }

                if (!statistics.Any(s => s.StatType == "TotalQuotes" && s.StatValue > 0))
                {
                    healthCheck.Warnings.Add("אין הצעות מחיר במערכת");
                }

                if (summary.PendingQuotes > summary.TotalActiveRequests * 0.8m)
                {
                    healthCheck.Warnings.Add("יותר מ-80% מהבקשות ממתינות להצעת מחיר");
                }

                healthCheck.IsHealthy = healthCheck.Warnings.Count == 0;

                return healthCheck;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BIBL.GetDataHealthCheck: {ex.Message}");
                return new BIHealthCheck
                {
                    IsHealthy = false,
                    Warnings = new List<string> { "שגיאה בבדיקת תקינות הנתונים" },
                    LastUpdated = DateTime.Now
                };
            }
        }
    }

        /// <summary>
    /// מחלקה לבדיקת תקינות נתוני BI
    /// </summary>
    public class BIHealthCheck
    {
        public bool IsHealthy { get; set; }
        public List<string> Warnings { get; set; }
        public DateTime LastUpdated { get; set; }

        public BIHealthCheck()
        {
            Warnings = new List<string>();
        }
    }

    /// <summary>
    /// מחלקה לנתוני מכירות חודשיים
    /// </summary>
    public class MonthlySalesData
    {
        public string Month { get; set; }
        public string MonthName { get; set; }
        public decimal TotalQuotes { get; set; }
        public int TotalCustomers { get; set; }
        public int CompletedInstalls { get; set; }
        public decimal TotalArea { get; set; }
    }

    /// <summary>
    /// מחלקה לנתוני פניות חודשיים
    /// </summary>
    public class MonthlyRequestsData
    {
        public string Month { get; set; }
        public string MonthName { get; set; }
        public int NewRequests { get; set; }
        public int NewCustomers { get; set; }
        public int CompletedRequests { get; set; }
        public decimal TotalArea { get; set; }
    }
}  