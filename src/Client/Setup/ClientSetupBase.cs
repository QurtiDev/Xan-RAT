

using InvokedClient.User;


namespace InvokedClient.Setup
{
	public abstract class ClientSetupBase
	{
		protected UserAccount UserAccount;

		protected ClientSetupBase() => this.UserAccount = new UserAccount();
	}
}