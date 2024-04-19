using System.ComponentModel.DataAnnotations;

namespace MusicApi.Security
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
    }
}
