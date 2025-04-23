using FinalProject.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParquetTypeController : ControllerBase
    {
        [HttpGet("GetAll")]
        public List<ParquetType> GetAllParquetTypes()
        {
            DBservices dbs = new DBservices();
            List<ParquetType> parquetTypes = dbs.GetAllParquetTypes();
            return parquetTypes;
        }
        [HttpPost("Add")]
        public int AddParquetType([FromBody] ParquetType parquetType)
        {
            int result = ParquetType.AddParquetType(parquetType);
            return result;

        }
        [HttpPut("Update")]
        public int UpdateParquetType([FromBody] ParquetType parquetType)
        {
            int result = ParquetType.updateParquetType(parquetType);
            return result;
        }
        [HttpDelete("Delete")]
        public int DeleteParquetType(int id)
        {
            DBservices dbs = new DBservices();
            int result = dbs.DeleteParquetType(id);
            return result;
        }


    }
}
