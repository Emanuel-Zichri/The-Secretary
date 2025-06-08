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
            
            int newCustomerID = Customer.Register(newReq.customerDetails);

           
            int workRequestID = WorkRequest.Register(newCustomerID, newReq.PreferredDate,newReq.PreferredSlot);

            
            foreach (var space in newReq.spaceDetails)
            {
                SpaceDetails.Register(workRequestID, space);
            }

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



