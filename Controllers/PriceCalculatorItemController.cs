using FinalProject.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}