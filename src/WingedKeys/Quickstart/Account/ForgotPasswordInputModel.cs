using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI {

    public class ForgotPasswordInputModel
    {
        [Required]
        public string Email { get; set; }
    }
}