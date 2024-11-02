using System.ComponentModel.DataAnnotations;

namespace MusicApi.Dtos.Auth
{
    public class AddClaimModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string ClaimType { get; set; } = string.Empty;
        [Required]
        public string ClaimValue { get; set; } = string.Empty;
    }
}
