using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {      
            // first check if user has account
            var isRegistered = await _userManager.FindByEmailAsync(registerModel.Email);
            if (isRegistered != null)
            {
                //Todo send them to login endpoint 
                return BadRequest();
            }

            // create new user object
            var user = new ApplicationUser();
            registerModel.Adapt(user);
      
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }
            //Todo send confirmation mail

            return Ok("Done");
        }
    }
}
