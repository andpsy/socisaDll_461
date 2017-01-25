using System;
using System.IO;
using ImageMagick;
using Xfinium.Pdf;
using Xfinium.Pdf.Rendering;
using Xfinium.Pdf.Imaging;
using Xfinium.Pdf.Graphics;
using System.Drawing;

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
            Models.DocumentScanat d = (Models.DocumentScanat)(new Models.DocumenteScanateRepository(_authenticatedUserId, _connectionString).Find(_idDocumentScanat).Result);
            return GenerateImgThumbNail(d, s);
        }

        public static response GenerateImgThumbNail(ThumbNailSizes tSize, string filePath, string fileName)
        {
            return GenerateImgThumbNail(tSize.thumbNailType.ToString(), filePath, fileName, tSize.Width, tSize.Height);
        }

        public static response GenerateImgThumbNail(string filePath, string fileName)
        {
            return GenerateImgThumbNail(CommonFunctions.GetThumbNailSizes(ThumbNailType.Custom), filePath, fileName);
        }

        public static response GenerateImgThumbNail(string fileName)
        {
            return GenerateImgThumbNail(CommonFunctions.GetThumbNailSizes(ThumbNailType.Custom), File.Exists(fileName) ? "" : CommonFunctions.GetScansFolder(), fileName);
        }

        public static response GenerateImgThumbNail(ThumbNailSizes tSize, string fileName)
        {
            return GenerateImgThumbNail(tSize.thumbNailType.ToString(), CommonFunctions.GetScansFolder(), fileName, tSize.Width, tSize.Height);
        }

        public static response GenerateImgThumbNail(string sType, string path, string fileName, int width, int height)
        {
            FileInfo fi = new FileInfo(Path.Combine(path, fileName));
            //string outputFile = fileName.Replace(fi.Extension, sType == "s" ? "_s.gif" : "_m.gif");
            string outputFile = fileName.Replace(fi.Extension, "_" + sType + ".jpg");
            MagickReadSettings settings = new MagickReadSettings();
            //settings.Density = new Density(600, 600);
            //settings.BorderColor = MagickColors.Red;
            //settings.BackgroundColor = MagickColors.White;
            //settings.FillColor = MagickColors.White;

            switch (fi.Extension)
            {
                case ".pdf":
                    try
                    {
                        FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read);
                        PdfFixedDocument pDoc = new PdfFixedDocument(fs);
                        fs.Dispose();
                        PdfPageRenderer renderer = new PdfPageRenderer(pDoc.Pages[0]);
                        PdfRendererSettings s = new PdfRendererSettings();
                        s.DpiX = s.DpiY = 96;
                        
                        FileStream pngStream = File.OpenWrite(Path.Combine(path, outputFile.Replace(".jpg", ".png")));
                        renderer.ConvertPageToImage(pngStream, PdfPageImageFormat.Png, s);
                        pngStream.Flush();
                        pngStream.Dispose();

                        MagickImageCollection images = new MagickImageCollection();
                        images.Read(Path.Combine(path, outputFile.Replace(".jpg", ".png")), settings);
                        MagickImage image = images[0];
                        ThumbNailSize ts = ScaleImage(image, width, height);
                        image.Resize(ts.Width, ts.Height);
                        image.Format = MagickFormat.Jpg;
                        //image.BackgroundColor = MagickColors.White;
                        //image.BorderColor = MagickColors.Red;
                        image.Write(Path.Combine(path, outputFile));
                        File.Delete(Path.Combine(path, outputFile.Replace(".jpg", ".png")));
                        return new response(true, outputFile, outputFile, null, null);
                    }
                    catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
                case ".jpg":
                case ".png":
                case ".bmp":
                    try
                    {
                        using (MagickImageCollection images = new MagickImageCollection())
                        {
                            images.Read(Path.Combine(path, fileName), settings);
                            MagickImage image = images[0];
                            ThumbNailSize ts = ScaleImage(image, width, height);
                            image.Resize(ts.Width, ts.Height);
                            image.Format = MagickFormat.Jpg;
                            //image.BackgroundColor = MagickColors.White;
                            //image.BorderColor = MagickColors.Red;
                            image.Write(Path.Combine(path, outputFile));
                            return new response(true, outputFile, outputFile, null, null);
                        }
                    }
                    catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
                default:
                    Error err = ErrorParser.ErrorMessage("unsupportedFormat");
                    return new response(false, err.ERROR_MESSAGE, null, null, new System.Collections.Generic.List<Error>() { err });
            }
        }

        public static ThumbNailSize ScaleImage(MagickImage image, double width, double height)
        {
            double ratioX = width / (double)image.Width;
            double ratioY = height / (double)image.Height;
            double sz = (double)Math.Max(image.Width, image.Height);
            double ratio = (double)Math.Min(width, height) / sz;
            ratio = ratio > 1 ? 1 : ratio;

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            return new ThumbNailSize(newWidth, newHeight);
        }
    }
}