using System;
using System.IO;
using ImageMagick;
using Xfinium.Pdf;

namespace SOCISA
{
    public enum ThumbNailType { Small = 0, Medium }

    public struct ThumbNailSizes
    {
        public ThumbNailType thumbNailType;
        public int Width, Height;
        public ThumbNailSizes(ThumbNailType t, int w, int h)
        {
            thumbNailType = t;
            Width = w;
            Height = h;
        }
    }
    public static class ThumbNails
    {
        public static byte[] GenerateImgThumbNail(Models.DocumentScanat _documentScanat, ThumbNailSizes s)
        {
            /*
            MagickReadSettings settings = new MagickReadSettings();
            //settings.Density = new Density(600, 600);
            settings.BorderColor = MagickColors.Red;
            settings.BackgroundColor = MagickColors.White;
            */
            using (MagickImageCollection images = new MagickImageCollection())
            {
                //images.Read(_documentScanat.DENUMIRE_FISIER, settings);
                images.Read(_documentScanat.FILE_CONTENT);
                MagickImage image = images[0];
                image.Resize(s.Width, s.Height);
                image.Format = MagickFormat.Gif;
                image.BackgroundColor = MagickColors.White;
                image.BorderColor = MagickColors.Red;
                return image.ToByteArray();
            }
        }

        public static byte[] GenerateImgThumbNail(int _authenticatedUserId, string _connectionString, int _idDocumentScanat, ThumbNailSizes s)
        {
            Models.DocumentScanat d = new Models.DocumenteScanateRepository(_authenticatedUserId, _connectionString).Find(_idDocumentScanat);
            return GenerateImgThumbNail(d, s);
        }
    }
}