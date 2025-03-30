using FinalProject.BL;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpaceDetailsController : ControllerBase
    {
        [HttpPost("Register")]
        public int Register([FromBody] SpaceDetails space)
        {
            return SpaceDetails.Register(space);
        }
    }

}

