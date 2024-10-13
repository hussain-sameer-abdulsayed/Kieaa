using System.ComponentModel.DataAnnotations;

namespace Kieaa.Dtos
{
    public class ForgotPasswordConfirmation
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
