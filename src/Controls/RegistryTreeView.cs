

using System.Windows.Forms;


namespace InvokedServer.Controls
{
	public class RegistryTreeView : TreeView
	{
		public RegistryTreeView()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
		}
	}
}