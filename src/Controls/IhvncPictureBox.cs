

using System.Drawing;


namespace InvokedServer.Controls
{
	public interface IhvncPictureBox
	{
		bool Running { get; set; }

		Image GetImageSafe { get; set; }

		void Start();

		void Stop();

		void UpdateImage(Bitmap bmp, bool cloneBitmap = false);
	}
}
