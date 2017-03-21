using System;
using Xfinium.Pdf;
using Xfinium.Pdf.Content;
using Xfinium.Pdf.FlowDocument;
using Xfinium.Pdf.Graphics.FormattedContent;
using Xfinium.Pdf.Graphics;
using System.IO;
using System.Collections.Generic;

namespace SOCISA
{
    public class Articol
    {
        public string Ordin { get; set; }
        public string ArticolPlata { get; set; }
        public string ArticolObiectiuni { get; set; }
        public string NrZile { get; set; }

        public Articol(string o, string ap, string ao, string nz)
        {
            Ordin = o; ArticolPlata = ap; ArticolObiectiuni = ao; NrZile = nz;
        }
    }

    public static class Articole
    {
        public static Articol[] articole
        {
            get
            {
                List<Articol> l = new List<Articol>();
                l.Add(new Articol("Ordinul CSA nr. 14/2011", "art. 64, alin. 2, lit. a", "art. 64, alin. 2, lit. b", "15"));
                l.Add(new Articol("Norma ASF nr. 23/2014", "art. 58, alin. 2, lit. a", "art. 58, alin. 2, lit. b", "15"));
                l.Add(new Articol("OUG nr. 54/2016", "art. 20, alin. 2", "art. 20, alin. 1, lit. b", "30"));

                return l.ToArray();
            }
        }
    }

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
                string fileName = dosar.NR_DOSAR_CASCO.Replace('/', '_').Replace(' ', '_') + "_cerere.pdf";
                FileStream fs = File.Open(Path.Combine(CommonFunctions.GetPdfsFolder(), fileName), FileMode.Create, FileAccess.ReadWrite);
                PdfFixedDocument poDocument = new PdfFixedDocument(ms);
                List<string> pdfText = new List<string>();
                string text = "";
                foreach (PdfPage p in poDocument.Pages)
                {
                    PdfContentExtractor ce = new PdfContentExtractor(p);
                    text += ce.ExtractText();
                }
                string[] paragraphs = text.Split(new string[] { "\r\n \r\n" }, StringSplitOptions.None);
                foreach (string s in paragraphs)
                {
                    pdfText.Add(s.Replace("\r\n", ""));
                    //pdfText.Add(s);
                }
                /*
                PdfFlowDocument pf = new PdfFlowDocument();
                pf.PageCreated += Pf_PageCreated;
                PdfFlowTextContent pftc = GeneratePdfContent(pdfText, dosar);
                pftc.InnerMargins = new PdfFlowContentMargins(0, 0, 0, 0);
                pftc.OuterMargins = new PdfFlowContentMargins(0, 0, 0, 0);
                pf.AddContent(pftc);
                */
                PdfFixedDocument pf = DrawFormattedContent(GeneratePdfContent(pdfText, dosar));
                pf.Save(fs);
                fs.Flush();
                fs.Dispose();
                string toReturn = Path.Combine(CommonFunctions.GetPdfsFolder(), fileName);
                return new response(true, toReturn, toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.Message, null, null, new List<Error>() { new Error(exp) }); }
        }

        private static void Pf_PageCreated(object sender, PdfFlowPageCreatedEventArgs e)
        {
            e.PageDefaults.Margins.Left = 60;
            e.PageDefaults.Margins.Right = 50;
            e.PageDefaults.Margins.Bottom = 50;
            e.PageDefaults.Margins.Top = 50;
            e.PageDefaults.Height = 842;
            e.PageDefaults.Width = 595;
        }

        //public static PdfFlowTextContent GeneratePdfContent(List<string> pdfText, Models.Dosar dosar)
        public static PdfFormattedContent GeneratePdfContent(List<string> pdfText, Models.Dosar dosar)            
        {
            Models.SocietateAsigurare sCasco = (Models.SocietateAsigurare)dosar.GetSocietateCasco().Result;
            Models.SocietateAsigurare sRca = (Models.SocietateAsigurare)dosar.GetSocietateRca().Result;
            Models.Asigurat aCasco = (Models.Asigurat)dosar.GetAsiguratCasco().Result;
            Models.Asigurat aRca = (Models.Asigurat)dosar.GetAsiguratRca().Result;
            Models.Auto autoCasco = (Models.Auto)dosar.GetAutoCasco().Result;
            Models.Auto autoRca = (Models.Auto)dosar.GetAutoRca().Result;

            Dictionary<string, string> field_names = new Dictionary<string, string>();
            field_names.Add("{{NR_SCA}}", dosar.NR_SCA == null || dosar.NR_SCA.Trim() == "" ? "_________" : dosar.NR_SCA);
            field_names.Add("{{DATA_SCA}}", dosar.DATA_SCA == null ? "____________" : Convert.ToDateTime(dosar.DATA_SCA).Year.ToString());
            field_names.Add("{{NR_DOSAR_CASCO}}", dosar.NR_DOSAR_CASCO == null || dosar.NR_DOSAR_CASCO.Trim() == "" ? "_________________" : dosar.NR_DOSAR_CASCO);
            field_names.Add("{{VALOARE_DAUNA}}", dosar.VALOARE_DAUNA.ToString().Trim() == "" ? "___________" : dosar.VALOARE_DAUNA.ToString());
            field_names.Add("{{NR_POLITA_CASCO}}", dosar.NR_POLITA_CASCO == null || dosar.NR_POLITA_CASCO.Trim() == "" ? "_________________" : dosar.NR_POLITA_CASCO);
            field_names.Add("{{NR_POLITA_RCA}}", dosar.NR_POLITA_RCA == null || dosar.NR_POLITA_RCA.Trim() == "" ? "_________________" : dosar.NR_POLITA_RCA);
            field_names.Add("{{DATA_EVENIMENT}}", dosar.DATA_EVENIMENT == null ? "___________" : Convert.ToDateTime(dosar.DATA_EVENIMENT).ToString("dd/MM/yyyy"));

            field_names.Add("{{SOCIETATE_CASCO}}", sCasco.DENUMIRE == null || sCasco.DENUMIRE.Trim() == "" ? "__________________________________" : sCasco.DENUMIRE);
            field_names.Add("{{ADRESA_SOCIETATE_CASCO}}", sCasco.ADRESA == null || sCasco.ADRESA.Trim() == "" ? "___________________________________________________" : sCasco.ADRESA);
            field_names.Add("{{NR_REG_COM_SOCIETATE_CASCO}}", sCasco.NR_REG_COM == null || sCasco.NR_REG_COM.Trim() == "" ? "_______________" : sCasco.NR_REG_COM.ToUpper());
            field_names.Add("{{CUI_SOCIETATE_CASCO}}", sCasco.CUI == null || sCasco.CUI.Trim() == "" ? "______________" : sCasco.CUI.ToUpper());
            field_names.Add("{{IBAN_SOCIETATE_CASCO}}", sCasco.IBAN == null || sCasco.IBAN.Trim() == "" ? "__________________________________________" : sCasco.IBAN.ToUpper());
            field_names.Add("{{BANCA_SOCIETATE_CASCO}}", sCasco.BANCA == null || sCasco.BANCA.Trim() == "" ? "____________________________________" : sCasco.BANCA.ToUpper());

            field_names.Add("{{SOCIETATE_RCA}}", sRca.DENUMIRE == null || sRca.DENUMIRE.Trim() == "" ? "_________________________________" : sRca.DENUMIRE);
            field_names.Add("{{ADRESA_SOCIETATE_RCA}}", sRca.ADRESA == null || sRca.ADRESA.Trim() == "" ? "___________________________________________________" : sRca.ADRESA);
            field_names.Add("{{ASIGURAT_CASCO}}", aCasco.DENUMIRE == null || aCasco.DENUMIRE.Trim() == "" ? "__________________________________" : aCasco.DENUMIRE);
            field_names.Add("{{ASIGURAT_RCA}}", aRca.DENUMIRE == null || aRca.DENUMIRE.Trim() == "" ? "__________________________________" : aRca.DENUMIRE);

            field_names.Add("{{NR_AUTO_CASCO}}", autoCasco.NR_AUTO == null || autoCasco.NR_AUTO.Trim() == "" ? "____________" : autoCasco.NR_AUTO.ToUpper());
            field_names.Add("{{MARCA_AUTO_CASCO}}", autoCasco.MARCA == null || autoCasco.MARCA.Trim() == "" ? "______________" : autoCasco.MARCA.ToUpper());
            field_names.Add("{{NR_AUTO_RCA}}", autoRca.NR_AUTO == null || autoRca.NR_AUTO.Trim() == "" ? "____________" : autoRca.NR_AUTO.ToUpper());
            field_names.Add("{{PROPRIETAR_AUTO_RCA}}", "_______________________");

            field_names.Add("{{NORMA}}", dosar.DATA_EVENIMENT < new DateTime(2015, 1, 1) ? Articole.articole[0].Ordin : dosar.DATA_EVENIMENT >= new DateTime(2015, 1, 1) && dosar.DATA_EVENIMENT <= new DateTime(2016, 12, 22) ? Articole.articole[1].Ordin : Articole.articole[2].Ordin);
            field_names.Add("{{ARTICOL_PLATA}}", dosar.DATA_EVENIMENT < new DateTime(2015, 1, 1) ? Articole.articole[0].ArticolPlata : dosar.DATA_EVENIMENT >= new DateTime(2015, 1, 1) && dosar.DATA_EVENIMENT <= new DateTime(2016, 12, 22) ? Articole.articole[1].ArticolPlata : Articole.articole[2].ArticolPlata);
            field_names.Add("{{ARTICOL_OBIECTIUNI}}", dosar.DATA_EVENIMENT < new DateTime(2015, 1, 1) ? Articole.articole[0].ArticolObiectiuni : dosar.DATA_EVENIMENT >= new DateTime(2015, 1, 1) && dosar.DATA_EVENIMENT <= new DateTime(2016, 12, 22) ? Articole.articole[1].ArticolObiectiuni : Articole.articole[2].ArticolObiectiuni);
            field_names.Add("{{TERMEN}}", dosar.DATA_EVENIMENT < new DateTime(2015, 1, 1) ? Articole.articole[0].NrZile : dosar.DATA_EVENIMENT >= new DateTime(2015, 1, 1) && dosar.DATA_EVENIMENT <= new DateTime(2016, 12, 22) ? Articole.articole[1].NrZile : Articole.articole[2].NrZile);
            field_names.Add("{{CAZ}}", dosar.CAZ == null || dosar.CAZ.Trim() == "" ? "_____" : dosar.CAZ);
            field_names.Add("{{DATA}}", DateTime.Now.ToString("dd/MM/yyyy"));


            string docs = "";
            Models.DocumentScanat[] dsj = (Models.DocumentScanat[])dosar.GetDocumente().Result;
            foreach (Models.DocumentScanat doc in dsj)
            {
                Models.Nomenclator tip_document = (Models.Nomenclator)doc.GetTipDocument().Result;
                //docs = String.Format("- {1}\r\n{0}", docs, (doc.DETALII != "" && doc.DETALII != null ? doc.DETALII : doc.DENUMIRE_FISIER));
                docs = String.Format("- {1}\r\n{0}", docs, tip_document.DENUMIRE + " " + (doc.DETALII != "" && doc.DETALII != null ? doc.DETALII : ""));
            }
            field_names.Add("{{DOCUMENTE}}", docs);


            PdfAnsiTrueTypeFont boldFont = new PdfAnsiTrueTypeFont(new FileStream("arialbold.ttf", FileMode.Open, FileAccess.Read, FileShare.Read), 12, true);
            PdfAnsiTrueTypeFont regularFont = new PdfAnsiTrueTypeFont(new FileStream("arial.ttf", FileMode.Open, FileAccess.Read, FileShare.Read), 12, true);

            PdfFormattedContent pfc = new PdfFormattedContent();
            foreach (string s in pdfText)
            {
                PdfFormattedParagraph pfp = new PdfFormattedParagraph();
                pfp.LineSpacingMode = PdfFormattedParagraphLineSpacing.Multiple;
                pfp.LineSpacing = 1.3;
                pfp.SpacingAfter = 15;

                if (s.IndexOf("{{NR_SCA}}") > -1)
                    pfp.HorizontalAlign = PdfStringHorizontalAlign.Right;
                else if (s.IndexOf("CERERE DE DESPAGUBIRE") > -1)
                {
                    pfp.HorizontalAlign = PdfStringHorizontalAlign.Center;
                }
                else
                {
                    pfp.HorizontalAlign = PdfStringHorizontalAlign.Justified;
                    if(s.IndexOf("Catre") < 0 && s.IndexOf("{{DOCUMENTE}}") < 0 && s.IndexOf("$$") < 0)
                        pfp.FirstLineIndent = 30;
                    if (s.IndexOf("{{DOCUMENTE}}") > -1 || s.IndexOf("$$") > -1)
                        pfp.LeftIndentation = 50;
                }

                List<string> splitters = new List<string>();
                foreach (KeyValuePair<string,string> field in field_names)
                {
                    if (s.IndexOf(field.Key) > -1)
                    {
                        splitters.Add(field.Key);
                    }
                }
                string[] sBlocks = null;
                if (splitters.Count > 0)
                    sBlocks = s.Split(splitters.ToArray(), StringSplitOptions.None);
                else
                    sBlocks = new string[] { s };

                int splitter_count = 0;
                for(int i = 0; i < sBlocks.Length; i++)
                {
                    try
                    {
                        PdfFormattedTextBlock b1 = new PdfFormattedTextBlock(sBlocks[i].Replace("$$","\r\n"), sBlocks[i].IndexOf("CERERE DE DESPAGUBIRE") > -1 ? boldFont : regularFont);
                        pfp.Blocks.Add(b1);
                        string theFuckingWrightSplitter = "";
                        //if (splitter_count < splitters.Count)
                        {
                            foreach (string splitter in splitters)
                            {
                                if (s.IndexOf(sBlocks[i] + splitter) > -1)
                                {
                                    theFuckingWrightSplitter = splitter;
                                    splitter_count++;
                                    splitters.Remove(splitter);
                                    break;
                                }
                            }
                            //PdfFormattedTextBlock b2 = new PdfFormattedTextBlock(field_names[splitters[i]], boldFont);
                            PdfFormattedTextBlock b2 = new PdfFormattedTextBlock(field_names[theFuckingWrightSplitter], boldFont);
                            pfp.Blocks.Add(b2);
                        }
                    }
                    catch { }
                }
                pfc.Paragraphs.Add(pfp);
            }
            //pfc.SplitByBox(485, 742);
            //PdfFlowTextContent pftc = new PdfFlowTextContent(pfc);
            //return pftc;
            return pfc;
        }

        private static PdfFixedDocument DrawFormattedContent(PdfFormattedContent fc)
        {
            PdfFixedDocument _doc = new PdfFixedDocument();
            PdfPage page = _doc.Pages.Add();
            page.Width = 595; page.Height = 842;
            PdfFormattedContent fragment = fc.SplitByBox(485, 742);
            while (fragment != null)
            {
                page.Graphics.DrawFormattedContent(fragment, 60, 50);
                page.Graphics.CompressAndClose();

                fragment = fc.SplitByBox(485, 742);
                if (fragment != null)
                {
                    page = _doc.Pages.Add();
                    page.Width = 595; page.Height = 842;
                }
            }
            return _doc;
        }

        public static response ExportDosarToPdfWithPdfForm(byte[] template_file_content, Models.Dosar dosar)
        {
            try
            {
                MemoryStream ms = new MemoryStream(template_file_content);
                string fileName = dosar.NR_DOSAR_CASCO.Replace('/','_').Replace(' ','_') + "_cerere.pdf";
                FileStream fs = File.Open(Path.Combine(CommonFunctions.GetPdfsFolder(), fileName), FileMode.Create, FileAccess.ReadWrite);
                PdfFixedDocument poDocument = new PdfFixedDocument(ms);

                Models.SocietateAsigurare sCasco = (Models.SocietateAsigurare)dosar.GetSocietateCasco().Result;
                Models.SocietateAsigurare sRca = (Models.SocietateAsigurare)dosar.GetSocietateRca().Result;
                Models.Asigurat aCasco = (Models.Asigurat)dosar.GetAsiguratCasco().Result;
                Models.Asigurat aRca = (Models.Asigurat)dosar.GetAsiguratRca().Result;
                Models.Auto autoCasco = (Models.Auto) dosar.GetAutoCasco().Result;
                Models.Auto autoRca = (Models.Auto)dosar.GetAutoRca().Result;

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
                Models.DocumentScanat[] dsj = (Models.DocumentScanat[])dosar.GetDocumente().Result;
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
                Models.DocumentScanat[] ds = (Models.DocumentScanat[])dosar.GetDocumente().Result;
                foreach (Models.DocumentScanat dsj in ds)
                {
                    try
                    {
                        if (dsj.VIZA_CASCO)
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
                                case "jpeg":
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
                    }
                    catch(Exception exp) { LogWriter.Log(exp); }
                }
                if (poDocument.Pages.Count > 0)
                {
                    string fileName = dosar.NR_DOSAR_CASCO.Replace('/', '_').Replace(' ', '_') + "_documente.pdf";
                    FileStream fs = File.Open(Path.Combine(CommonFunctions.GetPdfsFolder(), fileName), FileMode.Create, FileAccess.ReadWrite);
                    poDocument.Save(fs);
                    fs.Flush();
                    fs.Dispose();
                    return new response(true, Path.Combine(CommonFunctions.GetPdfsFolder(), fileName), Path.Combine(CommonFunctions.GetPdfsFolder(), fileName), null, null);
                }
                else
                {
                    return new response(false, ErrorParser.ErrorMessage("dosarFaraDocumente").ERROR_MESSAGE, null, null, new List<Error>() { ErrorParser.ErrorMessage("dosarFaraDocumente") });
                }
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
                    string fileNameToReturn = Dosar.NR_DOSAR_CASCO.Replace('/', '_').Replace(' ', '_') + ".pdf";
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
                    string fileNameToReturn = Dosar.NR_DOSAR_CASCO.Replace('/', '_').Replace(' ', '_') + ".pdf";
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