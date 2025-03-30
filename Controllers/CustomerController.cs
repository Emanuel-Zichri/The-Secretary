using FinalProject;
using FinalProject.BL;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {


        [HttpPost("Register")]
        public int Register([FromBody] Customer newCustomer)
        {
            int result = Customer.Register(newCustomer);
            return result;
        }

        //[HttpPost("Login")]
        //public Userr Login([FromBody] Userr userToLogin)
        //{
        //    return userToLogin.Login(userToLogin.Email, userToLogin.Password);
        //}


        //[HttpPut("UpdateUser")]
        //public Userr UpdateUser([FromBody] Userr changesForUser)
        //{
        //    return Userr.UpdateUser(changesForUser);
        //}
        //[HttpPut("isActive")]
        //public int changeActivation(int id, bool userActivation)
        //{
        //    return Userr.changeActivation(id, userActivation);
        //}
        //[HttpGet("UsersBI")]
        //public object UsersBI()
        //{
        //    return Userr.UsersBI();
        //}


    }
}
