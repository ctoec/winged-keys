namespace IdentityServer4.Quickstart.UI
{
	public class TriggerInitialLoginViewModel : TriggerInitialLoginInputModel
	{
		public bool? Success { get; set; } = null;
		public string EmailSent { get; set; } = null;
		public string Error { get; set; } = null;
	}
}