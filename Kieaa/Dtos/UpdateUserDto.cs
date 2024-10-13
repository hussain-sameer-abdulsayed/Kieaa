using System.ComponentModel.DataAnnotations;

namespace Kieaa.Dtos
{
    public class UpdateUserDto
    {
        [MaxLength(50)]
        public string? UserName { get; set; } = string.Empty;
        [MaxLength(50)]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
    }
}
