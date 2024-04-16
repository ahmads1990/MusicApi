using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicApi.Dtos.Auth;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
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
                return BadRequest(result.Message);

            //Todo send confirmation mail
            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginModel loginModel)
        {
            var result = await _authService.LoginUserAsync(loginModel);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            //Todo send confirmation mail
            return Ok(result);
        }
        [HttpPost("AddClaim")]
        public async Task<IActionResult> AddRoleAsync(AddClaimModel claimModel)
        {
            var result = await _authService.AddClaimAsync(claimModel);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(claimModel);
        }
    }
}
