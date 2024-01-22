using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel registerModel)
        {
            var result = await _authService.RegisterUserAsync(registerModel);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            //Todo send confirmation mail
            return Ok(result);
        }
    }
}
