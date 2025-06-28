using FinalProject.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingsController : ControllerBase
    {
        /// <summary>
        /// קבלת הגדרות מערכת
        /// </summary>
        [HttpGet("GetSettings")]
        public ActionResult<List<SystemSetting>> GetSettings()
        {
            try
            {
                var db = new DBservices();
                var settings = db.GetSystemSettingsSafe();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting system settings: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת הגדרות מערכת");
            }
        }

        /// <summary>
        /// קבלת הגדרה ספציפית
        /// </summary>
        [HttpGet("GetSetting/{key}")]
        public ActionResult<SystemSetting> GetSetting(string key)
        {
            try
            {
                var db = new DBservices();
                var settings = db.GetSystemSettingsSafe(key);
                var setting = settings.FirstOrDefault();
                
                if (setting == null)
                {
                    return NotFound($"הגדרה לא נמצאה: {key}");
                }
                
                return Ok(setting);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting system setting: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת הגדרת מערכת");
            }
        }

        /// <summary>
        /// עדכון הגדרת מערכת
        /// </summary>
        [HttpPost("UpdateSetting")]
        public ActionResult UpdateSetting([FromBody] SystemSetting setting)
        {
            try
            {
                if (setting == null || string.IsNullOrEmpty(setting.SettingKey))
                {
                    return BadRequest("נתוני הגדרה לא תקינים");
                }

                var db = new DBservices();
                var result = db.UpdateSystemSettingSafe(setting);
                
                if (result > 0)
                {
                    return Ok("הגדרה עודכנה בהצלחה");
                }
                else
                {
                    return StatusCode(500, "שגיאה בעדכון הגדרה");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating system setting: {ex.Message}");
                return StatusCode(500, "שגיאה בעדכון הגדרת מערכת");
            }
        }

        /// <summary>
        /// קבלת מחירי פרקט
        /// </summary>
        [HttpGet("GetParquetPrices")]
        public ActionResult<object> GetParquetPrices()
        {
            try
            {
                var db = new DBservices();
                var settings = db.GetSystemSettingsSafe();
                
                var prices = new
                {
                    SPC = decimal.Parse(settings.FirstOrDefault(s => s.SettingKey == "SPC_PRICE")?.SettingValue ?? "60"),
                    Wood = decimal.Parse(settings.FirstOrDefault(s => s.SettingKey == "WOOD_PRICE")?.SettingValue ?? "85"),
                    Fishbone = decimal.Parse(settings.FirstOrDefault(s => s.SettingKey == "FISHBONE_PRICE")?.SettingValue ?? "150")
                };
                
                return Ok(prices);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting parquet prices: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת מחירי פרקט");
            }
        }

        /// <summary>
        /// עדכון מחירי פרקט
        /// </summary>
        [HttpPost("UpdateParquetPrices")]
        public ActionResult UpdateParquetPrices([FromBody] ParquetPricesRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("נתוני מחירים לא תקינים");
                }

                var db = new DBservices();
                
                // עדכון מחיר SPC
                if (request.SPCPrice > 0)
                {
                    var spcSetting = new SystemSetting
                    {
                        SettingKey = "SPC_PRICE",
                        SettingValue = request.SPCPrice.ToString(),
                        SettingType = "DECIMAL",
                        Description = "מחיר SPC/למינציה למ\"ר"
                    };
                    db.UpdateSystemSettingSafe(spcSetting);
                }

                // עדכון מחיר עץ
                if (request.WoodPrice > 0)
                {
                    var woodSetting = new SystemSetting
                    {
                        SettingKey = "WOOD_PRICE",
                        SettingValue = request.WoodPrice.ToString(),
                        SettingType = "DECIMAL",
                        Description = "מחיר עץ למ\"ר"
                    };
                    db.UpdateSystemSettingSafe(woodSetting);
                }

                // עדכון מחיר פישבון
                if (request.FishbonePrice > 0)
                {
                    var fishboneSetting = new SystemSetting
                    {
                        SettingKey = "FISHBONE_PRICE",
                        SettingValue = request.FishbonePrice.ToString(),
                        SettingType = "DECIMAL",
                        Description = "מחיר פישבון למ\"ר"
                    };
                    db.UpdateSystemSettingSafe(fishboneSetting);
                }

                return Ok("מחירי פרקט עודכנו בהצלחה");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating parquet prices: {ex.Message}");
                return StatusCode(500, "שגיאה בעדכון מחירי פרקט");
            }
        }

        /// <summary>
        /// קבלת פרטי עסק
        /// </summary>
        [HttpGet("GetBusinessInfo")]
        public ActionResult<object> GetBusinessInfo()
        {
            try
            {
                var db = new DBservices();
                var settings = db.GetSystemSettingsSafe();
                
                var businessInfo = new
                {
                    Phone = settings.FirstOrDefault(s => s.SettingKey == "BUSINESS_PHONE")?.SettingValue ?? "050-1234567",
                    Email = settings.FirstOrDefault(s => s.SettingKey == "BUSINESS_EMAIL")?.SettingValue ?? "info@davidparquet.co.il",
                    WorkingHours = settings.FirstOrDefault(s => s.SettingKey == "WORKING_HOURS")?.SettingValue ?? "א'-ה': 8:00-18:00 | ו': 8:00-14:00",
                    VATPercentage = decimal.Parse(settings.FirstOrDefault(s => s.SettingKey == "VAT_PERCENTAGE")?.SettingValue ?? "18")
                };
                
                return Ok(businessInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting business info: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת פרטי עסק");
            }
        }
    }

    // מחלקות עזר
    public class ParquetPricesRequest
    {
        public decimal SPCPrice { get; set; }
        public decimal WoodPrice { get; set; }
        public decimal FishbonePrice { get; set; }
    }
} 