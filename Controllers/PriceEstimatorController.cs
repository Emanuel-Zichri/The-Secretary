using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalProject.BL;
using System.Data.SqlClient;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceEstimatorController : ControllerBase
    {
        /// <summary>
        /// חישוב הערכת מחיר מלאה על בסיס פרטי הבקשה
        /// </summary>
        [HttpPost("CalculateEstimate")]
        public ActionResult<PriceEstimator> CalculateEstimate([FromBody] EstimateRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("נתוני בקשה חסרים");
                }

                var estimate = PriceEstimator.CalculateEstimate(
                    request.RequestID,
                    request.TotalArea,
                    request.ParquetType,
                    request.RoomCount,
                    request.SpaceDetails
                );

                return Ok(estimate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating estimate: {ex.Message}");
                return StatusCode(500, "שגיאה בחישוב הערכת המחיר");
            }
        }

        /// <summary>
        /// שמירת הערכת מחיר בבסיס הנתונים
        /// </summary>
        [HttpPost("SaveEstimate")]
        public ActionResult<int> SaveEstimate([FromBody] PriceEstimator estimate)
        {
            try
            {
                if (estimate == null)
                {
                    return BadRequest("נתוני הערכה חסרים");
                }

                int estimateID = PriceEstimator.SaveEstimate(estimate);
                return Ok(estimateID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving estimate: {ex.Message}");
                return StatusCode(500, "שגיאה בשמירת ההערכה");
            }
        }

        /// <summary>
        /// קבלת הערכה לפי מזהה בקשה
        /// </summary>
        [HttpGet("GetEstimateByRequestID/{requestID}")]
        public ActionResult<PriceEstimator> GetEstimateByRequestID(int requestID)
        {
            try
            {
                if (requestID <= 0)
                {
                    return BadRequest("מזהה בקשה לא תקין");
                }

                var estimate = PriceEstimator.GetEstimateByRequestID(requestID);
                
                if (estimate == null)
                {
                    return NotFound("הערכה לא נמצאה");
                }

                return Ok(estimate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting estimate: {ex.Message}");
                return StatusCode(500, "שגיאה בקבלת ההערכה");
            }
        }

        /// <summary>
        /// חישוב מהיר של הערכת מחיר בלי שמירה בDB
        /// </summary>
        [HttpPost("QuickEstimate")]
        public ActionResult<QuickEstimateResponse> QuickEstimate([FromBody] QuickEstimateRequest request)
        {
            try
            {
                if (request == null || request.TotalArea <= 0)
                {
                    return BadRequest("נתוני בקשה לא תקינים");
                }

                var (minPrice, maxPrice, minDays, maxDays) = PriceEstimator.QuickEstimate(
                    request.TotalArea,
                    request.ParquetType,
                    request.RoomCount
                );

                var response = new QuickEstimateResponse
                {
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    MinDays = minDays,
                    MaxDays = maxDays,
                    PriceRange = $"{minPrice:N0} - {maxPrice:N0} ש\"ח",
                    DurationRange = $"{minDays} - {maxDays} ימים"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in quick estimate: {ex.Message}");
                return StatusCode(500, "שגיאה בחישוב הערכה מהירה");
            }
        }

        /// <summary>
        /// חישוב והערכה מלאה + שמירה בפעולה אחת
        /// </summary>
        [HttpPost("CalculateAndSave")]
        public ActionResult<EstimateResponse> CalculateAndSave([FromBody] EstimateRequest request)
        {
            try
            {
                Console.WriteLine($"🔄 CalculateAndSave נקרא עם RequestID: {request?.RequestID}");
                
                if (request == null)
                {
                    Console.WriteLine("❌ Request is null");
                    return BadRequest("נתוני בקשה חסרים");
                }

                Console.WriteLine($"📊 פרטי הבקשה:");
                Console.WriteLine($"   - RequestID: {request.RequestID}");
                Console.WriteLine($"   - TotalArea: {request.TotalArea}");
                Console.WriteLine($"   - ParquetType: {request.ParquetType}");
                Console.WriteLine($"   - RoomCount: {request.RoomCount}");
                Console.WriteLine($"   - SpaceDetails count: {request.SpaceDetails?.Count ?? 0}");

                // בדיקה שה-RequestID קיים בטבלה
                try
                {
                    var db = new DBservices();
                    var workRequest = db.GetWorkRequestByID(request.RequestID);
                    if (workRequest == null)
                    {
                        Console.WriteLine($"❌ RequestID {request.RequestID} לא קיים בטבלת WorkRequest");
                        return BadRequest($"RequestID {request.RequestID} לא תקף - הבקשה לא נמצאה במערכת");
                    }
                    Console.WriteLine($"✅ RequestID {request.RequestID} נמצא בטבלה, CustomerID: {workRequest.CustomerID}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ שגיאה בבדיקת RequestID: {ex.Message}");
                    return StatusCode(500, "שגיאה בבדיקת תקינות RequestID");
                }

                // חישוב ההערכה
                Console.WriteLine("🧮 מתחיל חישוב הערכה...");
                var estimate = PriceEstimator.CalculateEstimate(
                    request.RequestID,
                    request.TotalArea,
                    request.ParquetType,
                    request.RoomCount,
                    request.SpaceDetails
                );

                Console.WriteLine($"✅ הערכה חושבה:");
                Console.WriteLine($"   - BasePrice: {estimate.BasePrice}");
                Console.WriteLine($"   - EstimatedMinPrice: {estimate.EstimatedMinPrice}");
                Console.WriteLine($"   - EstimatedMaxPrice: {estimate.EstimatedMaxPrice}");
                Console.WriteLine($"   - ComplexityMultiplier: {estimate.ComplexityMultiplier}");

                // שמירה בDB
                Console.WriteLine("💾 מתחיל שמירת הערכה במסד נתונים...");
                int estimateID = PriceEstimator.SaveEstimate(estimate);
                Console.WriteLine($"✅ הערכה נשמרה בהצלחה עם EstimateID: {estimateID}");
                
                estimate.EstimateID = estimateID;

                var response = new EstimateResponse
                {
                    EstimateID = estimateID,
                    Estimate = estimate,
                    Success = true,
                    Message = "הערכה חושבה ונשמרה בהצלחה"
                };

                Console.WriteLine($"🎉 CalculateAndSave הושלם בהצלחה, EstimateID: {estimateID}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאה בCalculateAndSave: {ex.Message}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                
                // בדיקה אם זו שגיאת FOREIGN KEY
                if (ex.Message.Contains("FOREIGN KEY constraint"))
                {
                    Console.WriteLine("❌ זוהתה שגיאת FOREIGN KEY - RequestID לא תקף");
                    return BadRequest("RequestID לא תקף במסד הנתונים");
                }
                
                return StatusCode(500, $"שגיאה בחישוב ושמירת ההערכה: {ex.Message}");
            }
        }

        /// <summary>
        /// בדיקת קיום הטבלה - לצורכי דיבוג
        /// </summary>
        [HttpGet("TestTableExists")]
        public ActionResult<string> TestTableExists()
        {
            try
            {
                var db = new DBservices();
                // ננסה להריץ שאילתה פשוטה על הטבלה
                var con = db.connect("myProjDB");
                var cmd = new SqlCommand("SELECT COUNT(*) FROM PriceEstimates", con);
                var count = cmd.ExecuteScalar();
                con.Close();
                
                return Ok($"הטבלה קיימת, מכילה {count} רשומות");
            }
            catch (Exception ex)
            {
                return Ok($"שגיאה: {ex.Message}");
            }
        }

        /// <summary>
        /// בדיקת קיום הפרוצדורה - לצורכי דיבוג
        /// </summary>
        [HttpGet("TestProcedureExists")]
        public ActionResult<string> TestProcedureExists()
        {
            try
            {
                var db = new DBservices();
                var con = db.connect("myProjDB");
                var cmd = new SqlCommand("SELECT COUNT(*) FROM sys.procedures WHERE name = 'SavePriceEstimate'", con);
                var count = cmd.ExecuteScalar();
                con.Close();
                
                return Ok($"הפרוצדורה SavePriceEstimate {(Convert.ToInt32(count) > 0 ? "קיימת" : "לא קיימת")} (מציאות: {count})");
            }
            catch (Exception ex)
            {
                return Ok($"שגיאה בבדיקת פרוצדורה: {ex.Message}");
            }
        }

        /// <summary>
        /// בדיקה מפורטת של שמירת הערכה - לצורכי דיבוג
        /// </summary>
        [HttpPost("TestSaveEstimate")]
        public ActionResult<string> TestSaveEstimate()
        {
            try
            {
                // קבלת RequestID תקף מהמסד נתונים
                var db = new DBservices();
                var con = db.connect("myProjDB");
                var cmd = new SqlCommand("SELECT TOP 1 RequestID FROM WorkRequest ORDER BY RequestID DESC", con);
                var validRequestID = cmd.ExecuteScalar();
                con.Close();
                
                if (validRequestID == null)
                {
                    return Ok("❌ אין RequestID במסד הנתונים");
                }

                int requestID = Convert.ToInt32(validRequestID);
                Console.WriteLine($"🔍 משתמש ב-RequestID: {requestID} לבדיקה");

                // יצירת הערכה לבדיקה עם RequestID תקף
                var testEstimate = new PriceEstimator
                {
                    RequestID = requestID,
                    TotalArea = 50,
                    ParquetType = "למינציה",
                    RoomCount = 2,
                    BasePrice = 2500,
                    EstimatedMinPrice = 2125,
                    EstimatedMaxPrice = 2875,
                    EstimatedMinDays = 2,
                    EstimatedMaxDays = 4,
                    ComplexityMultiplier = 1.0m,
                    Notes = "בדיקה אוטומטית"
                };

                int result = db.SavePriceEstimate(testEstimate);
                
                return Ok($"✅ השמירה הצליחה! RequestID: {requestID}, EstimateID: {result}");
            }
            catch (Exception ex)
            {
                return Ok($"❌ שגיאה מפורטת: {ex.Message}\n\nStack Trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// קבלת RequestID תקף לבדיקה
        /// </summary>
        [HttpGet("GetValidRequestID")]
        public ActionResult<string> GetValidRequestID()
        {
            try
            {
                var db = new DBservices();
                var con = db.connect("myProjDB");
                var cmd = new SqlCommand("SELECT TOP 1 RequestID FROM WorkRequest ORDER BY RequestID DESC", con);
                var requestID = cmd.ExecuteScalar();
                con.Close();
                
                if (requestID != null)
                {
                    return Ok($"RequestID תקף: {requestID}");
                }
                else
                {
                    return Ok("אין RequestID במסד הנתונים");
                }
            }
            catch (Exception ex)
            {
                return Ok($"שגיאה: {ex.Message}");
            }
        }

        /// <summary>
        /// בדיקה מפורטת עם RequestID אמיתי - לצורכי דיבוג
        /// </summary>
        [HttpPost("DebugEstimate")]
        public ActionResult<string> DebugEstimate([FromBody] EstimateRequest request)
        {
            try
            {
                var debugInfo = new List<string>();
                
                debugInfo.Add($"RequestID: {request.RequestID}");
                debugInfo.Add($"TotalArea: {request.TotalArea}");
                debugInfo.Add($"ParquetType: {request.ParquetType}");
                debugInfo.Add($"RoomCount: {request.RoomCount}");
                debugInfo.Add($"SpaceDetails count: {request.SpaceDetails?.Count ?? 0}");

                // בדיקה אם RequestID קיים בטבלת WorkRequest
                var db = new DBservices();
                var con = db.connect("myProjDB");
                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM WorkRequest WHERE RequestID = @RequestID", con);
                checkCmd.Parameters.AddWithValue("@RequestID", request.RequestID);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                con.Close();
                
                debugInfo.Add($"RequestID exists in WorkRequest: {exists > 0}");

                if (exists == 0)
                {
                    return Ok($"שגיאה: RequestID {request.RequestID} לא קיים בטבלת WorkRequest!\n" + string.Join("\n", debugInfo));
                }

                // ניסיון חישוב הערכה
                var estimate = PriceEstimator.CalculateEstimate(
                    request.RequestID,
                    request.TotalArea,
                    request.ParquetType,
                    request.RoomCount,
                    request.SpaceDetails
                );
                
                debugInfo.Add($"Estimate calculated successfully:");
                debugInfo.Add($"- BasePrice: {estimate.BasePrice}");
                debugInfo.Add($"- EstimatedMinPrice: {estimate.EstimatedMinPrice}");
                debugInfo.Add($"- EstimatedMaxPrice: {estimate.EstimatedMaxPrice}");
                debugInfo.Add($"- ComplexityMultiplier: {estimate.ComplexityMultiplier}");

                // ניסיון שמירה
                int estimateID = PriceEstimator.SaveEstimate(estimate);
                debugInfo.Add($"Estimate saved successfully with EstimateID: {estimateID}");

                return Ok("הכל עבד בהצלחה!\n" + string.Join("\n", debugInfo));
            }
            catch (Exception ex)
            {
                return Ok($"שגיאה: {ex.Message}\n\nStack Trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// בדיקת מה יש בטבלת WorkRequest - לצורכי דיבוג
        /// </summary>
        [HttpGet("CheckWorkRequests")]
        public ActionResult<string> CheckWorkRequests()
        {
            try
            {
                var db = new DBservices();
                var con = db.connect("myProjDB");
                var cmd = new SqlCommand("SELECT TOP 5 RequestID, CustomerID, Status, CreatedAt FROM WorkRequest ORDER BY CreatedAt DESC", con);
                var reader = cmd.ExecuteReader();
                
                var results = new List<string>();
                results.Add("5 WorkRequest האחרונים:");
                
                while (reader.Read())
                {
                    results.Add($"RequestID: {reader["RequestID"]}, CustomerID: {reader["CustomerID"]}, Status: {reader["Status"]}, CreatedAt: {reader["CreatedAt"]}");
                }
                
                reader.Close();
                con.Close();
                
                return Ok(string.Join("\n", results));
            }
            catch (Exception ex)
            {
                return Ok($"שגיאה: {ex.Message}");
            }
        }

        /// <summary>
        /// בדיקת יצירת WorkRequest חדש - לאבחון בעיות
        /// </summary>
        [HttpPost("TestCreateWorkRequest")]
        public ActionResult<string> TestCreateWorkRequest()
        {
            try
            {
                var results = new List<string>();
                var db = new DBservices();
                
                // מציאת לקוח פעיל לבדיקה
                var con = db.connect("myProjDB");
                var customerCmd = new SqlCommand("SELECT TOP 1 CustomerID, FirstName, LastName FROM Customer WHERE IsActive = 1", con);
                var customerReader = customerCmd.ExecuteReader();
                
                if (!customerReader.Read())
                {
                    customerReader.Close();
                    con.Close();
                    return Ok("❌ אין לקוחות פעילים לבדיקה");
                }
                
                int testCustomerID = (int)customerReader["CustomerID"];
                string customerName = $"{customerReader["FirstName"]} {customerReader["LastName"]}";
                customerReader.Close();
                con.Close();
                
                results.Add($"🔍 משתמש בלקוח לבדיקה: {customerName} (ID: {testCustomerID})");
                
                // יצירת WorkRequest חדש
                DateTime testDate = DateTime.Now.AddDays(7);
                int testSlot = 1;
                
                results.Add($"📅 תאריך מועדף: {testDate}");
                results.Add($"🕐 משבצת זמן: {testSlot}");
                
                int newRequestID = db.InsertWorkRequest(testCustomerID, testDate, testSlot);
                
                results.Add($"✅ WorkRequest נוצר בהצלחה!");
                results.Add($"🆔 RequestID חדש: {newRequestID}");
                
                // אימות שהרשומה נוצרה
                con = db.connect("myProjDB");
                var verifyCmd = new SqlCommand(@"
                    SELECT RequestID, CustomerID, Status, CreatedAt, PreferredDate, PreferredSlot 
                    FROM WorkRequest 
                    WHERE RequestID = @RequestID", con);
                verifyCmd.Parameters.AddWithValue("@RequestID", newRequestID);
                var verifyReader = verifyCmd.ExecuteReader();
                
                if (verifyReader.Read())
                {
                    results.Add($"✅ אימות: הרשומה נמצאה במסד הנתונים");
                    results.Add($"   RequestID: {verifyReader["RequestID"]}");
                    results.Add($"   CustomerID: {verifyReader["CustomerID"]}");
                    results.Add($"   Status: {verifyReader["Status"]}");
                    results.Add($"   CreatedAt: {verifyReader["CreatedAt"]}");
                    results.Add($"   PreferredDate: {verifyReader["PreferredDate"]}");
                    results.Add($"   PreferredSlot: {verifyReader["PreferredSlot"]}");
                }
                else
                {
                    results.Add($"❌ שגיאה: הרשומה לא נמצאה במסד הנתונים!");
                }
                
                verifyReader.Close();
                con.Close();
                
                return Ok(string.Join("\n", results));
            }
            catch (Exception ex)
            {
                return Ok($"❌ שגיאה: {ex.Message}\n\nStack Trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// בדיקת IDENTITY של WorkRequest
        /// </summary>
        [HttpGet("CheckWorkRequestIdentity")]
        public ActionResult<string> CheckWorkRequestIdentity()
        {
            try
            {
                var db = new DBservices();
                var con = db.connect("myProjDB");
                var results = new List<string>();
                
                // בדיקת IDENTITY
                var identityCmd = new SqlCommand(@"
                    SELECT 
                        IDENT_CURRENT('WorkRequest') AS CurrentIdentity,
                        IDENT_SEED('WorkRequest') AS SeedValue,
                        IDENT_INCR('WorkRequest') AS IncrementValue", con);
                
                var identityReader = identityCmd.ExecuteReader();
                if (identityReader.Read())
                {
                    results.Add($"IDENTITY נוכחי: {identityReader["CurrentIdentity"]}");
                    results.Add($"Seed: {identityReader["SeedValue"]}");
                    results.Add($"Increment: {identityReader["IncrementValue"]}");
                }
                identityReader.Close();
                
                // בדיקת מספר רשומות
                var countCmd = new SqlCommand(@"
                    SELECT COUNT(*) AS WorkRequestCount,
                           MIN(RequestID) AS MinRequestID,
                           MAX(RequestID) AS MaxRequestID
                    FROM WorkRequest", con);
                
                var countReader = countCmd.ExecuteReader();
                if (countReader.Read())
                {
                    results.Add($"סה\"כ WorkRequest: {countReader["WorkRequestCount"]}");
                    results.Add($"טווח RequestID: {countReader["MinRequestID"]} - {countReader["MaxRequestID"]}");
                }
                countReader.Close();
                
                con.Close();
                
                return Ok(string.Join("\n", results));
            }
            catch (Exception ex)
            {
                return Ok($"שגיאה: {ex.Message}");
            }
        }
    }

    // מחלקות עזר עבור הAPI
    public class EstimateRequest
    {
        public int RequestID { get; set; }
        public decimal TotalArea { get; set; }
        public string ParquetType { get; set; }
        public int RoomCount { get; set; }
        public List<SpaceDetails> SpaceDetails { get; set; }
    }

    public class QuickEstimateRequest
    {
        public decimal TotalArea { get; set; }
        public string ParquetType { get; set; }
        public int RoomCount { get; set; } = 1;
    }

    public class QuickEstimateResponse
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int MinDays { get; set; }
        public int MaxDays { get; set; }
        public string PriceRange { get; set; }
        public string DurationRange { get; set; }
    }

    public class EstimateResponse
    {
        public int EstimateID { get; set; }
        public PriceEstimator Estimate { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
} 