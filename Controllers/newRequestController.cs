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
            int workRequestID= WorkRequest.Register(newCustomerID);
            foreach (var space in newReq.spaceDetails)
            {
                SpaceDetails.Register(workRequestID, space);
            }

            return workRequestID;


        }

    }
    
}
