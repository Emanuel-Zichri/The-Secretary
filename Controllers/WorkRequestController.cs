using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalProject.BL;
namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkRequestController : ControllerBase
    {
        [HttpPost("UpdateStatus")]
        public int UpdateStatus( int workRequestID, string workRequestNewStatus)
        {
            try
            {
                return WorkRequest.UpdateStatus(workRequestID, workRequestNewStatus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status: {ex.Message}");
                return 0;
            }
        }
    }  
}
