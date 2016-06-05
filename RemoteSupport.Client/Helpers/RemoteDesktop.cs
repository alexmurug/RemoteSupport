using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace RemoteSupport.Client.Helpers
{
    public static class RemoteDesktop
    {
        public static MemoryStream CaptureScreenToMemoryStream(int quality)
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), bmp.Size);
            g.Dispose();


            var codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;

            foreach (var codec in codecs)
            {
                if (codec.MimeType == "image/jpeg")
                    ici = codec;
            }

            var ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            var ms = new MemoryStream();
            bmp.Save(ms, ici, ep);
            ms.Position = 0;
            bmp.Dispose();

            return ms;
        }
    }
}