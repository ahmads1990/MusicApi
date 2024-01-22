namespace MusicApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterUserAsync(RegisterModel registerModel);
    }
}
