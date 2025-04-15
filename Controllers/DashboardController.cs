using FinalProject.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        [HttpPost("GetDashboardData")]
        public object GetDashboardData([FromBody] DashboardFilterDto filter)
        {
            try
            {
                DBservices dbs = new DBservices();
                return dbs.GetDashboardData(filter);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }
    }
}
