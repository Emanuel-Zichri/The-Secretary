using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalProject.BL;
namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        [HttpPost("AssignRequestToSlot")]
        public int AssignRequestToSlot([FromBody] ScheduleSlotAssignment assignment)
        {
            try
            {
                return Schedule.AssignRequestToSlot(assignment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning request to slot: {ex.Message}");
                return 0;
            }
        }
    }
}
