

using System.Threading;
using System.Windows.Forms;


namespace InvokedClient
{
	public class MultiFormContext : ApplicationContext
	{
		private int openForms;

		public MultiFormContext(params Form[] forms)
		{
			this.openForms = forms.Length;
			foreach (Form form in forms)
			{
				form.FormClosed += (FormClosedEventHandler)((s, args) =>
				{
					if (Interlocked.Decrement(ref this.openForms) != 0)
						return;
					this.ExitThread();
				});
				form.Show();
			}
		}
	}
}