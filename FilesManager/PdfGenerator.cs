using System;
using Xfinium.Pdf;
using System.IO;

namespace SOCISA
{
    public static class PdfGenerator
    {
        public static response ExportDosarToPdf(string templateFileName, Models.Dosar dosar)
        {
            try
            {
                FileStream fs = null;
                byte[] bs = null;
                if (File.Exists(templateFileName))
                    fs = new FileStream(templateFileName, FileMode.Open, FileAccess.Read);
                else
                    if (File.Exists(Path.Combine(CommonFunctions.GetPdfsFolder(), templateFileName)))
                    fs = new FileStream(Path.Combine(CommonFunctions.GetPdfsFolder(), templateFileName), FileMode.Open, FileAccess.Read);
                if (fs != null)
                {
                    bs = new byte[fs.Length];
                    int n = fs.Read(bs, 0, (int)fs.Length);
                }
                return ExportDosarToPdf(bs, dosar);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.Message, null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public static response ExportDosarToPdf(int authenticatedUserId, string connectionString, string templateFileName, Models.Dosar dosar)
        {
            try
            {
                FileStream fs = null;
                byte[] bs = null;
                if (File.Exists(templateFileName))
                    fs = new FileStream(templateFileName, FileMode.Open, FileAccess.Read);
                else
                    if (File.Exists(Path.Combine(CommonFunctions.GetPdfsFolder(), templateFileName)))
                    fs = new FileStream(Path.Combine(CommonFunctions.GetPdfsFolder(), templateFileName), FileMode.Open, FileAccess.Read);
                if (fs != null)
                {
                    bs = new byte[fs.Length];
                    int n = fs.Read(bs, 0, (int)fs.Length);
                }else
                {
                    bs = FileManager.GetTemplateFileFromDb(authenticatedUserId, connectionString, templateFileName);
                }
                return ExportDosarToPdf(bs, dosar);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.Message, null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public static response ExportDosarToPdf(byte[] template_file_content, Models.Dosar dosar)
        {
            try
            {
                MemoryStream ms = new MemoryStream(template_file_content);
                string fileName = dosar.NR_DOSAR_CASCO + "_cerere.pdf";
                FileStream fs = File.Open(Path.Combine(CommonFunctions.GetPdfsFolder(), fileName), FileMode.Create, FileAccess.ReadWrite);
                PdfFixedDocument poDocument = new PdfFixedDocument(ms);

                Models.SocietateAsigurare sCasco = dosar.GetSocietateCasco();
                Models.SocietateAsigurare sRca = dosar.GetSocietateRca();
                Models.Asigurat aCasco = dosar.GetAsiguratCasco();
                Models.Asigurat aRca = dosar.GetAsiguratRca();
                Models.Auto autoCasco = dosar.GetAutoCasco();
                Models.Auto autoRca = dosar.GetAutoRca();

                poDocument.Form.Fields["FieldSocietateCasco"].Value = sCasco.DENUMIRE;
                poDocument.Form.Fields["FieldAdresaSocietateCasco"].Value = sCasco.ADRESA;
                poDocument.Form.Fields["FieldCUISocietateCasco"].Value = sCasco.CUI;
                poDocument.Form.Fields["FieldContBancarSocietateCasco"].Value = sCasco.IBAN;
                poDocument.Form.Fields["FieldBancaSocietateCasco"].Value = sCasco.BANCA;
                poDocument.Form.Fields["FieldSocietateRCA"].Value = sRca.DENUMIRE;
                poDocument.Form.Fields["FieldAdresaSocietateRCA"].Value = sRca.ADRESA;
                poDocument.Form.Fields["FieldNrDosarCasco"].Value = dosar.NR_DOSAR_CASCO;
                poDocument.Form.Fields["FieldPolitaRCA"].Value = dosar.NR_POLITA_RCA;
                poDocument.Form.Fields["FieldPolitaCasco"].Value = dosar.NR_POLITA_CASCO;
                poDocument.Form.Fields["FieldAsiguratCasco"].Value = aCasco.DENUMIRE;
                poDocument.Form.Fields["FieldAsiguratRCA"].Value = aRca.DENUMIRE;
                poDocument.Form.Fields["FieldNrAutoCasco"].Value = autoCasco.NR_AUTO;
                poDocument.Form.Fields["FieldAutoCasco"].Value = autoCasco.MARCA + " " + autoCasco.MODEL;
                poDocument.Form.Fields["FieldNrAutoRCA"].Value = autoRca.NR_AUTO;
                poDocument.Form.Fields["FieldDataEveniment"].Value = Convert.ToDateTime(dosar.DATA_EVENIMENT).ToString("dd/MM/yyyy");
                poDocument.Form.Fields["FieldSuma"].Value = dosar.VALOARE_DAUNA.ToString();

                string docs = "";
                Models.DocumentScanat[] dsj = dosar.GetDocumente();
                foreach (Models.DocumentScanat doc in dsj)
                {
                    docs = String.Format("- {1}\r\n{0}", docs, (doc.DETALII != "" && doc.DETALII != null ? doc.DETALII : doc.DENUMIRE_FISIER));
                }
                poDocument.Form.Fields["FieldDocumente"].Value = docs;

                poDocument.Form.FlattenFields();

                poDocument.Save(fs);
                fs.Flush();
                fs.Dispose();
                string toReturn = Path.Combine(CommonFunctions.GetPdfsFolder(), fileName);
                return new response(true, toReturn, toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.Message, null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public static response ExportDocumenteDosarToPdf(Models.Dosar dosar)
        {
            try
            {
                PdfFixedDocument poDocument = new PdfFixedDocument();
                Models.DocumentScanat[] ds = dosar.GetDocumente();
                foreach (Models.DocumentScanat dsj in ds)
                {
                    try
                    {
                        MemoryStream ms = new MemoryStream(dsj.FILE_CONTENT);
                        switch (dsj.EXTENSIE_FISIER.Replace(".", "").ToLower())
                        {
                            case "pdf":
                                PdfFixedDocument pd = new PdfFixedDocument(ms);
                                for (int i = 0; i < pd.Pages.Count; i++)
                                    poDocument.Pages.Add(pd.Pages[i]);
                                break;
                            case "png":
                                Xfinium.Pdf.Graphics.PdfPngImage pngImg = new Xfinium.Pdf.Graphics.PdfPngImage(ms);
                                PdfPage p = new PdfPage();
                                p.Graphics.DrawImage(pngImg, 0, 0, p.Width, p.Height);
                                poDocument.Pages.Add(p);
                                break;
                            case "jpg":
                                Xfinium.Pdf.Graphics.PdfJpegImage jpgImg = new Xfinium.Pdf.Graphics.PdfJpegImage(ms);
                                p = new PdfPage();
                                p.Graphics.DrawImage(jpgImg, 0, 0, p.Width, p.Height);
                                poDocument.Pages.Add(p);
                                break;
                            case "tiff":
                                Xfinium.Pdf.Graphics.PdfTiffImage tiffImg = new Xfinium.Pdf.Graphics.PdfTiffImage(ms);
                                p = new PdfPage();
                                p.Graphics.DrawImage(tiffImg, 0, 0, p.Width, p.Height);
                                poDocument.Pages.Add(p);
                                break;
                            default:
                                throw new Exception("unsupportedFormat");
                        }
                    }
                    catch(Exception exp) { LogWriter.Log(exp); }
                }
                string fileName = dosar.NR_DOSAR_CASCO + "_documente.pdf";
                FileStream fs = File.Open(Path.Combine(CommonFunctions.GetPdfsFolder(), fileName), FileMode.Create, FileAccess.ReadWrite);
                poDocument.Save(fs);
                fs.Flush();
                fs.Dispose();
                return new response(true, Path.Combine(CommonFunctions.GetPdfsFolder(), fileName), Path.Combine(CommonFunctions.GetPdfsFolder(), fileName), null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.Message, null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public static response ExportDosarCompletToPdf(string templateFileName, Models.Dosar Dosar)
        {
            try
            {
                string f1 = null;
                response r1 = ExportDosarToPdf(templateFileName, Dosar);
                if (r1.Status)
                    f1 = r1.Message;

                string f2 = null;
                response r2 = ExportDocumenteDosarToPdf(Dosar);
                if (r2.Status)
                    f2 = r2.Message;

                if (r1.Status && r2.Status)
                {
                    FileStream fs1 = new FileStream(f1, FileMode.Open, FileAccess.Read);
                    PdfFixedDocument p1 = new PdfFixedDocument(fs1);
                    FileStream fs2 = new FileStream(f2, FileMode.Open, FileAccess.Read);
                    PdfFixedDocument p2 = new PdfFixedDocument(fs2);
                    for (int i = 0; i < p2.Pages.Count; i++)
                    {
                        p1.Pages.Add(p2.Pages[i]);
                    }
                    string fileNameToReturn = Dosar.NR_DOSAR_CASCO + ".pdf";
                    FileStream fs = File.Open(Path.Combine(CommonFunctions.GetPdfsFolder(), fileNameToReturn), FileMode.Create, FileAccess.ReadWrite);
                    p1.Save(fs);
                    fs.Flush();
                    fs.Dispose();
                    return new response(true, Path.Combine(CommonFunctions.GetPdfsFolder(), fileNameToReturn), Path.Combine(CommonFunctions.GetPdfsFolder(), fileNameToReturn), null, null);
                }
                r1.AddResponse(r2);
                return r1;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.Message, null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public static response ExportDosarCompletToPdf(int authenticatedUserId, string connectionString, string templateFileName, Models.Dosar Dosar)
        {
            try
            {
                string f1 = null;
                response r1 = ExportDosarToPdf(authenticatedUserId, connectionString, templateFileName, Dosar);
                if (r1.Status)
                    f1 = r1.Message;

                string f2 = null;
                response r2 = ExportDocumenteDosarToPdf(Dosar);
                if (r2.Status)
                    f2 = r2.Message;

                if (r1.Status && r2.Status)
                {
                    FileStream fs1 = new FileStream(f1, FileMode.Open, FileAccess.Read);
                    PdfFixedDocument p1 = new PdfFixedDocument(fs1);
                    FileStream fs2 = new FileStream(f2, FileMode.Open, FileAccess.Read);
                    PdfFixedDocument p2 = new PdfFixedDocument(fs2);
                    for (int i = 0; i < p2.Pages.Count; i++)
                    {
                        p1.Pages.Add(p2.Pages[i]);
                    }
                    string fileNameToReturn = Dosar.NR_DOSAR_CASCO + ".pdf";
                    FileStream fs = File.Open(Path.Combine(CommonFunctions.GetPdfsFolder(), fileNameToReturn), FileMode.Create, FileAccess.ReadWrite);
                    p1.Save(fs);
                    fs.Flush();
                    fs.Dispose();
                    return new response(true, Path.Combine(CommonFunctions.GetPdfsFolder(), fileNameToReturn), Path.Combine(CommonFunctions.GetPdfsFolder(), fileNameToReturn), null, null);
                }
                r1.AddResponse(r2);
                return r1;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.Message, null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }
    }
}