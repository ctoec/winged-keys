namespace IdentityServer4.Quickstart.UI
{
    public class ForgotPasswordViewModel : ForgotPasswordInputModel
    {
        public bool EmailSent { get; set; }
        public string Error { get; set; }
    }
}