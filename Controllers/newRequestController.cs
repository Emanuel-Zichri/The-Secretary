using FinalProject.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class newRequestController : ControllerBase
    {
        [HttpPost("RegisterNewRequest")]
        public int RegisterNewRequest([FromBody] NewRequest newReq)
        {
            Console.WriteLine("🔄 RegisterNewRequest נקרא");
            Console.WriteLine($"📅 תאריך מועדף: {newReq.PreferredDate}");
            Console.WriteLine($"🕐 משבצת זמן: {newReq.PreferredSlot}");
            Console.WriteLine($"🏠 מספר חללים: {newReq.spaceDetails.Length}");
            
            // רישום לקוח חדש
            Console.WriteLine("👤 רושם לקוח חדש...");
            int newCustomerID = Customer.Register(newReq.customerDetails);
            Console.WriteLine($"✅ לקוח נרשם בהצלחה, CustomerID: {newCustomerID}");

            if (newCustomerID <= 0)
            {
                Console.WriteLine("❌ רישום לקוח נכשל");
                return 0;
            }

            // רישום בקשת עבודה
            Console.WriteLine("📋 רושם בקשת עבודה...");
            int workRequestID = WorkRequest.Register(newCustomerID, newReq.PreferredDate, newReq.PreferredSlot);
            Console.WriteLine($"✅ בקשת עבודה נרשמה בהצלחה, WorkRequestID: {workRequestID}");

            if (workRequestID <= 0)
            {
                Console.WriteLine("❌ רישום בקשת עבודה נכשל");
                return 0;
            }

            // רישום פרטי חללים
            Console.WriteLine("🏠 רושם פרטי חללים...");
            foreach (var space in newReq.spaceDetails)
            {
                Console.WriteLine($"   - חלל בגודל {space.Size} מ\"ר, סוג רצפה: {space.FloorType}");
                SpaceDetails.Register(workRequestID, space);
            }
            Console.WriteLine("✅ כל פרטי החללים נרשמו");

            Console.WriteLine($"🎉 RegisterNewRequest הושלם בהצלחה, מחזיר WorkRequestID: {workRequestID}");
            return workRequestID;
        }

        [HttpPost("updateRequest")]
        public int UpdateRequest([FromBody] NewRequest reqToUpdate)
        {
            bool customerUpdated = false;
            bool spacesUpdated = false;

            int result = Customer.UpdateCustomer(reqToUpdate.customerDetails);
            if (result == 1)
            {
                customerUpdated = true;
            }

            foreach (var space in reqToUpdate.spaceDetails)
            {
                SpaceDetails spaceDetailsInstance = new SpaceDetails();
                result = spaceDetailsInstance.UpdateSpaceDetails(space);
                if (result == 1)
                {
                    spacesUpdated = true;
                }
                else if (space.SpaceID > 0) // נשלח מזהה לגיטימי אך לא הצליח לעדכן
                {
                    return 0; // כישלון חלקי - עדיף לעצור פה
                }
            }

            if (customerUpdated && spacesUpdated)
                return 2; // הצליח גם לקוח וגם חללים
            if (customerUpdated)
                return 1; // הצליח רק לקוח
            if (spacesUpdated)
                return 3; // הצליח רק חללים

            return 0; // לא הצליח כלום
        }

    }
}



