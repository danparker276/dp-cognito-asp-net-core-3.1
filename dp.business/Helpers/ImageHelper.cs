using System;
using System.Drawing;

namespace dp.business.Helpers
{
    public class ImageHelper
    {
        public static byte[] CreateImageThumbnail(byte[] image, bool keepRatio, int width = 50, int height = 50)
        {
            try
            {
                using (var stream = new System.IO.MemoryStream(image))
                {
                    var img = Image.FromStream(stream);
                    if (keepRatio == true)
                    {

                        if (img.Width > width)
                        {
                            height = (img.Height * width) / img.Width;
                        }
                        else
                        {
                            
                            width = img.Width;
                            height = img.Height;
                        }
                     
                    }
                    //var thumbnail = img.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                    var thumbnail = img.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                    using (var thumbStream = new System.IO.MemoryStream())
                    {
                        thumbnail.Save(thumbStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return thumbStream.GetBuffer();
                    }
                }
            }
            catch (ArgumentException)
            {
                throw new Exception("Image data is not valid");
            }
        }
    }
}
