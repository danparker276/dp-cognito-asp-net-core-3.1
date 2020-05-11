using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dp.business.Helpers
{
    /// <summary>
    /// Validates the binaries for the image content and outputs the result.  If the value is "unknown" then consider the
    /// bytes to not
    /// be a valid image.  Courtesy http://stackoverflow.com/a/9446045/16454
    /// </summary>
    public static class ImageBinaryValidator
    {
        private static readonly Dictionary<ImageFormat, Func<byte[], bool>> Definitions;

        public enum ImageFormat
        {
            Unknown,
            Bmp,
            Jpeg,
            Gif,
            Tiff,
            Png
        }
        static ImageBinaryValidator()
        {
            // see http://www.mikekunz.com/image_file_header.html  
            var bmp = Encoding.ASCII.GetBytes("BM"); // BMP
            var gif = Encoding.ASCII.GetBytes("GIF"); // GIF
            var png = new byte[] { 137, 80, 78, 71 }; // PNG
            var tiff = new byte[] { 73, 73, 42 }; // TIFF
            var tiff2 = new byte[] { 77, 77, 42 }; // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

            Func<byte[], byte[], bool> hasMatch =
                (format, bytesToTest) => format.SequenceEqual(bytesToTest.Take(format.Length));

            Definitions = new Dictionary<ImageFormat, Func<byte[], bool>>
            {
                { ImageFormat.Bmp, b => hasMatch(bmp, b) },
                { ImageFormat.Gif, b => hasMatch(gif, b) },
                { ImageFormat.Png, b => hasMatch(png, b) },
                { ImageFormat.Tiff, b => hasMatch(tiff, b) || hasMatch(tiff2, b) },
                { ImageFormat.Jpeg, b => hasMatch(jpeg, b) || hasMatch(jpeg2, b) }
            };
        }

        public static ImageFormat GetImageFormat(byte[] bytes)
        {
            var q = from formatCheck in Definitions
                    where formatCheck.Value(bytes)
                    select formatCheck.Key;
            return q.FirstOrDefault();
        }
    }
}
