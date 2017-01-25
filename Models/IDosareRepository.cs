using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface IDosareRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response Find(int _id);
        response Insert(Dosar item);
        response InsertWithErrors(Dosar item);
        response Update(Dosar item);
        response UpdateWithErrors(Dosar item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);
        response Delete(Dosar item);
        response DeleteWithErrors(Dosar item);
        response HasChildrens(Dosar item, string tableName);
        response HasChildren(Dosar item, string tableName, int childrenId);
        response GetChildrens(Dosar item, string tableName);
        response GetChildren(Dosar item, string tableName, int childrenId);

        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);

        Utilizator[] GetUtilizatori(Dosar item);
        SocietateAsigurare GetSocietateCasco(Dosar item);
        SocietateAsigurare GetSocietateRca(Dosar item);
        Auto GetAutoCasco(Dosar item);
        Auto GetAutoRca(Dosar item);
        Asigurat GetAsiguratCasco(Dosar item);
        Asigurat GetAsiguratRca(Dosar item);
        Intervenient GetIntervenient(Dosar item);
        DocumentScanat[] GetDocumente(Dosar item);
        Proces[] GetProcese(Dosar item);
        Nomenclator GetTipDosar(Dosar item);
        Utilizator[] GetInvolvedParties(Dosar item);

        response ExportDocumenteDosarToPdf(Dosar item);
        response ExportDocumenteDosarToPdf(int _id);
        response ExportDosarToPdf(string templateFileName, Dosar item);
        response ExportDosarToPdf(string templateFileName, int _id);
        response ExportDosarCompletToPdf(string templateFileName, Dosar item);
        response ExportDosarCompletToPdf(string templateFileName, int _id);
        response ExportDosarCompletToPdf(Dosar item);
        response ExportDosarCompletToPdf(int _id);
        void Import(Dosar item);
        response SetDataUltimeiModificari(DateTime data, Dosar item);
        DateTime? GetDataUltimeiModificari(Dosar item);
    }

    public class DosareRepository : IDosareRepository
    {
        private const string _TEMPLATE_CERERE_DESPAGUBIRE = "Cerere_despagubire_t.pdf";
        private string connectionString;
        private int authenticatedUserId;

        public DosareRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Dosar a = new Dosar(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Dosar[] toReturn = new Dosar[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Dosar)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public response GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(Dosar), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Dosar a = new Dosar(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Dosar[] toReturn = new Dosar[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Dosar)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                Dosar item = new Dosar(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }

        }

        public response Insert(Dosar item)
        {
            return item.Insert();
        }

        public response InsertWithErrors(Dosar item)
        {
            return item.InsertWithErrors();
        }

        public response Update(Dosar item)
        {
            return item.Update();
        }

        public response UpdateWithErrors(Dosar item)
        {
            return item.UpdateWithErrors();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Dosar item = JsonConvert.DeserializeObject<Dosar>(Find(id).Message);
            return item.Update(fieldValueCollection);
        }

        public response Update(string fieldValueCollection)
        {
            Dosar tmpItem = JsonConvert.DeserializeObject<Dosar>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            return JsonConvert.DeserializeObject<Dosar>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
        }

        public response Delete(Dosar item)
        {
            return item.Delete();
        }

        public response DeleteWithErrors(Dosar item)
        {
            return item.DeleteWithErrors();
        }
        public response HasChildrens(Dosar item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(Dosar item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(Dosar item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(Dosar item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }

        public response Delete(int _id)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Dosar>(obj.Message).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Dosar>(obj.Message).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Dosar>(obj.Message).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Dosar>(obj.Message).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Dosar>(obj.Message).GetChildren(tableName, childrenId);
        }

        public Utilizator[] GetUtilizatori(Dosar item)
        {
            return item.GetUtilizatori();
        }
        public SocietateAsigurare GetSocietateCasco(Dosar item)
        {
            return item.GetSocietateCasco();
        }
        public SocietateAsigurare GetSocietateRca(Dosar item)
        {
            return item.GetSocietateRca();
        }
        public Auto GetAutoCasco(Dosar item)
        {
            return item.GetAutoCasco();
        }
        public Auto GetAutoRca(Dosar item)
        {
            return item.GetAutoRca();
        }
        public Asigurat GetAsiguratCasco(Dosar item)
        {
            return item.GetAsiguratCasco();
        }
        public Asigurat GetAsiguratRca(Dosar item)
        {
            return item.GetAsiguratRca();
        }
        public Intervenient GetIntervenient(Dosar item)
        {
            return item.GetIntervenient();
        }
        public DocumentScanat[] GetDocumente(Dosar item)
        {
            return item.GetDocumente();
        }

        public Proces[] GetProcese(Dosar item)
        {
            return item.GetProcese();
        }

        public Nomenclator GetTipDosar(Dosar item)
        {
            return item.GetTipDosar();
        }

        public response ExportDocumenteDosarToPdf(Dosar item)
        {
            return item.ExportDocumenteDosarToPdf();
        }
        public response ExportDocumenteDosarToPdf(int _id)
        {
            return JsonConvert.DeserializeObject<Dosar>(Find(_id).Message).ExportDocumenteDosarToPdf();
        }
        public response ExportDosarToPdf(string templateFileName, Dosar item)
        {
            return item.ExportDosarToPdf(templateFileName);
        }
        public response ExportDosarToPdf(string templateFileName, int _id)
        {
            return JsonConvert.DeserializeObject<Dosar>(Find(_id).Message).ExportDosarToPdf(templateFileName);
        }
        public response ExportDosarCompletToPdf(string templateFileName, Dosar item)
        {
            return item.ExportDosarCompletToPdf(templateFileName);
        }
        public response ExportDosarCompletToPdf(Dosar item)
        {
            return item.ExportDosarCompletToPdf(_TEMPLATE_CERERE_DESPAGUBIRE);
        }
        public response ExportDosarCompletToPdf(string templateFileName, int _id)
        {
            return JsonConvert.DeserializeObject<Dosar>(Find(_id).Message).ExportDosarCompletToPdf(templateFileName);
        }
        public response ExportDosarCompletToPdf(int _id)
        {
            return JsonConvert.DeserializeObject<Dosar>(Find(_id).Message).ExportDosarCompletToPdf(_TEMPLATE_CERERE_DESPAGUBIRE);
        }
        public void Import(Dosar item)
        {
            item.Import();
        }
        public response SetDataUltimeiModificari(DateTime data, Dosar item)
        {
            return item.SetDataUltimeiModificari(data);
        }
        public DateTime? GetDataUltimeiModificari(Dosar item)
        {
            return item.GetDataUltimeiModificari();
        }
        public Utilizator[] GetInvolvedParties(Dosar item)
        {
            return item.GetInvolvedParties();
        }

        /// <summary>
        /// Metoda pentru incarcarea vectorului de Dosare din fisierul Excel de importat
        /// </summary>
        /// <param name="sheet">Denumirea Sheet-ului din fisierul Excel in care se gasesc Dosarele de importat</param>
        /// <param name="fileName">Denumirea completa a fisierului cu Dosarele de importat</param>
        /// <returns>vector de {SOCISA.response, SOCISA.DosareJson}</returns>
        public object[] GetDosareFromExcel(string sheet, string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            ExcelPackage ep = new ExcelPackage(fi);
            ExcelWorksheet ews = ep.Workbook.Worksheets[sheet];

            Dictionary<string, int> columnNames = new Dictionary<string, int>();
            int colIndex = 1;
            foreach (var firstRowCell in ews.Cells[1, 1, 1, ews.Dimension.End.Column])
            {
                columnNames.Add(firstRowCell.Text, colIndex);
                colIndex++;
            }
            List<object[]> toReturnList = new List<object[]>();
            //TO DO: trebuie stabilita maparea exacta cu coloanele din Excel !!!
            for (var rowNumber = 2; rowNumber <= ews.Dimension.End.Row; rowNumber++)
            {
                try
                {
                    response toReturn = new response(true, "", null, null, new List<Error>()); ;
                    response r = new response();

                    Asigurat asigCasco = new Asigurat(authenticatedUserId, connectionString);
                    Asigurat asigRca = new Asigurat(authenticatedUserId, connectionString);
                    SocietateAsigurare sCasco = new SocietateAsigurare(authenticatedUserId, connectionString);
                    SocietateAsigurare sRca = new SocietateAsigurare(authenticatedUserId, connectionString);
                    Intervenient intervenient = new Intervenient(authenticatedUserId, connectionString);
                    Auto autoCasco = new Auto(authenticatedUserId, connectionString);
                    Auto autoRca = new Auto(authenticatedUserId, connectionString);
                    Nomenclator tipDosar = new Nomenclator(authenticatedUserId, connectionString, "tip_dosare");
                    Dosar dosar = new Dosar(authenticatedUserId, connectionString);

                    try
                    {
                        try
                        {
                            asigCasco = new Asigurat(authenticatedUserId, connectionString, ews.Cells[rowNumber, columnNames["Asigurat CASCO"]].Text.Trim());
                        }
                        catch { asigCasco = null; }
                        if (asigCasco == null && columnNames.ContainsKey("Asigurat CASCO"))
                        {
                            asigCasco = new Asigurat();
                            asigCasco.DENUMIRE = ews.Cells[rowNumber, columnNames["Asigurat CASCO"]].Text.Trim();
                            r = asigCasco.Insert();
                            if (r.Status && r.InsertedId != null)
                            {
                                asigCasco.ID = r.InsertedId;
                            }
                            else
                            {
                                toReturn.AddResponse(r);
                            }
                        }
                        dosar.ID_ASIGURAT_CASCO = asigCasco.ID;
                    }
                    catch
                    {
                        Error err = ErrorParser.ErrorMessage("couldNotInsertAsiguratCasco");
                        List<Error> errs = new List<Error>();
                        errs.Add(err);
                        r = new response(false, err.ERROR_MESSAGE, null, null, errs);
                        toReturn.AddResponse(r);
                    }

                    try
                    {
                        try
                        {
                            asigRca = new Asigurat(authenticatedUserId, connectionString, ews.Cells[rowNumber, columnNames["Asigurat RCA"]].Text.Trim());
                        }
                        catch { asigRca = null; }
                        if (asigRca == null && columnNames.ContainsKey("Asigurat RCA"))
                        {
                            asigRca = new Asigurat();
                            asigRca.DENUMIRE = ews.Cells[rowNumber, columnNames["Asigurat RCA"]].Text.Trim();
                            r = asigRca.Insert();
                            if (r.Status && r.InsertedId != null)
                            {
                                asigRca.ID = r.InsertedId;
                            }
                            else
                            {
                                toReturn.AddResponse(r);
                            }
                        }
                        dosar.ID_ASIGURAT_RCA = asigRca.ID;
                    }
                    catch
                    {
                        Error err = ErrorParser.ErrorMessage("couldNotInsertAsiguratRca");
                        List<Error> errs = new List<Error>();
                        errs.Add(err);
                        r = new response(false, err.ERROR_MESSAGE, null, null, errs);
                        toReturn.AddResponse(r);
                    }

                    try
                    {
                        try
                        {
                            sCasco = new SocietateAsigurare(authenticatedUserId, connectionString, ews.Cells[rowNumber, columnNames["Asigurator CASCO"]].Text.Trim());
                        }
                        catch { sCasco = null; }
                        if (sCasco == null && columnNames.ContainsKey("Asigurator CASCO"))
                        {
                            sCasco = new SocietateAsigurare();
                            sCasco.DENUMIRE_SCURTA = ews.Cells[rowNumber, columnNames["Asigurator CASCO"]].Text.Trim();
                            r = sCasco.Insert();
                            if (r.Status && r.InsertedId != null)
                            {
                                sCasco.ID = r.InsertedId;
                            }
                            else
                            {
                                toReturn.AddResponse(r);
                            }
                        }
                        dosar.ID_SOCIETATE_CASCO = sCasco.ID;
                    }
                    catch
                    {
                        Error err = ErrorParser.ErrorMessage("couldNotInsertAsiguratorCasco");
                        List<Error> errs = new List<Error>();
                        errs.Add(err);
                        r = new response(false, err.ERROR_MESSAGE, null, null, errs);
                        toReturn.AddResponse(r);
                    }

                    try
                    {
                        try
                        {
                            sRca = new SocietateAsigurare(authenticatedUserId, connectionString, ews.Cells[rowNumber, columnNames["Asigurator RCA"]].Text.Trim());
                        }
                        catch { sRca = null; }
                        if (sRca == null && columnNames.ContainsKey("Asigurator RCA"))
                        {
                            sRca = new SocietateAsigurare();
                            sRca.DENUMIRE_SCURTA = ews.Cells[rowNumber, columnNames["Asigurator RCA"]].Text.Trim();
                            r = sRca.Insert();
                            if (r.Status && r.InsertedId != null)
                            {
                                sRca.ID = r.InsertedId;
                            }
                            else
                            {
                                toReturn.AddResponse(r);
                            }
                        }
                        dosar.ID_SOCIETATE_RCA = sRca.ID;
                    }
                    catch
                    {
                        Error err = ErrorParser.ErrorMessage("couldNotInsertAsiguratorRca");
                        List<Error> errs = new List<Error>();
                        errs.Add(err);
                        r = new response(false, err.ERROR_MESSAGE, null, null, errs);
                        toReturn.AddResponse(r);
                    }

                    try
                    {
                        try
                        {
                            autoCasco = new Auto(authenticatedUserId, connectionString, ews.Cells[rowNumber, columnNames["Auto CASCO"]].Text.Trim());
                        }
                        catch { autoCasco = null; }
                        if (autoCasco == null && columnNames.ContainsKey("Auto CASCO"))
                        {
                            autoCasco = new Auto();
                            autoCasco.NR_AUTO = ews.Cells[rowNumber, columnNames["Auto CASCO"]].Text.Trim();
                            autoCasco.SERIE_SASIU = ews.Cells[rowNumber, columnNames["Serie sasiu CASCO"]].Text.Trim();
                            r = autoCasco.Insert();
                            if (r.Status && r.InsertedId != null)
                            {
                                autoCasco.ID = r.InsertedId;
                            }
                            else
                            {
                                toReturn.AddResponse(r);
                            }
                        }
                        dosar.ID_AUTO_CASCO = autoCasco.ID;
                    }
                    catch
                    {
                        Error err = ErrorParser.ErrorMessage("couldNotInsertAutoCasco");
                        List<Error> errs = new List<Error>();
                        errs.Add(err);
                        r = new response(false, err.ERROR_MESSAGE, null, null, errs);
                        toReturn.AddResponse(r);
                    }

                    try
                    {
                        try
                        {
                            autoRca = new Auto(authenticatedUserId, connectionString, ews.Cells[rowNumber, columnNames["Auto RCA"]].Text.Trim());
                        }
                        catch { autoRca = null; }
                        if (autoRca == null && columnNames.ContainsKey("Auto RCA"))
                        {
                            autoRca = new Auto();
                            autoRca.NR_AUTO = ews.Cells[rowNumber, columnNames["Auto RCA"]].Text.Trim();
                            autoRca.SERIE_SASIU = ews.Cells[rowNumber, columnNames["Serie sasiu RCA"]].Text.Trim();
                            r = autoRca.Insert();
                            if (r.Status && r.InsertedId != null)
                            {
                                autoRca.ID = r.InsertedId;
                            }
                            else
                            {
                                toReturn.AddResponse(r);
                            }
                        }
                        dosar.ID_AUTO_RCA = autoRca.ID;
                    }
                    catch
                    {
                        Error err = ErrorParser.ErrorMessage("couldNotInsertAutoRca");
                        List<Error> errs = new List<Error>();
                        errs.Add(err);
                        r = new response(false, err.ERROR_MESSAGE, null, null, errs);
                        toReturn.AddResponse(r);
                    }

                    try
                    {
                        try
                        {
                            intervenient = new Intervenient(authenticatedUserId, connectionString, ews.Cells[rowNumber, columnNames["Intervenient"]].Text.Trim());
                        }
                        catch { intervenient = null; }
                        if (intervenient == null && columnNames.ContainsKey("Intervenient"))
                        {
                            intervenient = new Intervenient();
                            intervenient.DENUMIRE = ews.Cells[rowNumber, columnNames["Intervenient"]].Text.Trim();
                            r = intervenient.Insert();
                            intervenient.ID = r.InsertedId;
                        }
                        dosar.ID_INTERVENIENT = intervenient.ID;
                    }
                    catch
                    {
                        /*
                        Error err = ErrorParser.ErrorMessage("couldNotInsertIntervenient");
                        List<Error> errs = new List<Error>();
                        errs.Add(err);
                        r = new response(false, err.ERROR_MESSAGE, null, errs);
                        toReturn.AddResponse(r);
                        */
                    }

                    try { dosar.NR_DOSAR_CASCO = ews.Cells[rowNumber, columnNames["Nr# CASCO"]].Text.Trim(); }
                    catch { }
                    try { dosar.NR_SCA = ews.Cells[rowNumber, columnNames["Nr# SCA"]].Text.Trim(); }
                    catch { }
                    try { dosar.DATA_SCA = CommonFunctions.SwitchBackFormatedDate(ews.Cells[rowNumber, columnNames["Data SCA"]].Text.Trim()); }
                    catch { }
                    try { dosar.NR_POLITA_CASCO = ews.Cells[rowNumber, columnNames["Polita CASCO"]].Text.Trim(); }
                    catch { }
                    try { dosar.NR_POLITA_RCA = ews.Cells[rowNumber, columnNames["Polita RCA"]].Text.Trim(); }
                    catch { }
                    try { dosar.DATA_EVENIMENT = CommonFunctions.SwitchBackFormatedDate(ews.Cells[rowNumber, columnNames["Data CASCO"]].Text.Trim()); }
                    catch { }
                    try { dosar.VALOARE_DAUNA = Convert.ToDouble(ews.Cells[rowNumber, columnNames["Valoare dauna"]].Text.Trim()); }
                    catch { }
                    try { dosar.VALOARE_REGRES = Convert.ToDouble(ews.Cells[rowNumber, columnNames["Valoare Regres"]].Text.Trim()); }
                    catch { }
                    try { dosar.REZERVA_DAUNA = Convert.ToDouble(ews.Cells[rowNumber, columnNames["Valoare dauna"]].Text.Trim()); }
                    catch { }
                    try { dosar.SUMA_IBNR = Convert.ToDouble(ews.Cells[rowNumber, columnNames["Valoare dauna"]].Text.Trim()); }
                    catch { }

                    /*
                    try { dosar.OBSERVATII = dr["OBSERVATII"].ToString(); }
                    catch { }                    
                    try { dosar.DATA_AVIZARE = CommonFunctions.SwitchBackFormatedDate(dr["DATA_AVIZARE"].ToString()); }
                    catch { }
                    try { dosar.DATA_IESIRE_CASCO = CommonFunctions.SwitchBackFormatedDate(dr["DATA_IESIRE_CASCO"].ToString()); }
                    catch { }
                    try { dosar.DATA_INTRARE_RCA = CommonFunctions.SwitchBackFormatedDate(dr["DATA_INTRARE_RCA"].ToString()); }
                    catch { }
                    try { dosar.DATA_NOTIFICARE = CommonFunctions.SwitchBackFormatedDate(dr["DATA_NOTIFICARE"].ToString()); }
                    catch { }
                    try { dosar.NR_DOSAR_CASCO = dr["NR_DOSAR_CASCO"].ToString(); }
                    catch { }
                    try { dosar.NR_IESIRE_CASCO = dr["NR_IESIRE_CASCO"].ToString(); }
                    catch { }
                    try { dosar.NR_INTRARE_RCA = dr["NR_INTRARE_RCA"].ToString(); }
                    catch { }
                    try { dosar.VMD = Convert.ToDouble(dr["VMD"]); }
                    catch { }
                    */

                    // verificare daca exista dosarul in baza de date sau a mai fost importat si adaugare mesaj in errorlist ....
                    r = dosar.Validare();
                    if (!r.Status)
                    {
                        toReturn.AddResponse(r);
                    }
                    toReturnList.Add(new object[] { toReturn, dosar });
                }
                catch { }
            }
            return toReturnList.ToArray();
        }

        /// <summary>
        /// Metoda pentru importul Dosarelor in baza de date
        /// </summary>
        /// <param name="dosare">vector de SOCISA.DosareJson cu Dosarele de importat</param>
        /// <returns>vector de {SOCISA.response, SOCISA.DosareJson}</returns>
        public object[] ImportDosare(object[] responsesDosare, DateTime _date)
        {
            List<object[]> toReturnList = new List<object[]>();
            foreach (object[] responseDosar in responsesDosare)
            {
                Dosar dosar = (Dosar)responseDosar[1];
                response response = (response)responseDosar[0];
                response r = dosar.Insert();
                response.InsertedId = r.InsertedId;
                dosar.Log(response, _date);
                toReturnList.Add(new object[] { response, dosar });
            }
            return toReturnList.ToArray();
        }

        /// <summary>
        /// Metoda pentru importul Dosarelor cu erori in tabela temporara din baza de date
        /// </summary>
        /// <param name="dosare">vector de SOCISA.DosareJson cu Dosarele de importat</param>
        /// <returns>vector de {SOCISA.response, SOCISA.DosareJson}</returns>
        public object[] ImportDosareWithErrors(object[] responsesDosareWithErrors, DateTime _date)
        {
            List<object[]> toReturnList = new List<object[]>();
            foreach (object[] responseDosarWithErrors in responsesDosareWithErrors)
            {
                Dosar dosar = (Dosar)responseDosarWithErrors[1];
                response response = (response)responseDosarWithErrors[0];
                response r = dosar.InsertWithErrors();
                response.InsertedId = r.InsertedId;
                response.Status = false;
                dosar.Log(response, _date);
                toReturnList.Add(new object[] { response, dosar });
            }
            return toReturnList.ToArray();
        }

        /// <summary>
        /// Metoda pentru returnarea Dosarelor importate, din Log-ul salvat in baza de date
        /// </summary>
        /// <param name="data">Data la care s-a facut importul</param>
        /// <returns>vector de obiecte {SOCISA.response, SOCISA.DosareJson}</returns>
        public object[] GetDosareFromLog(DateTime data)
        {
            List<object[]> toReturnList = new List<object[]>();
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "IMPORT_LOGsp_GetDosare", new object[] { new MySqlParameter("_DATA_IMPORT", data) });
            IDataReader dr = da.ExecuteSelectQuery();
            while (dr.Read())
            {
                response r = new response();
                r.Status = Convert.ToBoolean(dr["STATUS"]);
                r.Message = dr["MESSAGE"].ToString();
                r.InsertedId = Convert.ToInt32(dr["INSERTED_ID"]);
                r.Error = JsonConvert.DeserializeObject<List<Error>>(dr["ERRORS"].ToString());

                Dosar dosar = r.Status ? new Dosar(authenticatedUserId, connectionString, Convert.ToInt32(r.InsertedId)) : new Dosar(authenticatedUserId, connectionString, Convert.ToInt32(r.InsertedId), true);
                toReturnList.Add(new object[] { r, dosar });
            }
            return toReturnList.ToArray();
        }

        public string GetImportDates()
        {
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_GetImportDates");
            IDataReader r = da.ExecuteSelectQuery();
            List<string> dates = new List<string>();
            try
            {
                while (r.Read())
                {
                    dates.Add(r["DATA_IMPORT"].ToString());
                }
                return Newtonsoft.Json.JsonConvert.SerializeObject(dates);
            }
            catch { return null; }
        }
    }
}