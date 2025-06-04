namespace IdentityService.Pages.Account.Register
{
    public class ViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public required string ReturnUrl { get; set; }
        public string Button { get; set; } = string.Empty;
    }
}
