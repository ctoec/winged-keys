using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class NewAccountInputModel
    {
        [Required]
        public string GivenName { get; set; }
        [Required]
        public string FamilyName { get; set; }
        [Required]
        public string Username{ get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}