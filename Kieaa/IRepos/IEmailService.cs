using Kieaa.Dtos;

namespace Kieaa.IRepos
{
    public interface IEmailService
    {
        Task SendValidationEmailAsync(EmailDto request);
    }
}
