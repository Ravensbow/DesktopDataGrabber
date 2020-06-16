using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DesktopDataGrabber.Tools
{
    static class ImageCreator
    {
        public static Bitmap Create(int[] sensLEDs, int w, int h)
        {
            // Create 2D array of integers
            int width = w;
            int height = h;
            int stride = width * 4;
            int[,] integers = new int[width, height];

            Random random = new Random();
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    var color = System.Windows.Media.Color.
                        FromRgb((byte)((sensLEDs[x * 8 + y] >> 16) & 0xFF), (byte)((sensLEDs[x * 8 + y] >> 8) & 0xFF), (byte)((sensLEDs[x * 8 + y] >> 0) & 0xFF));
                    byte[] bgra = new byte[] { (byte)color.R, (byte)color.G, (byte)color.B, 255 };
                    integers[x, y] = BitConverter.ToInt32(bgra, 0);
                }
            }

            Bitmap bitmap;
            unsafe
            {
                fixed (int* intPtr = &integers[0, 0])
                {
                    bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppRgb, new IntPtr(intPtr));
                }
            }
            return bitmap;
        }
        public static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
