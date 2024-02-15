using System.ComponentModel.DataAnnotations;

namespace MusicApi.Dtos.Auth
{
    public class AddClaimModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string ClaimType { get; set; }
        [Required]
        public string ClaimValue { get; set; }
    }
}
