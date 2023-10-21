using AwsLocalStack.Contracts;
using AwsLocalStack.Models;
using Microsoft.AspNetCore.Mvc;

namespace AwsLocalStack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        IUserService _userService;
        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await _userService.GetUsers();
            if(users == null) 
                return NotFound("No record found");
            return Ok(users);
        }

        [HttpGet("GetUserById")]
        public IActionResult GetById(string id)
        {
            var user =  _userService.GetUserById(id);
            if (user == null)
                return NotFound("No record found");
            return Ok(user);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateAsync(User user)
        {
            await _userService.CreateAsync(user);

            return Ok(user);
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var resp = await _userService.DeleteAsync(id);

            return Ok(resp);
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateAsync(User user)
        {
            var resp = await _userService.UpdateAsyc(user);

            return Ok(resp);
        }
    }
}
