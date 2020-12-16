using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class InviteUserInputModel
    {
        [Required]
        public string Username{ get; set; }

        [Required]
        public string Email { get; set; }
    }
}