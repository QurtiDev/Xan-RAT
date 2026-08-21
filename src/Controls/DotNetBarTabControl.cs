

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace InvokedServer.Controls
{
	internal class DotNetBarTabControl : TabControl
	{
		public DotNetBarTabControl()
		{
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.SizeMode = TabSizeMode.Fixed;
			this.ItemSize = new Size(44, 136);
			this.Alignment = TabAlignment.Left;
			this.SelectedIndex = 0;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Bitmap bitmap = new Bitmap(this.Width, this.Height);
			Graphics graphics1 = Graphics.FromImage((Image)bitmap);
			if (!this.DesignMode)
				this.SelectedTab.BackColor = SystemColors.Control;
			graphics1.Clear(SystemColors.Control);
			graphics1.FillRectangle((Brush)new SolidBrush(Color.FromArgb(246, 248, 252)), new Rectangle(0, 0, this.ItemSize.Height + 4, this.Height));
			graphics1.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(this.ItemSize.Height + 3, 0), new Point(this.ItemSize.Height + 3, 999));
			Graphics graphics2 = graphics1;
			Pen pen1 = new Pen(Color.FromArgb(170, 187, 204));
			Point pt1_1 = new Point(0, this.Size.Height - 1);
			int x1 = this.Width + 3;
			Size size1 = this.Size;
			int y1 = size1.Height - 1;
			Point pt2_1 = new Point(x1, y1);
			graphics2.DrawLine(pen1, pt1_1, pt2_1);
			Rectangle tabRect;
			Point location1;
			for (int index = 0; index <= this.TabCount - 1; ++index)
			{
				if (index == this.SelectedIndex)
				{
					Rectangle rectangle = default;
					ref Rectangle local = ref rectangle;
					tabRect = this.GetTabRect(index);
					location1 = tabRect.Location;
					int x2 = location1.X - 2;
					tabRect = this.GetTabRect(index);
					location1 = tabRect.Location;
					int y2 = location1.Y - 2;
					Point location2 = new Point(x2, y2);
					tabRect = this.GetTabRect(index);
					int width = tabRect.Width + 3;
					tabRect = this.GetTabRect(index);
					int height = tabRect.Height - 1;
					Size size2 = new Size(width, height);
					local = new Rectangle(location2, size2);
					graphics1.FillRectangle((Brush)new LinearGradientBrush(rectangle, Color.Black, Color.Black, 90f)
					{
						InterpolationColors = new ColorBlend()
						{
							Colors = new Color[3]
						{
				Color.FromArgb(232, 232, 240),
				Color.FromArgb(232, 232, 240),
				Color.FromArgb(232, 232, 240)
						},
							Positions = new float[3] { 0.0f, 0.5f, 1f }
						}
					}, rectangle);
					graphics1.DrawRectangle(new Pen(Color.FromArgb(170, 187, 204)), rectangle);
					graphics1.SmoothingMode = SmoothingMode.HighQuality;
					Point[] pointArray = new Point[3];
					size1 = this.ItemSize;
					int x3 = size1.Height - 3;
					tabRect = this.GetTabRect(index);
					location1 = tabRect.Location;
					int y3 = location1.Y + 20;
					pointArray[0] = new Point(x3, y3);
					size1 = this.ItemSize;
					int x4 = size1.Height + 4;
					tabRect = this.GetTabRect(index);
					location1 = tabRect.Location;
					int y4 = location1.Y + 14;
					pointArray[1] = new Point(x4, y4);
					size1 = this.ItemSize;
					int x5 = size1.Height + 4;
					tabRect = this.GetTabRect(index);
					location1 = tabRect.Location;
					int y5 = location1.Y + 27;
					pointArray[2] = new Point(x5, y5);
					Point[] points = pointArray;
					graphics1.FillPolygon(SystemBrushes.Control, points);
					graphics1.DrawPolygon(new Pen(Color.FromArgb(170, 187, 204)), points);
					if (this.ImageList != null)
					{
						try
						{
							Graphics graphics3 = graphics1;
							Image image = this.ImageList.Images[this.TabPages[index].ImageIndex];
							location1 = rectangle.Location;
							int x6 = location1.X + 8;
							location1 = rectangle.Location;
							int y6 = location1.Y + 6;
							Point point = new Point(x6, y6);
							graphics3.DrawImage(image, point);
							graphics1.DrawString("  " + this.TabPages[index].Text, this.Font, Brushes.Black, (RectangleF)rectangle, new StringFormat()
							{
								LineAlignment = StringAlignment.Center,
								Alignment = StringAlignment.Center
							});
						}
						catch 
						{
							graphics1.DrawString(this.TabPages[index].Text, new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold), Brushes.Black, (RectangleF)rectangle, new StringFormat()
							{
								LineAlignment = StringAlignment.Center,
								Alignment = StringAlignment.Center
							});
						}
					}
					else
						graphics1.DrawString(this.TabPages[index].Text, new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold), Brushes.Black, (RectangleF)rectangle, new StringFormat()
						{
							LineAlignment = StringAlignment.Center,
							Alignment = StringAlignment.Center
						});
					Graphics graphics4 = graphics1;
					Pen pen2 = new Pen(Color.FromArgb(200, 200, 250));
					location1 = rectangle.Location;
					int x7 = location1.X - 1;
					location1 = rectangle.Location;
					int y7 = location1.Y - 1;
					Point pt1_2 = new Point(x7, y7);
					location1 = rectangle.Location;
					int x8 = location1.X;
					location1 = rectangle.Location;
					int y8 = location1.Y;
					Point pt2_2 = new Point(x8, y8);
					graphics4.DrawLine(pen2, pt1_2, pt2_2);
					Graphics graphics5 = graphics1;
					Pen pen3 = new Pen(Color.FromArgb(200, 200, 250));
					location1 = rectangle.Location;
					Point pt1_3 = new Point(location1.X - 1, rectangle.Bottom - 1);
					location1 = rectangle.Location;
					Point pt2_3 = new Point(location1.X, rectangle.Bottom);
					graphics5.DrawLine(pen3, pt1_3, pt2_3);
				}
				else
				{
					Rectangle rectangle = default;
					ref Rectangle local = ref rectangle;
					tabRect = this.GetTabRect(index);
					location1 = tabRect.Location;
					int x9 = location1.X - 2;
					tabRect = this.GetTabRect(index);
					location1 = tabRect.Location;
					int y9 = location1.Y - 2;
					Point location3 = new Point(x9, y9);
					tabRect = this.GetTabRect(index);
					int width = tabRect.Width + 3;
					tabRect = this.GetTabRect(index);
					int height = tabRect.Height - 1;
					Size size3 = new Size(width, height);
					local = new Rectangle(location3, size3);
					graphics1.FillRectangle((Brush)new SolidBrush(Color.FromArgb(246, 248, 252)), rectangle);
					graphics1.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(rectangle.Right, rectangle.Top), new Point(rectangle.Right, rectangle.Bottom));
					if (this.ImageList != null)
					{
						try
						{
							Graphics graphics6 = graphics1;
							Image image = this.ImageList.Images[this.TabPages[index].ImageIndex];
							location1 = rectangle.Location;
							int x10 = location1.X + 8;
							location1 = rectangle.Location;
							int y10 = location1.Y + 6;
							Point point = new Point(x10, y10);
							graphics6.DrawImage(image, point);
							graphics1.DrawString("  " + this.TabPages[index].Text, this.Font, Brushes.DimGray, (RectangleF)rectangle, new StringFormat()
							{
								LineAlignment = StringAlignment.Center,
								Alignment = StringAlignment.Center
							});
						}
						catch
						{
							graphics1.DrawString(this.TabPages[index].Text, this.Font, Brushes.DimGray, (RectangleF)rectangle, new StringFormat()
							{
								LineAlignment = StringAlignment.Center,
								Alignment = StringAlignment.Center
							});
						}
					}
					else
						graphics1.DrawString(this.TabPages[index].Text, this.Font, Brushes.DimGray, (RectangleF)rectangle, new StringFormat()
						{
							LineAlignment = StringAlignment.Center,
							Alignment = StringAlignment.Center
						});
				}
			}
			e.Graphics.DrawImage((Image)bitmap, new Point(0, 0));
			graphics1.Dispose();
			bitmap.Dispose();
		}
	}
}