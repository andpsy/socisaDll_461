using System;
using System.IO;
using ImageMagick;
using Xfinium.Pdf;

namespace SOCISA
{
    public enum ThumbNailType { Small = 0, Medium, Custom }

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
                //image.Format = MagickFormat.Gif;
                image.Format = MagickFormat.Jpg;
                //image.BackgroundColor = MagickColors.White;
                //image.BorderColor = MagickColors.Red;
                return image.ToByteArray();
            }
        }

        public static byte[] GenerateImgThumbNail(int _authenticatedUserId, string _connectionString, int _idDocumentScanat, ThumbNailSizes s)
        {
            Models.DocumentScanat d = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.DocumentScanat>(new Models.DocumenteScanateRepository(_authenticatedUserId, _connectionString).Find(_idDocumentScanat).Message);
            return GenerateImgThumbNail(d, s);
        }

        public static string GenerateImgThumbNail(ThumbNailSizes tSize, string filePath, string fileName)
        {
            return GenerateImgThumbNail(tSize.thumbNailType.ToString(), filePath, fileName, tSize.Width, tSize.Height);
        }

        public static string GenerateImgThumbNail(string sType, string path, string fileName, int width, int height)
        {
            FileInfo fi = new FileInfo(Path.Combine(path, fileName));
            //string outputFile = fileName.Replace(fi.Extension, sType == "s" ? "_s.gif" : "_m.gif");
            string outputFile = fileName.Replace(fi.Extension, "_" + sType + ".jpg");
            //MagickNET.Initialize(path.Replace("\\FILE_STORAGE", ""));
            MagickReadSettings settings = new MagickReadSettings();
            // Settings the density to 300 dpi will create an image with a better quality
            //settings.Density = new Density(600, 600);
            //settings.BorderColor = MagickColors.Red;
            //settings.BackgroundColor = MagickColors.White;
            //settings.FillColor = MagickColors.White;

            try
            {
                using (MagickImageCollection images = new MagickImageCollection())
                {
                    images.Read(Path.Combine(path, fileName), settings);
                    MagickImage image = images[0];
                    image.Resize(width, height);
                    //image.Format = MagickFormat.Gif;
                    image.Format = MagickFormat.Jpg;
                    //image.BackgroundColor = MagickColors.White;
                    //image.BorderColor = MagickColors.Red;

                    image.Write(Path.Combine(path, outputFile));
                }
                return outputFile;
            }
            catch { return null; }
        }
    }
}