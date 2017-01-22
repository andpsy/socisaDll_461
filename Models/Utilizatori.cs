using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Security.Cryptography;

namespace SOCISA.Models
{
    /// <summary>
    /// Clasa care contine definitia obiectului ce mapeaza tabela cu Utilizatori din baza de date
    /// </summary>
    public class Utilizator
    {
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }
        public int? ID {get;set;}
        public string USER_NAME { get; set; }
        public string PASSWORD { get; set; }
        public string NUME_COMPLET { get; set; }
        public string DETALII { get; set; }
        public bool IS_ONLINE { get; set; }
        public string EMAIL { get; set; }
        public string IP { get; set; }
        public string MAC { get; set; }
        public int ID_TIP_UTILIZATOR { get; set; }
        public string DEPARTAMENT { get; set; }
        public DateTime? LAST_REFRESH { get; set; }

        /// <summary>
        /// Constructorul default
        /// </summary>
        public Utilizator() { }

        public Utilizator(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Utilizator(int _authenticatedUserId, string _connectionString, int _ID)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
            DbDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord utilizator = (IDataRecord)r;
                UtilizatorConstructor(utilizator);
                break;
            }
        }

        public Utilizator(int _authenticatedUserId, string _connectionString, IDataRecord utilizator)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            UtilizatorConstructor(utilizator);
        }

        /// <summary>
        /// Functie pentru popularea obiectului Utilizator, folosita de diferiti constructori
        /// </summary>
        /// <param name="utilizator">Inregistrare din DB cu informatiile Utilizatorului curent</param>
        public void UtilizatorConstructor(IDataRecord utilizator)
        {
            try { this.ID = Convert.ToInt32(utilizator["ID"]); }
            catch { }
            try { this.USER_NAME = utilizator["USER_NAME"].ToString(); }
            catch { }
            try { this.PASSWORD = utilizator["PASSWORD"].ToString(); }
            catch { }
            try { this.NUME_COMPLET = utilizator["NUME_COMPLET"].ToString(); }
            catch { }
            try { this.DETALII = utilizator["DETALII"].ToString(); }
            catch { }
            try { this.IS_ONLINE = Convert.ToBoolean( utilizator["DETALII"]); }
            catch { }
            try { this.EMAIL = utilizator["EMAIL"].ToString(); }
            catch { }
            try { this.IP = utilizator["IP"].ToString(); }
            catch { }
            try { this.MAC = utilizator["MAC"].ToString(); }
            catch { }
            try { this.ID_TIP_UTILIZATOR = Convert.ToInt32(utilizator["ID_TIP_UTILIZATOR"]); }
            catch { }
            try { this.DEPARTAMENT = utilizator["DEPARTAMENT"].ToString(); }
            catch { }
            try { this.LAST_REFRESH = Convert.ToDateTime(utilizator["LAST_REFRESH"]); }
            catch { }
        }

        /// <summary>
        /// Metoda pentru inserarea Utilizatorului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Insert()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "utilizatori");
            foreach (PropertyInfo prop in props)
            {
                if (col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                {
                    string propName = prop.Name;
                    string propType = prop.PropertyType.ToString();
                    object propValue = prop.GetValue(this, null);
                    propValue = propValue == null ? DBNull.Value : propValue;
                    if (propType != null)
                    {
                        if (propName.ToUpper() == "PASSWORD")
                        {
                            MD5 md5h = MD5.Create();
                            string md5p = CommonFunctions.GetMd5Hash(md5h, propValue.ToString());
                            _parameters.Add(new MySqlParameter("_PASSWORD", md5p));
                        }
                        else if (propName.ToUpper() != "ID") // il vom folosi doar la Edit!
                            _parameters.Add(new MySqlParameter(String.Format("_{0}", propName.ToUpper()), propValue));
                    }
                }
            }
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status) this.ID = toReturn.InsertedId;

            return toReturn;
        }

        /// <summary>
        /// Metoda pentru actualizarea Utilizatorului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Update()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "utilizatori");
            foreach (PropertyInfo prop in props)
            {
                if (col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                {
                    string propName = prop.Name;
                    string propType = prop.PropertyType.ToString();
                    object propValue = prop.GetValue(this, null);
                    propValue = propValue == null ? DBNull.Value : propValue;
                    if (propType != null)
                    {
                        if (propName.ToUpper() == "PASSWORD")
                        {
                            MD5 md5h = MD5.Create();
                            string md5p = CommonFunctions.GetMd5Hash(md5h, propValue.ToString());
                            _parameters.Add(new MySqlParameter("_PASSWORD", md5p));
                        }
                        else
                            _parameters.Add(new MySqlParameter(String.Format("_{0}", propName.ToUpper()), propValue));
                    }
                }
            }
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_update", _parameters.ToArray());
            toReturn = da.ExecuteUpdateQuery();
            if (toReturn.Status) this.ID = toReturn.InsertedId;

            return toReturn;
        }

        public response Update(string fieldValueCollection)
        {
            response r = ValidareColoane(fieldValueCollection);
            if (!r.Status)
            {
                return r;
            }
            else
            {
                Dictionary<string, string> changes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldValueCollection);
                foreach (string fieldName in changes.Keys)
                {
                    PropertyInfo[] props = this.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        //var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "actions");
                        //if (col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1 && fieldName.ToUpper() == prop.Name.ToUpper()) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                        if (fieldName.ToUpper() == prop.Name.ToUpper())
                        {
                            var tmpVal = prop.PropertyType.FullName.IndexOf("System.String") > -1 ? changes[fieldName] : Newtonsoft.Json.JsonConvert.DeserializeObject(changes[fieldName], prop.PropertyType);
                            prop.SetValue(this, tmpVal);
                            break;
                        }
                    }

                }
                return this.Update();
            }
        }

        /// <summary>
        /// Metoda pentru stergerea utilizatorului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Delete()
        {
            response toReturn = new response(false, "", null, new List<Error>());;
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru validarea Utilizatorului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Validare()
        {
            response toReturn = new response(true, "", null, new List<Error>());;
            Error err = new Error();
            if (this.USER_NAME == null || this.USER_NAME.Trim() == "")
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyUserName");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            if (this.PASSWORD == null || this.PASSWORD.Trim() == "")
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyUserPassword");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            return toReturn;
        }

        public response ValidareColoane(string fieldValueCollection)
        {
            return CommonFunctions.ValidareColoane(this, fieldValueCollection);
        }

        public string GenerateFilterFromJsonObject()
        {
            return Filtering.GenerateFilterFromJsonObject(this);
        }

        public bool HasChildrens(string tableName)
        {
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "utilizatori", tableName);
        }

        public bool HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "utilizatori", tableName, childrenId);
        }

        public object[] GetChildrens(string tableName)
        {
            return null;
        }

        public object GetChildren(string tableName, int childrenId)
        {
            return null;
        }

        /// <summary>
        /// Metoda pt. popularea dosarelor care sunt assignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.DosareJson</returns>
        /*
        public Dosar[] GetDosare()
        {
            try
            {
                DataAccess da = new DataAccess(CommandType.StoredProcedure, "UTILIZATORI_DOSAREsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DataTable utilizatoriDosare = da.ExecuteSelectQuery().Tables[0];
                //this.UtilizatoriDosare = SOCISA.UtilizatoriDosare.GetUtilizatoriDosare(utilizatoriDosare);

                DosareJson[] toReturn = new DosareJson[utilizatoriDosare.Rows.Count];
                for (int i = 0; i < utilizatoriDosare.Rows.Count; i++)
                {
                    toReturn[i] = new DosareJson(Convert.ToInt32(utilizatoriDosare.Rows[i]["ID_DOSAR"]));
                }
                return toReturn;
            }
            catch { return null; }
        }
        */
        /// <summary>
        /// Metoda pt. popularea relatiilor dintre utilizatori si dosarele asignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriDosareJson</returns>
        public UtilizatorDosar[] GetUtilizatoriDosare()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_DOSAREsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    UtilizatorDosar d = new UtilizatorDosar(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_DREPT"]));
                    aList.Add(d);
                }
                UtilizatorDosar[] toReturn = new UtilizatorDosar[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (UtilizatorDosar)aList[i];
                }
                return toReturn;
            }catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea drepturilor care sunt asignate utilizatorului curent
        /// </summary>
        /// <returns>SOCISA.DrepturiJson</returns>
        public Drept[] GetDrepturi()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_DREPTURIsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Drept d = new Drept(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_DREPT"]));
                    aList.Add(d);
                }
                Drept[] toReturn = new Drept[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Drept)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea relatiilor dintre utilizatori si drepturile asignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriDrepturiJson</returns>
        public UtilizatorDrept[] GetUtilizatoriDrepturi()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_DREPTURIsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    UtilizatorDrept d = new UtilizatorDrept(authenticatedUserId, connectionString, Convert.ToInt32(r["ID"]));
                    aList.Add(d);
                }
                UtilizatorDrept[] toReturn = new UtilizatorDrept[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (UtilizatorDrept)aList[i];
                }
                return toReturn;
            }catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea actiunilor care sunt assignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.ActionsJson</returns>
        public Action[] GetActions()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_ACTIONSsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Action a = new Action(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_ACTION"]));
                    aList.Add(a);
                }
                Action[] toReturn = new Action[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Action)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea relatiilor dintre utilizatori si actiunile asignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriActionsJson</returns>
        public UtilizatorAction[] GetUtilizatoriActions()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_ACTIONSsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    UtilizatorAction d = new UtilizatorAction(authenticatedUserId, connectionString, Convert.ToInt32(r["ID"]));
                    aList.Add(d);
                }
                UtilizatorAction[] toReturn = new UtilizatorAction[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (UtilizatorAction)aList[i];
                }
                return toReturn;
            }catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea setarilor care sunt asignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.SetariJson</returns>
        public Setare[] GetSetari()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SETARIsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Setare a = new Setare(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_SETARE"]));
                    aList.Add(a);
                }
                Setare[] toReturn = new Setare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Setare)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea relatiilor dintre utilizatori si setarile asignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriSetariJson</returns>
        public UtilizatorSetare[] GetUtilizatoriSetari()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SETARIsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    UtilizatorSetare a = new UtilizatorSetare(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_SETARE"]));
                    aList.Add(a);
                }
                UtilizatorSetare[] toReturn = new UtilizatorSetare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (UtilizatorSetare)aList[i];
                }
                return toReturn;
            }catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea societatilor care sunt asignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.SocietatiAsigurareJson</returns>
        public SocietateAsigurare[] GetSocietatiAsigurare()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SOCIETATIsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    SocietateAsigurare a = new SocietateAsigurare(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_SOCIETATE"]));
                    aList.Add(a);
                }
                SocietateAsigurare[] toReturn = new SocietateAsigurare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (SocietateAsigurare)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }
        
        /// <summary>
        /// Metoda pt. popularea relatiilor dintre utilizatori si societatile de asigurare asignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriSocietatiJson</returns>
        public UtilizatorSocietate[] GetUtilizatoriSocietati()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SOCIETATIsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    UtilizatorSocietate d = new UtilizatorSocietate(authenticatedUserId, connectionString, Convert.ToInt32(r["ID"]));
                    aList.Add(d);
                }
                UtilizatorSocietate[] toReturn = new UtilizatorSocietate[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (UtilizatorSocietate)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }

        public UtilizatorSocietateAdministrata[] GetUtilizatoriSocietatiAdministrate()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SOCIETATI_ADMINISTRATEsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    UtilizatorSocietateAdministrata d = new UtilizatorSocietateAdministrata(authenticatedUserId, connectionString, Convert.ToInt32(r["ID"]));
                    aList.Add(d);
                }
                UtilizatorSocietateAdministrata[] toReturn = new UtilizatorSocietateAdministrata[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (UtilizatorSocietateAdministrata)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea societatilor care sunt asignate utilizatorului curent
        /// </summary>
        /// <returns>vector de SOCISA.SocietatiAsigurareJson</returns>
        public SocietateAsigurare[] GetSocietatiAdministrate()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SOCIETATI_ADMINISTRATEsp_GetByIdUtilizator", new object[] { new MySqlParameter("_ID_UTILIZATOR", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    SocietateAsigurare a = new SocietateAsigurare(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_SOCIETATE"]));
                    aList.Add(a);
                }
                SocietateAsigurare[] toReturn = new SocietateAsigurare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (SocietateAsigurare)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea tipului utilizatorului curent
        /// </summary>
        /// <returns>SOCISA.NomenclatorJson</returns>
        public Nomenclator GetTipUtilizator()
        {
            try
            {
                Nomenclator tu = new Nomenclator(authenticatedUserId, connectionString, "tip_utilizatori", this.ID_TIP_UTILIZATOR);
                return tu;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. returnarea datei ultimului refresh pt. un utilizator (pt. refresh dashboard automat)
        /// </summary>
        /// <param name="_ID_UTILIZATOR">ID-ul unic al utilizatorului</param>
        /// <returns>System.DateTime sau null</returns>
        public DateTime? GetLastRefresh()
        {
            DateTime? toReturn = null;
            try
            {
                DataAccess da = new DataAccess( authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_GetLastRefresh", new object[] { new MySqlParameter("_ID_UTILIZATOR", Convert.ToInt32(ID)) });
                toReturn = Convert.ToDateTime(da.ExecuteScalarQuery());
            }
            catch { }
            return toReturn;
        }

        /// <summary>
        /// Metoda pt. setarea datei ultimului refresh al unui utilizator (pt. refresh dashboard automat)
        /// </summary>
        /// <param name="_ID_UTILIZATOR">ID-ul unic al utilizatorului</param>
        /// <param name="_LAST_REFRESH">Data ultimului refresh</param>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response SetLastRefresh(DateTime _LAST_REFRESH)
        {
            response toReturn = new response(false, "", null, new List<Error>()); ;
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_SetLastRefresh", new object[] { new MySqlParameter("_ID_UTILIZATOR", Convert.ToInt32(ID)), new MySqlParameter("_LAST_REFRESH", _LAST_REFRESH) });
                toReturn = da.ExecuteUpdateQuery();
            }
            catch { }
            return toReturn;
        }

        /// <summary>
        /// Functie pt. returnarea mesajelor noi, folosita pt. dashboard
        /// </summary>
        /// <param name="_ID_UTILIZATOR">Id-ul unic al utilizatorului</param>
        /// <returns>vector de perechi [SOCISA.NomenclatorJson, int]</returns>
        public object[] GetNewMessages()
        {
            object[] toReturn = null;
            try
            {
                List<object[]> dtList = new List<object[]>();
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_CountUnreadMessages", new object[] { new MySqlParameter("_ID_UTILIZATOR", Convert.ToInt32(ID)) });
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    dtList.Add(new object[] { new Nomenclator(authenticatedUserId, connectionString, "tip_mesaje", Convert.ToInt32(r["ID_TIP_MESAJ"])), Convert.ToInt32(r["MESAJE_NOI"]) });
                }
                toReturn = dtList.ToArray();
            }
            catch { }
            return toReturn;
        }
    }
}