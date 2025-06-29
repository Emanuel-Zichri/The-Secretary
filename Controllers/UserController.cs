using Microsoft.AspNetCore.Mvc;
using FinalProject.BL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalProject.Controllers
{
    /// <summary>
    /// קונטרולר לניהול משתמשים ופרטי עסק
    /// מספק API endpoints למערכת ניהול גנרית וגמישה
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserBL userBL;

        public UserController()
        {
            userBL = new UserBL();
        }

        /// <summary>
        /// קבלת פרטי העסק הראשי
        /// </summary>
        /// <returns>פרטי העסק או שגיאה</returns>
        [HttpGet("GetBusinessInfo")]
        public IActionResult GetBusinessInfo()
        {
            try
            {
                var businessInfo = userBL.GetPrimaryBusinessInfo();
                
                if (businessInfo == null)
                {
                    return NotFound(new { message = "לא נמצאו פרטי עסק במערכת" });
                }

                // החזרת הנתונים ללא סיסמה מטעמי אבטחה
                var safeBusinessInfo = new
                {
                    businessInfo.UserID,
                    businessInfo.BusinessName,
                    businessInfo.BusinessPhone,
                    businessInfo.BusinessEmail,
                    businessInfo.WorkingHours,
                    businessInfo.AboutUs,
                    businessInfo.BusinessLogo,
                    businessInfo.IntroVideoURL,
                    businessInfo.IsActive,
                    businessInfo.CreatedAt,
                    businessInfo.UpdatedAt
                };

                return Ok(safeBusinessInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "שגיאה בקבלת פרטי העסק", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// עדכון פרטי העסק (ללא סיסמה)
        /// </summary>
        /// <param name="businessInfo">פרטי העסק לעדכון</param>
        /// <returns>תוצאת העדכון</returns>
        [HttpPost("UpdateBusinessInfo")]
        public IActionResult UpdateBusinessInfo([FromBody] BusinessInfoUpdateRequest businessInfo)
        {
            try
            {
                if (businessInfo == null)
                {
                    return BadRequest(new { message = "נתוני העסק נדרשים" });
                }

                // קבלת המשתמש הקיים
                var existingUser = userBL.GetPrimaryBusinessInfo();
                if (existingUser == null)
                {
                    return NotFound(new { message = "לא נמצא עסק לעדכון" });
                }

                // עדכון השדות המותרים בלבד
                existingUser.BusinessName = businessInfo.BusinessName ?? existingUser.BusinessName;
                existingUser.BusinessPhone = businessInfo.BusinessPhone ?? existingUser.BusinessPhone;
                existingUser.BusinessEmail = businessInfo.BusinessEmail ?? existingUser.BusinessEmail;
                existingUser.WorkingHours = businessInfo.WorkingHours ?? existingUser.WorkingHours;
                existingUser.AboutUs = businessInfo.AboutUs ?? existingUser.AboutUs;
                existingUser.BusinessLogo = businessInfo.BusinessLogo ?? existingUser.BusinessLogo;
                existingUser.IntroVideoURL = businessInfo.IntroVideoURL ?? existingUser.IntroVideoURL;

                var success = userBL.UpdateBusinessInfo(existingUser);
                
                if (success)
                {
                    return Ok(new { 
                        message = "פרטי העסק עודכנו בהצלחה",
                        updatedAt = existingUser.UpdatedAt
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "שגיאה בעדכון פרטי העסק" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "שגיאה בעדכון פרטי העסק", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// קבלת משתמש לפי ID
        /// </summary>
        /// <param name="id">מזהה המשתמש</param>
        /// <returns>פרטי המשתמש או שגיאה</returns>
        [HttpGet("GetUser/{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var user = userBL.GetUserById(id);
                
                if (user == null)
                {
                    return NotFound(new { message = "המשתמש לא נמצא" });
                }

                // החזרת נתונים בטוחים (ללא סיסמה)
                var safeUser = new
                {
                    user.UserID,
                    user.Username,
                    user.BusinessName,
                    user.BusinessPhone,
                    user.BusinessEmail,
                    user.WorkingHours,
                    user.AboutUs,
                    user.BusinessLogo,
                    user.IntroVideoURL,
                    user.IsActive,
                    user.CreatedAt,
                    user.UpdatedAt
                };

                return Ok(safeUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "שגיאה בקבלת פרטי משתמש", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// קבלת רשימת משתמשים פעילים
        /// </summary>
        /// <returns>רשימת משתמשים פעילים</returns>
        [HttpGet("GetActiveUsers")]
        public IActionResult GetActiveUsers()
        {
            try
            {
                var users = userBL.GetActiveUsers();
                
                // החזרת נתונים בטוחים (ללא סיסמאות)
                var safeUsers = users.Select(user => new
                {
                    user.UserID,
                    user.Username,
                    user.BusinessName,
                    user.BusinessPhone,
                    user.BusinessEmail,
                    user.WorkingHours,
                    user.IsActive,
                    user.CreatedAt
                }).ToList();

                return Ok(safeUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "שגיאה בקבלת רשימת משתמשים", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// הוספת משתמש חדש
        /// </summary>
        /// <param name="newUser">פרטי המשתמש החדש</param>
        /// <returns>מזהה המשתמש החדש או שגיאה</returns>
        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] User newUser)
        {
            try
            {
                if (newUser == null)
                {
                    return BadRequest(new { message = "פרטי המשתמש נדרשים" });
                }

                var userId = userBL.AddUser(newUser);
                
                if (userId > 0)
                {
                    return Ok(new { 
                        message = "המשתמש נוסף בהצלחה",
                        userId = userId
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "שגיאה בהוספת המשתמש" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "שגיאה בהוספת משתמש", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// השבתת משתמש (מחיקה רכה)
        /// </summary>
        /// <param name="id">מזהה המשתמש</param>
        /// <returns>תוצאת ההשבתה</returns>
        [HttpPost("DeactivateUser/{id}")]
        public IActionResult DeactivateUser(int id)
        {
            try
            {
                var success = userBL.DeactivateUser(id);
                
                if (success)
                {
                    return Ok(new { message = "המשתמש הושבת בהצלחה" });
                }
                else
                {
                    return NotFound(new { message = "המשתמש לא נמצא או כבר מושבת" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "שגיאה בהשבתת משתמש", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// קבלת סטטיסטיקות על המשתמשים במערכת
        /// </summary>
        /// <returns>סטטיסטיקות משתמשים</returns>
        [HttpGet("GetStatistics")]
        public IActionResult GetStatistics()
        {
            try
            {
                var statistics = userBL.GetUserStatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "שגיאה בקבלת סטטיסטיקות", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// עדכון זמן התחברות אחרונה
        /// </summary>
        /// <param name="id">מזהה המשתמש</param>
        /// <returns>תוצאת העדכון</returns>
        [HttpPost("UpdateLastLogin/{id}")]
        public IActionResult UpdateLastLogin(int id)
        {
            try
            {
                var success = userBL.UpdateLastLogin(id);
                
                if (success)
                {
                    return Ok(new { message = "זמן התחברות עודכן בהצלחה" });
                }
                else
                {
                    return NotFound(new { message = "המשתמש לא נמצא" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "שגיאה בעדכון זמן התחברות", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "User Controller works!" });
        }
    }

    /// <summary>
    /// מחלקת בקשה לעדכון פרטי עסק (ללא שדות רגישים)
    /// </summary>
    public class BusinessInfoUpdateRequest
    {
        public string BusinessName { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessEmail { get; set; }
        public string WorkingHours { get; set; }
        public string AboutUs { get; set; }
        public string BusinessLogo { get; set; }
        public string IntroVideoURL { get; set; }
    }
} 