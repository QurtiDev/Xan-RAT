

using System.Drawing;
using System.Windows.Forms;


namespace InvokedServer.Controls
{
	public class Line : Control
	{
		public Line.Alignment LineAlignment { get; set; }

		public Line() => this.TabStop = false;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawLine(new Pen((Brush)new SolidBrush(Color.LightGray)), new Point(5, 5), this.LineAlignment == Line.Alignment.Horizontal ? new Point(500, 5) : new Point(5, 500));
		}

		public enum Alignment
		{
			Horizontal,
			Vertical,
		}
	}
}