using System;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.BL
{
    /// <summary>
    /// מחלקה המייצגת משתמש ופרטי עסק במערכת
    /// מאפשרת גמישות לתמיכה במספר עסקים בעתיד
    /// </summary>
    public class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "שם משתמש נדרש")]
        [StringLength(50, ErrorMessage = "שם משתמש חייב להיות עד 50 תווים")]
        public string Username { get; set; }

        [Required(ErrorMessage = "סיסמה נדרשת")]
        [StringLength(255, ErrorMessage = "סיסמה חייבת להיות עד 255 תווים")]
        public string Password { get; set; }

        // פרטי העסק הכלליים
        [Required(ErrorMessage = "שם העסק נדרש")]
        [StringLength(100, ErrorMessage = "שם העסק חייב להיות עד 100 תווים")]
        public string BusinessName { get; set; }

        [StringLength(20, ErrorMessage = "מספר טלפון חייב להיות עד 20 תווים")]
        public string BusinessPhone { get; set; }

        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        [StringLength(100, ErrorMessage = "כתובת אימייל חייבת להיות עד 100 תווים")]
        public string BusinessEmail { get; set; }

        [StringLength(200, ErrorMessage = "שעות פעילות חייבות להיות עד 200 תווים")]
        public string WorkingHours { get; set; }

        // תוכן דינמי לעמוד הבית
        public string AboutUs { get; set; }

        [StringLength(500, ErrorMessage = "נתיב לוגו חייב להיות עד 500 תווים")]
        public string BusinessLogo { get; set; }

        [StringLength(500, ErrorMessage = "קישור סרטון חייב להיות עד 500 תווים")]
        public string IntroVideoURL { get; set; }

        // מטא-נתונים
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        // קונסטרקטור ברירת מחדל
        public User()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            IsActive = true;
        }

        // קונסטרקטור עם פרמטרים בסיסיים
        public User(string username, string password, string businessName)
        {
            Username = username;
            Password = password;
            BusinessName = businessName;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            IsActive = true;
        }

        /// <summary>
        /// עדכון זמן התחברות אחרונה
        /// </summary>
        public void UpdateLastLogin()
        {
            LastLogin = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// עדכון זמן שינוי אחרון
        /// </summary>
        public void UpdateModifiedTime()
        {
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// בדיקה האם המשתמש פעיל
        /// </summary>
        public bool IsUserActive()
        {
            return IsActive;
        }

        /// <summary>
        /// קבלת שם הקובץ של הלוגו (ללא הנתיב המלא)
        /// </summary>
        public string GetLogoFileName()
        {
            if (string.IsNullOrEmpty(BusinessLogo))
                return "logo.png"; // ברירת מחדל

            return BusinessLogo.Contains("/") ? 
                BusinessLogo.Substring(BusinessLogo.LastIndexOf("/") + 1) : 
                BusinessLogo;
        }
    }
} 