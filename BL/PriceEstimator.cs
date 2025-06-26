namespace FinalProject.BL
{
    public class PriceEstimator
    {
        public int EstimateID { get; set; }
        public int RequestID { get; set; }
        public decimal TotalArea { get; set; }
        public string ParquetType { get; set; }
        public int RoomCount { get; set; }
        public decimal BasePrice { get; set; }
        public decimal EstimatedMinPrice { get; set; }
        public decimal EstimatedMaxPrice { get; set; }
        public int EstimatedMinDays { get; set; }
        public int EstimatedMaxDays { get; set; }
        public decimal ComplexityMultiplier { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Notes { get; set; }

        public PriceEstimator() { }

        /// <summary>
        /// חישוב הערכת מחיר וזמן על בסיס פרמטרים מתקדמים
        /// </summary>
        public static PriceEstimator CalculateEstimate(int requestID, decimal totalArea, string parquetType, int roomCount, List<SpaceDetails> spaceDetails)
        {
            var estimate = new PriceEstimator
            {
                RequestID = requestID,
                TotalArea = totalArea,
                ParquetType = parquetType ?? "סטנדרט",
                RoomCount = roomCount,
                CreatedAt = DateTime.Now
            };

            // מחיר בסיס לפי סוג פרקט
            decimal basePricePerM2 = GetBasePriceByType(parquetType);
            estimate.BasePrice = totalArea * basePricePerM2;

            // חישוב מכפיל מורכבות
            estimate.ComplexityMultiplier = CalculateComplexityMultiplier(spaceDetails, roomCount);

            // חישוב מחיר סופי
            decimal finalBasePrice = estimate.BasePrice * estimate.ComplexityMultiplier;
            estimate.EstimatedMinPrice = Math.Round(finalBasePrice * 0.85m, 0);
            estimate.EstimatedMaxPrice = Math.Round(finalBasePrice * 1.15m, 0);

            // חישוב זמן התקנה
            int baseDays = CalculateBaseDays(totalArea, roomCount);
            estimate.EstimatedMinDays = Math.Max(1, baseDays - 1);
            estimate.EstimatedMaxDays = baseDays + 2;

            // הוספת הערות
            estimate.Notes = GenerateEstimateNotes(estimate.ComplexityMultiplier, roomCount, spaceDetails);

            return estimate;
        }

        /// <summary>
        /// קביעת מחיר בסיס לפי סוג פרקט - עם קבלת מחירים מהמסד נתונים
        /// </summary>
        private static decimal GetBasePriceByType(string parquetType)
        {
            try
            {
                // קבלת מחירים מהמסד נתונים
                DBservices db = new DBservices();
                var settings = db.GetSystemSettings(null);
                
                var spcPrice = decimal.Parse(settings.FirstOrDefault(s => s.SettingKey == "DEFAULT_PARQUET_PRICE_SPC")?.SettingValue ?? "60");
                var woodPrice = decimal.Parse(settings.FirstOrDefault(s => s.SettingKey == "DEFAULT_PARQUET_PRICE_WOOD")?.SettingValue ?? "85");
                var fishbonePrice = decimal.Parse(settings.FirstOrDefault(s => s.SettingKey == "DEFAULT_PARQUET_PRICE_FISHBONE")?.SettingValue ?? "150");
                
                if (string.IsNullOrEmpty(parquetType)) return spcPrice;

                string type = parquetType.ToLower();
                
                // עץ מלא - המחיר הגבוה ביותר
                if (type.Contains("עץ מלא") || type.Contains("אלון") || type.Contains("אגוז")) return woodPrice;
                
                // פישבון - מורכב להתקנה
                if (type.Contains("פישבון") || type.Contains("הרינגבון")) return fishbonePrice;
                
                // עץ הנדסי - איכות בינונית-גבוהה
                if (type.Contains("הנדסי") || type.Contains("עץ")) return woodPrice * 0.85m;
                
                // למינציה - איכות בינונית
                if (type.Contains("למינציה") || type.Contains("למינט")) return spcPrice;
                
                // ויניל/PVC - הזול ביותר
                if (type.Contains("ויניל") || type.Contains("pvc") || type.Contains("פי.וי.סי")) return spcPrice * 0.8m;
                
                // ברירת מחדל
                return spcPrice;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting prices from DB, using defaults: {ex.Message}");
                
                // ברירת מחדל במקרה של שגיאה
                if (string.IsNullOrEmpty(parquetType)) return 55;

                string type = parquetType.ToLower();
                
                if (type.Contains("עץ מלא") || type.Contains("אלון") || type.Contains("אגוז")) return 85;
                if (type.Contains("פישבון") || type.Contains("הרינגבון")) return 95;
                if (type.Contains("הנדסי") || type.Contains("עץ")) return 70;
                if (type.Contains("למינציה") || type.Contains("למינט")) return 50;
                if (type.Contains("ויניל") || type.Contains("pvc") || type.Contains("פי.וי.סי")) return 40;
                
                return 60;
            }
        }

        /// <summary>
        /// חישוב מכפיל מורכבות על בסיס פרטי החללים
        /// </summary>
        private static decimal CalculateComplexityMultiplier(List<SpaceDetails> spaceDetails, int roomCount)
        {
            decimal multiplier = 1.0m;

            if (spaceDetails != null && spaceDetails.Count > 0)
            {
                // מכפיל לפי כמות חדרים
                if (roomCount > 3) multiplier += 0.08m;
                if (roomCount > 5) multiplier += 0.12m;

                // מכפיל לפי גודל חללים קטנים (מתחת ל-12 מ"ר)
                int smallRooms = spaceDetails.Count(s => s.Size < 12);
                multiplier += smallRooms * 0.06m;

                // מכפיל לפי סוגי רצפה שונים (צריך פירוק שונה)
                var floorTypes = spaceDetails.Select(s => s.FloorType).Where(f => !string.IsNullOrEmpty(f)).Distinct().Count();
                if (floorTypes > 1) multiplier += 0.1m;

                // בונוס מורכבות לפי הערות מיוחדות
                bool hasSpecialRequirements = spaceDetails.Any(s => 
                    !string.IsNullOrEmpty(s.Notes) && 
                    (s.Notes.Contains("מדרגות") || s.Notes.Contains("עמודים") || s.Notes.Contains("זוויות")));
                
                if (hasSpecialRequirements) multiplier += 0.15m;
            }
            else
            {
                // אם אין פרטי חללים, מכפיל בסיסי לפי כמות חדרים בלבד
                if (roomCount > 3) multiplier += 0.1m;
                if (roomCount > 5) multiplier += 0.1m;
            }

            return Math.Min(multiplier, 1.6m); // מגביל לעד 60% תוספת
        }

        /// <summary>
        /// חישוב ימי עבודה בסיסיים
        /// </summary>
        private static int CalculateBaseDays(decimal totalArea, int roomCount)
        {
            // 20 מ"ר ביום כבסיס, פחות ביעילות עם יותר חדרים
            decimal dailyCapacity = Math.Max(12, 20 - (roomCount * 1.5m));
            return (int)Math.Ceiling(totalArea / dailyCapacity);
        }

        /// <summary>
        /// יצירת הערות להערכה
        /// </summary>
        private static string GenerateEstimateNotes(decimal complexityMultiplier, int roomCount, List<SpaceDetails> spaceDetails)
        {
            var notes = new List<string>();

            if (complexityMultiplier > 1.3m)
                notes.Add("פרויקט מורכב - דורש תכנון מדויק");
            
            if (roomCount > 5)
                notes.Add("מספר חדרים גבוה - זמן התקנה ארוך יותר");

            if (spaceDetails?.Any(s => s.Size < 10) == true)
                notes.Add("כולל חללים קטנים - עלות נוספת להתקנה");

            return notes.Count > 0 ? string.Join("; ", notes) : "הערכה סטנדרטית";
        }

        /// <summary>
        /// שמירת הערכה בבסיס נתונים
        /// </summary>
        public static int SaveEstimate(PriceEstimator estimate)
        {
            if (estimate == null)
                throw new ArgumentNullException(nameof(estimate));

            DBservices db = new DBservices();
            try
            {
                return db.SavePriceEstimate(estimate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving price estimate: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// קבלת הערכה לפי RequestID
        /// </summary>
        public static PriceEstimator GetEstimateByRequestID(int requestID)
        {
            if (requestID <= 0)
                throw new ArgumentException("RequestID must be greater than 0", nameof(requestID));

            DBservices db = new DBservices();
            try
            {
                return db.GetPriceEstimateByRequestID(requestID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting price estimate: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// חישוב מהיר של הערכת מחיר על בסיס נתונים בסיסיים בלבד
        /// </summary>
        public static (decimal minPrice, decimal maxPrice, int minDays, int maxDays) QuickEstimate(decimal totalArea, string parquetType, int roomCount = 1)
        {
            decimal basePricePerM2 = GetBasePriceByType(parquetType);
            decimal basePrice = totalArea * basePricePerM2;
            
            // מכפיל בסיסי לפי חדרים
            decimal multiplier = 1.0m;
            if (roomCount > 3) multiplier += 0.1m;
            if (roomCount > 5) multiplier += 0.1m;

            decimal finalPrice = basePrice * multiplier;
            decimal minPrice = Math.Round(finalPrice * 0.85m, 0);
            decimal maxPrice = Math.Round(finalPrice * 1.15m, 0);

            int baseDays = CalculateBaseDays(totalArea, roomCount);
            int minDays = Math.Max(1, baseDays - 1);
            int maxDays = baseDays + 2;

            return (minPrice, maxPrice, minDays, maxDays);
        }
    }
} 