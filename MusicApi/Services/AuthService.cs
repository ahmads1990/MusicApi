
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicApi.Dtos.Auth;
using MusicApi.Helpers;
using MusicApi.Helpers.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

#pragma warning disable CS8604, CS8601, CS8602 // Possible null reference argument.

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
            var jwtToken = await CreateJwtTokenAsync(user);

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
        public async Task<AuthModel> LoginUserAsync(LoginModel loginModel)
        {
            AuthModel authModel = new AuthModel();
            // return if email doesn't exist OR email+password don't match
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if(user is null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtToken = await CreateJwtTokenAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Username = user.UserName;
            authModel.Email = user.Email;
            authModel.Claims = claims.Select(c=>c.Type).ToList();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.ExpiresOn=jwtToken.ValidTo;

            return authModel;
        }
        public async Task<string> AddClaimAsync(AddClaimModel claimModel)
        {
            var user = await _userManager.FindByIdAsync(claimModel.UserId);

            //check first if user with that id exists
            if (user is null)
                return "Invalid user ID";

            // claim type exists in allowed types
            if (!CustomClaimTypes.ALLOWEDTYPES.Contains(claimModel.ClaimType))
                return "Invalid claim type not allowed";
           
            // check user claims to see if user has this claim already
            var claims = await _userManager.GetClaimsAsync(user);
            if(claims.FirstOrDefault(c=>c.Type.Equals(claimModel.ClaimType)) != null)
                return "User already assigned to this claim";

            // try to add the claim to user
            var result = await _userManager.AddClaimAsync(user, new Claim(claimModel.ClaimType, claimModel.ClaimValue));
            
            return result.Succeeded ? string.Empty : "Something went wrong";
        }
        private async Task<JwtSecurityToken?> CreateJwtTokenAsync(ApplicationUser user)
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
#pragma warning restore CS8604 // Possible null reference argument.
                              // merge both claims lists and jwtClaims to allClaims
            var allClaims =  jwtClaims.Union(userClaims);

            // specify the signing key and algorithm
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            // finally create the token
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: allClaims,
                expires: DateTime.Now.AddHours(_jwtConfig.DurationInHours),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }
    }
}
