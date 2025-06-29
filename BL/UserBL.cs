using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalProject.BL
{
    /// <summary>
    /// מחלקת לוגיקה עסקית לניהול משתמשים ופרטי עסק
    /// </summary>
    public class UserBL
    {
        private DBservices dBservices;

        public UserBL()
        {
            dBservices = new DBservices();
        }

        /// <summary>
        /// קבלת פרטי העסק הראשי (המשתמש הפעיל הראשון)
        /// </summary>
        /// <returns>אובייקט User עם פרטי העסק או null אם לא נמצא</returns>
        public User GetPrimaryBusinessInfo()
        {
            try
            {
                var users = dBservices.GetAllUsers();
                return users?.FirstOrDefault(u => u.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בקבלת פרטי העסק: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// עדכון פרטי העסק
        /// </summary>
        /// <param name="user">פרטי המשתמש/עסק לעדכון</param>
        /// <returns>האם העדכון הצליח</returns>
        public bool UpdateBusinessInfo(User user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                // ולידציה בסיסית
                if (string.IsNullOrWhiteSpace(user.BusinessName))
                    throw new ArgumentException("שם העסק נדרש");

                if (!string.IsNullOrEmpty(user.BusinessEmail) && !IsValidEmail(user.BusinessEmail))
                    throw new ArgumentException("כתובת אימייל לא תקינה");

                user.UpdateModifiedTime();
                return dBservices.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בעדכון פרטי העסק: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// קבלת משתמש לפי ID
        /// </summary>
        /// <param name="userId">מזהה המשתמש</param>
        /// <returns>אובייקט User או null אם לא נמצא</returns>
        public User GetUserById(int userId)
        {
            try
            {
                return dBservices.GetUserById(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בקבלת פרטי משתמש: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// קבלת משתמש לפי שם משתמש
        /// </summary>
        /// <param name="username">שם המשתמש</param>
        /// <returns>אובייקט User או null אם לא נמצא</returns>
        public User GetUserByUsername(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return null;

                return dBservices.GetUserByUsername(username);
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בחיפוש משתמש: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// הוספת משתמש חדש
        /// </summary>
        /// <param name="user">פרטי המשתמש החדש</param>
        /// <returns>מזהה המשתמש החדש או -1 במקרה של כשלון</returns>
        public int AddUser(User user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                // ולידציה מלאה
                ValidateUser(user);

                // בדיקה שהמשתמש לא קיים
                var existingUser = GetUserByUsername(user.Username);
                if (existingUser != null)
                    throw new ArgumentException("שם המשתמש כבר קיים במערכת");

                return dBservices.AddUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בהוספת משתמש: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// מחיקה רכה של משתמש (הפיכה ללא פעיל)
        /// </summary>
        /// <param name="userId">מזהה המשתמש</param>
        /// <returns>האם המחיקה הצליחה</returns>
        public bool DeactivateUser(int userId)
        {
            try
            {
                var user = GetUserById(userId);
                if (user == null)
                    return false;

                user.IsActive = false;
                user.UpdateModifiedTime();
                
                return dBservices.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בהשבתת משתמש: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// קבלת כל המשתמשים הפעילים
        /// </summary>
        /// <returns>רשימת משתמשים פעילים</returns>
        public List<User> GetActiveUsers()
        {
            try
            {
                var allUsers = dBservices.GetAllUsers();
                return allUsers?.Where(u => u.IsActive).ToList() ?? new List<User>();
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בקבלת רשימת משתמשים: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// עדכון זמן התחברות אחרונה
        /// </summary>
        /// <param name="userId">מזהה המשתמש</param>
        /// <returns>האם העדכון הצליח</returns>
        public bool UpdateLastLogin(int userId)
        {
            try
            {
                var user = GetUserById(userId);
                if (user == null)
                    return false;

                user.UpdateLastLogin();
                return dBservices.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בעדכון זמן התחברות: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ולידציה מלאה של פרטי משתמש
        /// </summary>
        /// <param name="user">המשתמש לולידציה</param>
        private void ValidateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("שם משתמש נדרש");

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("סיסמה נדרשת");

            if (string.IsNullOrWhiteSpace(user.BusinessName))
                throw new ArgumentException("שם העסק נדרש");

            if (!string.IsNullOrEmpty(user.BusinessEmail) && !IsValidEmail(user.BusinessEmail))
                throw new ArgumentException("כתובת אימייל לא תקינה");

            if (user.Username.Length > 50)
                throw new ArgumentException("שם משתמש חייב להיות עד 50 תווים");

            if (user.BusinessName.Length > 100)
                throw new ArgumentException("שם העסק חייב להיות עד 100 תווים");
        }

        /// <summary>
        /// בדיקה האם כתובת אימייל תקינה
        /// </summary>
        /// <param name="email">כתובת האימייל לבדיקה</param>
        /// <returns>האם האימייל תקין</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// קבלת סטטיסטיקות על המשתמשים במערכת
        /// </summary>
        /// <returns>דיקטיונרי עם סטטיסטיקות</returns>
        public Dictionary<string, object> GetUserStatistics()
        {
            try
            {
                var allUsers = dBservices.GetAllUsers();
                if (allUsers == null || !allUsers.Any())
                    return new Dictionary<string, object>();

                return new Dictionary<string, object>
                {
                    ["TotalUsers"] = allUsers.Count,
                    ["ActiveUsers"] = allUsers.Count(u => u.IsActive),
                    ["InactiveUsers"] = allUsers.Count(u => !u.IsActive),
                    ["UsersWithEmail"] = allUsers.Count(u => !string.IsNullOrEmpty(u.BusinessEmail)),
                    ["LastUserAdded"] = allUsers.OrderByDescending(u => u.CreatedAt).FirstOrDefault()?.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בקבלת סטטיסטיקות: {ex.Message}", ex);
            }
        }
    }
} 