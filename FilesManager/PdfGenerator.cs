using System;
using Xfinium.Pdf;
using System.IO;

namespace SOCISA
{
    public static class PdfGenerator
    {
        public static string ExportDosarToPdf(string templateFileName, Models.Dosar dosar)
        {
            try
            {
                FileStream fs = new FileStream(templateFileName, FileMode.Open, FileAccess.Read);
                byte[] bs = new byte[fs.Length];
                int n = fs.Read(bs, 0, (int)fs.Length);
                return ExportDosarToPdf(bs, dosar);
            }
            catch
            {
                return null;
            }
        }

        public static string ExportDosarToPdf(byte[] template_file_content, Models.Dosar dosar)
        {
            MemoryStream ms = new MemoryStream(template_file_content);
            string fileName = dosar.NR_DOSAR_CASCO + "_cerere.pdf";
            FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite);
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
            return fileName;
        }

        public static string ExportDocumenteDosarToPdf(Models.Dosar dosar)
        {
            try
            {
                PdfFixedDocument poDocument = new PdfFixedDocument();
                foreach (Models.DocumentScanat dsj in dosar.GetDocumente())
                {
                    MemoryStream ms = new MemoryStream(dsj.FILE_CONTENT);
                    PdfFixedDocument pd = new PdfFixedDocument(ms);

                    switch (dsj.EXTENSIE_FISIER.Replace(".", "").ToLower())
                    {
                        case "pdf":
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
                    }
                }
                string fileName = dosar.NR_DOSAR_CASCO + "_documente.pdf";
                FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite);
                poDocument.Save(fs);
                fs.Flush();
                fs.Dispose();
                return fileName;
            }
            catch { return null; }
        }
        
        public static string ExportDosarCompletToPdf(string templateFileName, Models.Dosar Dosar)
        {
            try
            {
                string f1 = ExportDosarToPdf(templateFileName, Dosar);
                string f2 = ExportDocumenteDosarToPdf(Dosar);

                FileStream fs1 = new FileStream(f1, FileMode.Open, FileAccess.Read);
                PdfFixedDocument p1 = new PdfFixedDocument(fs1);
                FileStream fs2 = new FileStream(f2, FileMode.Open, FileAccess.Read);
                PdfFixedDocument p2 = new PdfFixedDocument(fs2);
                for (int i = 0; i < p2.Pages.Count; i++)
                {
                    p1.Pages.Add(p2.Pages[i]);
                }
                string fileNameToReturn = Dosar.NR_DOSAR_CASCO + ".pdf";
                FileStream fs = File.Open(fileNameToReturn, FileMode.Create, FileAccess.ReadWrite);
                p1.Save(fs);
                fs.Flush();
                fs.Dispose();
                return fileNameToReturn;
            }catch { return null; }
        }
    }
}