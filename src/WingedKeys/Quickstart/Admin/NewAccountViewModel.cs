namespace IdentityServer4.Quickstart.UI
{
	public class NewAccountViewModel : NewAccountInputModel
	{
		public bool? Success { get; set; } = null;
		public string Error { get; set; } = null;
	}
}