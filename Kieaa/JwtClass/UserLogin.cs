namespace Kieaa.JwtClass
{
    public class UserLogin
    {
        public string email { get; set; }//or phone number
        public string password { get; set; }
        public string? Message { get; set; }
        public bool? IsAuthenticated { get; set; }
    }
}
