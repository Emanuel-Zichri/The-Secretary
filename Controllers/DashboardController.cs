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
        [HttpPost("DeactivateCustomer")]
        public int DeactivateCustomer([FromBody] int customerID)
        {
            try
            {
                return Customer.Deactivate(customerID);
            }
            catch
            {
                return 0;
            }
        }
        [HttpPost("ReactivateCustomer")]
        public int ReactivateCustomer([FromBody] int customerID)
        {
            try
            {
                return Customer.Reactivate(customerID);
            }
            catch
            {
                return 0;
            }
        }
        [HttpPost("UpdateTimeWhenJobCompleted")]
        public int UpdateTimeWhenJobCompleted([FromBody] int customerID)
        {
            try
            {
                return WorkRequest.UpdateTimeWhenJobCompleted(customerID);
            }
            catch
            {
                return 0;
            }
        }

        [HttpPost("UpdateCustomerStatus")]
        public int UpdateCustomerStatus([FromBody] CustomerStatusUpdate statusUpdate)
        {
            try
            {
                return Customer.UpdateStatus(statusUpdate.CustomerID, statusUpdate.NewStatus);
            }
            catch
            {
                return 0;
            }
        }
    }

    public class CustomerStatusUpdate
    {
        public int CustomerID { get; set; }
        public string NewStatus { get; set; }
    }
}
