using FinalProject.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static FinalProject.BL.CalculatorItemCandidate;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceCalculatorItemController : ControllerBase
    {
        [HttpPost("AddNewCalculatorItem")]
        public int AddNewCalculatorItem([FromBody] PriceCalculatorItem newItem)
        {
            try
            {
                int newItemId = PriceCalculatorItem.AddCalcItem(newItem);
                return newItemId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        [HttpDelete("DeleteCalculatorItem")]
        public int DeleteCalculatorItem([FromBody] PriceCalculatorItem itemID)
        {
            try
            {
                int result = PriceCalculatorItem.DeleteCalcItem(itemID);
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        [HttpPost("updateCalculatorItem")]
        public int UpdateCalculatorItem([FromBody] PriceCalculatorItem item)
        {
            try
            {
                int result = PriceCalculatorItem.UpdateCalcItem(item);
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        [HttpGet("GetCalculatorItems")]
        public List<PriceCalculatorItem> GetCalculatorItems()
        {
            try
            {
                DBservices db = new DBservices();
                List<PriceCalculatorItem> items = db.GetCalculatorItems();
                return items;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpGet("GetPopularCandidates")]
        public List<CalculatorItemCandidate> GetPopularCandidates()
        {
            return CalculatorItemCandidate.GetCandidates();
        }
        [HttpPost("CreateFromCandidate")]
        public int CreateFromCandidate([FromBody] string customItemName)
        {
            try
            {
                int result = CalculatorItemCandidate.AddCalcItem(customItemName);
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        [HttpPost("RejectCandidate")]
        public int RejectCandidate([FromBody] string customItemName)
        {
            try
            {
                int result = CalculatorItemCandidate.RejectCandidate(customItemName);
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


    }
}