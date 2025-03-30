using FinalProject.BL;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkRequestController : ControllerBase
    {
        [HttpPost("Register")]
        public int Register([FromBody] WorkRequest request)
        {
            return WorkRequest.Register(request);
        }
    }

}
