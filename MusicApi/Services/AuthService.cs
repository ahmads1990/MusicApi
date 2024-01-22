
namespace MusicApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<AuthModel> RegisterUserAsync(RegisterModel registerModel)
        {
            // first check if user email is already exists in database
            if (await _userManager.FindByEmailAsync(registerModel.Email) is not null)
                return new AuthModel { Message = "Email already exists" };
            // check if username is already exists in database
            if (await _userManager.FindByNameAsync(registerModel.UserName) is not null)
                return new AuthModel { Message = "Username already exists" };

            // create new user object
            var user = new ApplicationUser();
            registerModel.Adapt(user);

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                string errorMessage = string.Empty;
                foreach (var error in result.Errors)
                {
                    errorMessage += $"{error.Description} | ";
                }
                return new AuthModel { Message = errorMessage };
            }

            // user creation went ok  
            return new AuthModel
            {
                Email = user.Email,
                IsAuthenticated = true,
                Username = user.UserName
            };
        }
    }
}
