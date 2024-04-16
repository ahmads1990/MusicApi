using MusicApi.Dtos.Auth;

namespace MusicApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterUserAsync(RegisterModel registerModel);
        Task<AuthModel> LoginUserAsync(LoginModel loginModel);
        Task<string> AddClaimAsync(AddClaimModel claimModel);
    }
}
