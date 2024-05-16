using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace WPF.ParticleLife.Updated
{
    internal static class ExtensionMethods
    {
        #region System.Drawing.Bitmap

        public static BitmapSource ToBitmapSource(this Bitmap bitmap) 
        {
            if (bitmap == null) return null;

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(bitmap.Width, bitmap.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution,
                    System.Windows.Media.PixelFormats.Bgra32, null, bitmapData.Scan0, size, bitmapData.Stride);
            }
            catch
            {
                return null;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        #endregion

        #region System.Windows.Media.Color

        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion

        #region System.Drawing.Color

        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion
    }
}
