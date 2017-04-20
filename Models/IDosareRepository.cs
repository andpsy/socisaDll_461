using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SOCISA.Models
{
    public interface IDosareRepository
    {
        response GetAll();
        response CountAll();
        response CountFromLastLogin();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response GetFiltered(string _json);
        response GetFiltered(JObject _json);
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

        response GetMesaje(Dosar item);
        response GetUtilizatori(Dosar item);
        response GetSocietateCasco(Dosar item);
        response GetSocietateRca(Dosar item);
        response GetAutoCasco(Dosar item);
        response GetAutoRca(Dosar item);
        response GetAsiguratCasco(Dosar item);
        response GetAsiguratRca(Dosar item);
        response GetIntervenient(Dosar item);
        response GetDocumente(Dosar item);
        response GetProcese(Dosar item);
        response GetTipDosar(Dosar item);
        response GetInvolvedParties(Dosar item);

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
        response GetDataUltimeiModificari(Dosar item);
        response GetDosareFromExcel(string sheet, string fileName);
        response GetDosareFromExcel(JObject _json);
        response ImportDosare(response responsesDosare, DateTime _date);
        response ImportDosareDirect(string sheet, string fileName);
        response ImportDosareDirect(JObject _json);
        response ImportDosareWithErrors(response responsesDosareWithErrors, DateTime _date);
        response GetDosareFromLog(DateTime data);
        response GetImportDates();
    }

    public class DosareRepository : IDosareRepository
    {
        private const string _TEMPLATE_CERERE_DESPAGUBIRE1 = "Cerere_despagubire_t1.pdf";
        private const string _TEMPLATE_CERERE_DESPAGUBIRE2 = "Cerere_despagubire_t2.pdf";
        private const string _TEMPLATE_CERERE_DESPAGUBIRE3 = "Cerere_t2.pdf";
        private const string _TEMPLATE_CERERE_DESPAGUBIRE4 = "Cerere_t1.pdf";
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
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Dosar a = new Dosar(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Dosar[] toReturn = new Dosar[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Dosar)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public response CountAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_count");
                object count = da.ExecuteScalarQuery().Result;
                if(count == null)
                    return new response(true, "0", 0, null, null);
                return new response(true, count.ToString(), Convert.ToInt32(count), null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public response CountFromLastLogin()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_CountFromLastLogin");
                object count = da.ExecuteScalarQuery().Result;
                if (count == null)
                    return new response(true, "0", 0, null, null);
                return new response(true, count.ToString(), Convert.ToInt32(count), null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public response GetFiltered(string _json)
        {
            JObject jObj = JObject.Parse(_json);
            return GetFiltered(jObj);
        }

        public response GetFiltered(JObject _json)
        {
            try
            {                
                Filter f = new Filter();
                foreach(var t in _json)
                {
                    JToken j = t.Value;
                    switch (t.Key.ToLower())
                    {
                        case "sort":
                            //f.Sort = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j, CommonFunctions.JsonSerializerSettings);
                            f.Sort = CommonFunctions.IsNullOrEmpty(j) ? null : j.ToString();
                            break;
                        case "order":
                            //f.Order = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j, CommonFunctions.JsonSerializerSettings);
                            f.Order = CommonFunctions.IsNullOrEmpty(j) ? null : j.ToString();
                            break;
                        case "filter":
                            f.Filtru = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j, CommonFunctions.JsonSerializerSettings);
                            break;
                        case "limit":
                            //f.Limit = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j, CommonFunctions.JsonSerializerSettings);
                            f.Limit = CommonFunctions.IsNullOrEmpty(j) ? null : j.ToString();
                            break;
                    }
                }
                return GetFiltered(f.Sort, f.Order, f.Filtru, f.Limit);
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
                    _filter = newFilter == null || newFilter.Trim() == "" ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Dosar a = new Dosar(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Dosar[] toReturn = new Dosar[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Dosar)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                Dosar item = new Dosar(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item, CommonFunctions.JsonSerializerSettings), item, null, null); ;
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
            //Dosar item = JsonConvert.DeserializeObject<Dosar>(Find(id).Message);
            Dosar item = (Dosar)(Find(id).Result);
            return item.Update(fieldValueCollection);
        }

        public response Update(string fieldValueCollection)
        {
            Dosar tmpItem = JsonConvert.DeserializeObject<Dosar>(fieldValueCollection, CommonFunctions.JsonDeserializerSettings); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<Dosar>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((Dosar)(Find(Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
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
            response obj = Find(_id);
            //return JsonConvert.DeserializeObject<Dosar>(obj.Message).Delete();
            return ((Dosar)obj.Result).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Dosar>(obj.Message).HasChildrens(tableName);
            return ((Dosar)obj.Result).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Dosar>(obj.Message).HasChildren(tableName, childrenId);
            return ((Dosar)obj.Result).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Dosar>(obj.Message).GetChildrens(tableName);
            return ((Dosar)obj.Result).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Dosar>(obj.Message).GetChildren(tableName, childrenId);
            return ((Dosar)obj.Result).GetChildren(tableName, childrenId);
        }

        public response GetMesaje(Dosar item)
        {
            return item.GetMesaje();
        }
        public response GetUtilizatori(Dosar item)
        {
            return item.GetUtilizatori();
        }
        public response GetSocietateCasco(Dosar item)
        {
            return item.GetSocietateCasco();
        }
        public response GetSocietateRca(Dosar item)
        {
            return item.GetSocietateRca();
        }
        public response GetAutoCasco(Dosar item)
        {
            return item.GetAutoCasco();
        }
        public response GetAutoRca(Dosar item)
        {
            return item.GetAutoRca();
        }
        public response GetAsiguratCasco(Dosar item)
        {
            return item.GetAsiguratCasco();
        }
        public response GetAsiguratRca(Dosar item)
        {
            return item.GetAsiguratRca();
        }
        public response GetIntervenient(Dosar item)
        {
            return item.GetIntervenient();
        }
        public response GetDocumente(Dosar item)
        {
            return item.GetDocumente();
        }

        public response GetProcese(Dosar item)
        {
            return item.GetProcese();
        }

        public response GetTipDosar(Dosar item)
        {
            return item.GetTipDosar();
        }

        public response ExportDocumenteDosarToPdf(Dosar item)
        {
            return item.ExportDocumenteDosarToPdf();
        }
        public response ExportDocumenteDosarToPdf(int _id)
        {
            //return JsonConvert.DeserializeObject<Dosar>(Find(_id).Message).ExportDocumenteDosarToPdf();
            return ((Dosar)(Find(_id).Result)).ExportDocumenteDosarToPdf();
        }
        public response ExportDosarToPdf(string templateFileName, Dosar item)
        {
            return item.ExportDosarToPdf(templateFileName);
        }
        public response ExportDosarToPdf(string templateFileName, int _id)
        {
            //return JsonConvert.DeserializeObject<Dosar>(Find(_id).Message).ExportDosarToPdf(templateFileName);
            return ((Dosar)(Find(_id).Result)).ExportDosarToPdf(templateFileName);
        }
        public response ExportDosarCompletToPdf(string templateFileName, Dosar item)
        {
            return item.ExportDosarCompletToPdf(templateFileName);
        }
        public response ExportDosarCompletToPdf(Dosar item)
        {
            DocumentScanat[] tmp = (DocumentScanat[])item.GetDocumente().Result;
            SocietateAsigurare srca = (SocietateAsigurare)item.GetSocietateRca().Result;
            bool faliment = false;
            if (srca.DENUMIRE.ToUpper().IndexOf("ASTRA") > -1 || srca.DENUMIRE.ToUpper().IndexOf("CARPATICA") > -1)
            {
                faliment = true;
            }
            bool constatare_amiabila = false;
            foreach(DocumentScanat ds in tmp)
            {
                Nomenclator tip_doc = new Nomenclator(authenticatedUserId, connectionString, "TIP_DOCUMENT", Convert.ToInt32(ds.ID_TIP_DOCUMENT));
                if (tip_doc.DENUMIRE == "CONSTATARE AMIABILA")
                {
                    constatare_amiabila = true;
                    break;
                }
            }
            string template = "";
            if (faliment && constatare_amiabila) template = _TEMPLATE_CERERE_DESPAGUBIRE3;
            if (faliment && !constatare_amiabila) template = _TEMPLATE_CERERE_DESPAGUBIRE4;
            if (!faliment && constatare_amiabila) template = _TEMPLATE_CERERE_DESPAGUBIRE1;
            if (!faliment && !constatare_amiabila) template = _TEMPLATE_CERERE_DESPAGUBIRE2;

            return item.ExportDosarCompletToPdf(template);
        }
        public response ExportDosarCompletToPdf(string templateFileName, int _id)
        {
            //return JsonConvert.DeserializeObject<Dosar>(Find(_id).Message).ExportDosarCompletToPdf(templateFileName);
            return ((Dosar)(Find(_id).Result)).ExportDosarCompletToPdf(templateFileName);
        }
        public response ExportDosarCompletToPdf(int _id)
        {
            Dosar d = (Dosar)(Find(_id).Result);
            SocietateAsigurare srca = (SocietateAsigurare)d.GetSocietateRca().Result;
            bool faliment = false;
            if (srca.DENUMIRE.ToUpper().IndexOf("ASTRA") > -1 || srca.DENUMIRE.ToUpper().IndexOf("CARPATICA") > -1)
            {
                faliment = true;
            }
            DocumentScanat[] tmp = (DocumentScanat[])d.GetDocumente().Result;
            bool constatare_amiabila = false;
            foreach (DocumentScanat ds in tmp)
            {
                Nomenclator tip_doc = new Nomenclator(authenticatedUserId, connectionString, "TIP_DOCUMENT", Convert.ToInt32(ds.ID_TIP_DOCUMENT));
                if (tip_doc.DENUMIRE == "CONSTATARE AMIABILA")
                {
                    constatare_amiabila = true;
                    break;
                }
            }
            //return JsonConvert.DeserializeObject<Dosar>(Find(_id).Message).ExportDosarCompletToPdf(_TEMPLATE_CERERE_DESPAGUBIRE);
            string template = "";
            if (faliment && constatare_amiabila) template = _TEMPLATE_CERERE_DESPAGUBIRE3;
            if (faliment && !constatare_amiabila) template = _TEMPLATE_CERERE_DESPAGUBIRE4;
            if (!faliment && constatare_amiabila) template = _TEMPLATE_CERERE_DESPAGUBIRE1;
            if (!faliment && !constatare_amiabila) template = _TEMPLATE_CERERE_DESPAGUBIRE2;

            return d.ExportDosarCompletToPdf(template);
        }
        public void Import(Dosar item)
        {
            item.Import();
        }
        public response SetDataUltimeiModificari(DateTime data, Dosar item)
        {
            return item.SetDataUltimeiModificari(data);
        }
        public response GetDataUltimeiModificari(Dosar item)
        {
            return item.GetDataUltimeiModificari();
        }
        public response GetInvolvedParties(Dosar item)
        {
            return item.GetInvolvedParties();
        }

        public response ImportDosareDirect(string sheet, string fileName)
        {
            response r = GetDosareFromExcel(sheet, fileName);
            return ImportAll(r, DateTime.Now);
        }

        public response ImportDosareDirect (JObject _json)
        {
            response r = GetDosareFromExcel(_json);
            return ImportAll(r, DateTime.Now);
        }

        /// <summary>
        /// Metoda pentru incarcarea vectorului de Dosare din fisierul Excel de importat
        /// </summary>
        /// <param name="sheet">Denumirea Sheet-ului din fisierul Excel in care se gasesc Dosarele de importat</param>
        /// <param name="fileName">Denumirea completa a fisierului cu Dosarele de importat</param>
        /// <returns>vector de {SOCISA.response, SOCISA.DosareJson}</returns>
        public response GetDosareFromExcel(string sheet, string fileName)
        {
            try
            {                
                FileInfo fi = new FileInfo(File.Exists(fileName) ? fileName : Path.Combine(CommonFunctions.GetImportsFolder(), fileName));
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
                                asigCasco = asigCasco.ID == null ? null : asigCasco;
                            }
                            catch { asigCasco = null; }
                            if (asigCasco == null && columnNames.ContainsKey("Asigurat CASCO"))
                            {
                                asigCasco = new Asigurat(authenticatedUserId, connectionString);
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
                                asigRca = asigRca.ID == null ? null : asigRca;
                            }
                            catch { asigRca = null; }
                            if (asigRca == null && columnNames.ContainsKey("Asigurat RCA"))
                            {
                                asigRca = new Asigurat(authenticatedUserId, connectionString);
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
                                sCasco = sCasco.ID == null ? null : sCasco;
                            }
                            catch { sCasco = null; }
                            if (sCasco == null && columnNames.ContainsKey("Asigurator CASCO"))
                            {
                                sCasco = new SocietateAsigurare(authenticatedUserId, connectionString);
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
                                sRca = sRca.ID == null ? null : sRca;
                            }
                            catch { sRca = null; }
                            if (sRca == null && columnNames.ContainsKey("Asigurator RCA"))
                            {
                                sRca = new SocietateAsigurare(authenticatedUserId, connectionString);
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
                                autoCasco = autoCasco.ID == null ? null : autoCasco;
                            }
                            catch { autoCasco = null; }
                            if (autoCasco == null && columnNames.ContainsKey("Auto CASCO"))
                            {
                                autoCasco = new Auto(authenticatedUserId, connectionString);
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
                                autoRca = autoRca.ID == null ? null : autoRca;
                            }
                            catch { autoRca = null; }
                            if (autoRca == null && columnNames.ContainsKey("Auto RCA"))
                            {
                                autoRca = new Auto(authenticatedUserId, connectionString);
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
                                intervenient = intervenient.ID == null ? null : intervenient;
                            }
                            catch { intervenient = null; }
                            if (intervenient == null && columnNames.ContainsKey("Intervenient"))
                            {
                                intervenient = new Intervenient(authenticatedUserId, connectionString);
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

                        try { dosar.NR_DOSAR_CASCO = ews.Cells[rowNumber, columnNames["Nr. CASCO"]].Text.Trim(); }
                        catch { }
                        try { dosar.NR_SCA = ews.Cells[rowNumber, columnNames["Nr. SCA"]].Text.Trim(); }
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
                return new response(true, JsonConvert.SerializeObject(toReturnList.ToArray(), CommonFunctions.JsonSerializerSettings), toReturnList.ToArray(), null, null);
            }
            catch (Exception exp)
            {
                return new response(false, exp.Message, null, null, new List<Error>() { new Error(exp) });
            }
        }

        public response GetDosareFromExcel(JObject _json)
        {
            try
            {
                string sheet = null, fileName = null;
                foreach (var t in _json)
                {
                    JToken j = t.Value;
                    switch (t.Key.ToLower())
                    {
                        case "sheet":
                            sheet = CommonFunctions.IsNullOrEmpty(j) ? null : j.ToString();
                            break;
                        case "filename":
                            fileName = CommonFunctions.IsNullOrEmpty(j) ? null : j.ToString();
                            break;
                    }
                }
                return GetDosareFromExcel(sheet, fileName);
            }
            catch (Exception exp)
            {
                return new response(false, exp.Message, null, null, new List<Error>() { new Error(exp) });
            }
        }

        public response ImportAll(response responsesDosare, DateTime _date)
        {
            try
            {
                List<object[]> toReturnList = new List<object[]>();
                foreach (object[] responseDosar in (object[])responsesDosare.Result)
                {
                    Dosar dosar = (Dosar)responseDosar[1];
                    response response = (response)responseDosar[0];
                    response r = new response();
                    if (response.Status)
                    {
                        r = dosar.Insert();
                    }
                    else
                    {
                        r = dosar.InsertWithErrors();
                        response.Status = false;
                    }
                    response.InsertedId = r.InsertedId;
                    dosar.Log(response, _date);
                    toReturnList.Add(new object[] { response, dosar });
                }
                return new response(true, JsonConvert.SerializeObject(toReturnList.ToArray(), CommonFunctions.JsonSerializerSettings), toReturnList.ToArray(), null, null);
            }
            catch (Exception exp)
            {
                return new response(false, exp.Message, null, null, new List<Error>() { new Error(exp) });
            }
        }

        /// <summary>
        /// Metoda pentru importul Dosarelor in baza de date
        /// </summary>
        /// <param name="dosare">vector de SOCISA.DosareJson cu Dosarele de importat</param>
        /// <returns>vector de {SOCISA.response, SOCISA.DosareJson}</returns>
        public response ImportDosare(response responsesDosare, DateTime _date)
        {
            try
            {
                List<object[]> toReturnList = new List<object[]>();
                foreach (object[] responseDosar in (object[])responsesDosare.Result)
                {
                    Dosar dosar = (Dosar)responseDosar[1];
                    response response = (response)responseDosar[0];
                    response r = dosar.Insert();
                    response.InsertedId = r.InsertedId;
                    dosar.Log(response, _date);
                    toReturnList.Add(new object[] { response, dosar });
                }
                return new response(true, JsonConvert.SerializeObject(toReturnList.ToArray(), CommonFunctions.JsonSerializerSettings), toReturnList.ToArray(), null, null);
            }catch(Exception exp)
            {
                return new response(false, exp.Message, null, null, new List<Error>() { new Error(exp) });
            }
        }

        /// <summary>
        /// Metoda pentru importul Dosarelor cu erori in tabela temporara din baza de date
        /// </summary>
        /// <param name="dosare">vector de SOCISA.DosareJson cu Dosarele de importat</param>
        /// <returns>vector de {SOCISA.response, SOCISA.DosareJson}</returns>
        public response ImportDosareWithErrors(response responsesDosareWithErrors, DateTime _date)
        {
            try
            {
                List<object[]> toReturnList = new List<object[]>();
                foreach (object[] responseDosarWithErrors in (object[])responsesDosareWithErrors.Result)
                {
                    Dosar dosar = (Dosar)responseDosarWithErrors[1];
                    response response = (response)responseDosarWithErrors[0];
                    response r = dosar.InsertWithErrors();
                    response.InsertedId = r.InsertedId;
                    response.Status = false;
                    dosar.Log(response, _date);
                    toReturnList.Add(new object[] { response, dosar });
                }
                return new response(true, JsonConvert.SerializeObject(toReturnList.ToArray(), CommonFunctions.JsonSerializerSettings), toReturnList.ToArray(), null, null);
            }
            catch (Exception exp)
            {
                return new response(false, exp.Message, null, null, new List<Error>() { new Error(exp) });
            }
        }

        /// <summary>
        /// Metoda pentru returnarea Dosarelor importate, din Log-ul salvat in baza de date
        /// </summary>
        /// <param name="data">Data la care s-a facut importul</param>
        /// <returns>vector de obiecte {SOCISA.response, SOCISA.DosareJson}</returns>
        public response GetDosareFromLog(DateTime data)
        {
            try
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
                    r.Error = JsonConvert.DeserializeObject<List<Error>>(dr["ERRORS"].ToString(), CommonFunctions.JsonDeserializerSettings);

                    Dosar dosar = r.Status ? new Dosar(authenticatedUserId, connectionString, Convert.ToInt32(r.InsertedId)) : new Dosar(authenticatedUserId, connectionString, Convert.ToInt32(r.InsertedId), true);
                    toReturnList.Add(new object[] { r, dosar });
                }
                dr.Close(); dr.Dispose();
                return new response(true, JsonConvert.SerializeObject(toReturnList.ToArray(), CommonFunctions.JsonSerializerSettings), toReturnList.ToArray(), null, null);
            }
            catch (Exception exp)
            {
                return new response(false, exp.Message, null, null, new List<Error>() { new Error(exp) });
            }
        }

        public response GetImportDates()
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
                r.Close(); r.Dispose();
                return new response(true, JsonConvert.SerializeObject(dates, CommonFunctions.JsonSerializerSettings), dates, null, null);
            }
            catch (Exception exp)
            {
                return new response(false, exp.Message, null, null, new List<Error>() { new Error(exp) });
            }
        }
    }
}