using System;
using System.IO;
//using ImageMagick;
using Xfinium.Pdf;
using Xfinium.Pdf.Rendering;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime;

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
            MemoryStream ms = new MemoryStream(_documentScanat.FILE_CONTENT);
            Image image = Image.FromStream(ms);
            string tmp_file = "tmp_" + DateTime.Now.ToString();
            response r = SaveThumbNail(ThumbNailType.Custom.ToString(), Path.Combine(AppContext.BaseDirectory, "scans"), tmp_file, image, s.Width, s.Height);
            FileStream fs = new FileStream(Path.Combine(AppContext.BaseDirectory, "scans", r.Message), FileMode.Open, FileAccess.Read);
            byte[] b = new byte[fs.Length];
            fs.Read(b, 0, (int)fs.Length);
            fs.Dispose();
            try
            {
                File.Delete(Path.Combine(AppContext.BaseDirectory, "scans", tmp_file));
            }
            catch { }
            return b;
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

        public static response GenerateByteThumbNail(string fileName, ThumbNailSizes tSize)
        {
            return GenerateByteThumbNail(File.Exists(fileName) ? "" : CommonFunctions.GetScansFolder(), fileName, tSize);
        }

        public static response GenerateByteThumbNail(string path, string fileName, ThumbNailSizes tSize)
        {
            return GenerateByteThumbNail(path, fileName, tSize.Width, tSize.Height);
        }

        public static response GenerateByteThumbNail(string path, string fileName, int width, int height)
        {
            FileInfo fi = new FileInfo(Path.Combine(path, fileName));
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

                        FileStream pngStream = File.OpenWrite(Path.Combine(path, fileName.Replace(".pdf", ".png")));
                        renderer.ConvertPageToImage(pngStream, PdfPageImageFormat.Png, s);
                        pngStream.Flush();
                        pngStream.Dispose();
                        byte[] toReturn = GetByteThumbNail(path, fileName.Replace(".pdf", ".png"), null, width, height);
                        response r = new response(true, "", toReturn, null, null);
                        try
                        {
                            File.Delete(Path.Combine(path, fileName.Replace(".pdf", ".png")));
                        }
                        catch { }
                        return r;
                    }
                    catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                    try
                    {
                        byte[] toReturn = GetByteThumbNail(path, fileName, null, width, height);
                        return new response(true, "", toReturn, null, null);
                    }
                    catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
                default:
                    Error err = ErrorParser.ErrorMessage("unsupportedFormat");
                    return new response(false, err.ERROR_MESSAGE, null, null, new System.Collections.Generic.List<Error>() { err });
            }
        }

        private static byte[] GetByteThumbNail(string path, string originalFilename, Image img, int canvasWidth, int canvasHeight)
        {
            try
            {
                Image image;
                if (img == null && originalFilename != null)
                {
                    //image = Image.FromFile(Path.Combine(path, originalFilename)); // -- locks the file and can not delete temp png!
                    using (var bmpTemp = new Bitmap(Path.Combine(path, originalFilename)))
                    {
                        image = new Bitmap(bmpTemp);
                    }
                }
                else
                {
                    image = img;
                }

                int originalWidth = image.Width;
                int originalHeight = image.Height;

                Image thumbnail = new Bitmap(canvasWidth, canvasHeight);
                Graphics graphic = Graphics.FromImage(thumbnail);

                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;

                double ratioX = (double)canvasWidth / (double)originalWidth;
                double ratioY = (double)canvasHeight / (double)originalHeight;
                double ratio = ratioX < ratioY ? ratioX : ratioY;

                int newHeight = Convert.ToInt32(originalHeight * ratio);
                int newWidth = Convert.ToInt32(originalWidth * ratio);

                int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
                int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

                graphic.Clear(Color.White); // white padding
                graphic.DrawImage(image, posX, posY, newWidth, newHeight);

                ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
                EncoderParameters encoderParameters;
                encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                using (var ms = new MemoryStream())
                {
                    thumbnail.Save(ms, ImageFormat.Gif);
                    graphic.Dispose();
                    thumbnail.Dispose();
                    image.Dispose();
                    return ms.ToArray();
                }
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }


        public static response GenerateImgThumbNail(string sType, string path, string fileName, int width, int height)
        {
            FileInfo fi = new FileInfo(Path.Combine(path, fileName));
            //string outputFile = fileName.Replace(fi.Extension, sType == "s" ? "_s.gif" : "_m.gif");
            string outputFile = fileName.Replace(fi.Extension, "_" + sType + ".jpg");

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

                        FileStream pngStream = File.OpenWrite(Path.Combine(path, fileName.Replace(".pdf", ".png")));
                        renderer.ConvertPageToImage(pngStream, PdfPageImageFormat.Png, s);
                        pngStream.Flush();
                        pngStream.Dispose();

                        response r = SaveThumbNail(sType, path, fileName.Replace(".pdf", ".png"), null, width, height);
                        try
                        {
                            File.Delete(Path.Combine(path, fileName.Replace(".pdf", ".png")));
                        }
                        catch { }
                        return r;
                    }
                    catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                    try
                    {
                        return SaveThumbNail(sType, path, fileName, null, width, height);
                    }
                    catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
                default:
                    Error err = ErrorParser.ErrorMessage("unsupportedFormat");
                    return new response(false, err.ERROR_MESSAGE, null, null, new System.Collections.Generic.List<Error>() { err });
            }
        }

        private static response SaveThumbNail(string sType, string path, string originalFilename, Image img, int canvasWidth, int canvasHeight)
        {
            try
            {
                FileInfo fi = new FileInfo(Path.Combine(path, originalFilename));
                //string outputFile = fileName.Replace(fi.Extension, sType == "s" ? "_s.gif" : "_m.gif");
                string outputFile = originalFilename.Replace(fi.Extension, "_" + sType + ".jpg");

                Image image;
                if (img == null && originalFilename != null)
                {
                    //image = Image.FromFile(Path.Combine(path, originalFilename)); // -- locks the file and can not delete temp png!
                    using (var bmpTemp = new Bitmap(Path.Combine(path, originalFilename)))
                    {
                        image = new Bitmap(bmpTemp);
                    }
                }
                else
                {
                    image = img;
                }

                int originalWidth = image.Width;
                int originalHeight = image.Height;

                Image thumbnail = new Bitmap(canvasWidth, canvasHeight);
                Graphics graphic = Graphics.FromImage(thumbnail);

                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;

                double ratioX = (double)canvasWidth / (double)originalWidth;
                double ratioY = (double)canvasHeight / (double)originalHeight;
                double ratio = ratioX < ratioY ? ratioX : ratioY;

                int newHeight = Convert.ToInt32(originalHeight * ratio);
                int newWidth = Convert.ToInt32(originalWidth * ratio);

                int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
                int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

                graphic.Clear(Color.White); // white padding
                graphic.DrawImage(image, posX, posY, newWidth, newHeight);

                ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
                EncoderParameters encoderParameters;
                encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                thumbnail.Save(Path.Combine(path, outputFile), info[1], encoderParameters);

                graphic.Dispose();
                thumbnail.Dispose();
                image.Dispose();
                return new response(true, outputFile, outputFile, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public static ThumbNailSize ScaleImage(Image image, double width, double height)
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

        public static ThumbNailSize ScaleImage(Xfinium.Pdf.Graphics.PdfImage image, double width, double height)
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

        public static void DeleteThumbNail(Models.DocumentScanat d)
        {
            try {
                if(File.Exists(Path.Combine(AppContext.BaseDirectory, "scans", d.DENUMIRE_FISIER.Replace(d.EXTENSIE_FISIER, "_Custom.jpg"))))
                    File.Delete(Path.Combine(AppContext.BaseDirectory, "scans", d.DENUMIRE_FISIER.Replace(d.EXTENSIE_FISIER, "_Custom.jpg")));
            } catch { }
            try
            {
                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "scans", d.DENUMIRE_FISIER.Replace(d.EXTENSIE_FISIER, "_Medium.jpg"))))
                    File.Delete(Path.Combine(AppContext.BaseDirectory, "scans", d.DENUMIRE_FISIER.Replace(d.EXTENSIE_FISIER, "_Medium.jpg")));
            }
            catch { }
            try
            {
                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "scans", d.DENUMIRE_FISIER.Replace(d.EXTENSIE_FISIER, "_Small.jpg"))))
                    File.Delete(Path.Combine(AppContext.BaseDirectory, "scans", d.DENUMIRE_FISIER.Replace(d.EXTENSIE_FISIER, "_Small.jpg")));
            }
            catch { }
        }

        #region -- Magick.NET --
        /*
        public static byte[] GenerateImgThumbNail(Models.DocumentScanat _documentScanat, ThumbNailSizes s)
        {
            
            MagickReadSettings settings = new MagickReadSettings();
            //settings.Density = new Density(600, 600);
            settings.BorderColor = MagickColors.Red;
            settings.BackgroundColor = MagickColors.White;
            
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
              
        public static response GenerateImgThumbNail_WithMagickNet(string sType, string path, string fileName, int width, int height)
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
        */
        #endregion
    }
}