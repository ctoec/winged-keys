namespace IdentityServer4.Quickstart.UI
{
	public class InviteUserViewModel : InviteUserInputModel
	{
		public bool? Success { get; set; } = null;
		public string EmailRecipient { get; set; } = null;
		public string Error { get; set; } = null;
	}
}