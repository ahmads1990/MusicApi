
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MusicApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
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

            // user creation went ok then create token and send it back
            var jwtToken = await CreateJwtToken(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Username = user.UserName,
                Email = user.Email,
                Claims = new List<string>(),
                Token=new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn=jwtToken.ValidTo                         
            };
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            if (user is null) return null;
            // get user claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // create jwt claims
            var jwtClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            };
            // merge both claims lists to jwtClaims
            jwtClaims = (Claim[])jwtClaims.Union(userClaims);

            // specify the signing key and algorithm
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            // finally create the token
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: jwtClaims,
                expires: DateTime.Now.AddHours(_jwtConfig.DurationInHours),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }
    }
}
