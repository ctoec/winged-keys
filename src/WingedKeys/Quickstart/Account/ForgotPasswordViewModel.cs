using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}