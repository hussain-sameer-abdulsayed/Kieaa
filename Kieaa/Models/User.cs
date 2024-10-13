using Microsoft.AspNetCore.Identity;

namespace Kieaa.Models
{
    public class User : IdentityUser
    {
        public string? RefreeshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string ValidationEmailToken { get; set; }

        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
    }
}
