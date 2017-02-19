using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace SOCISA.Models
{
    public class Sentinta
    {
        const string _TABLE_NAME = "sentinte";
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }

        public int? ID { get; set; }
        public string NR_SENTINTA { get; set; }
        public DateTime DATA_SENTINTA { get; set; }
        public DateTime DATA_COMUNICARE { get; set; }
        //public int? ID_SOLUTIE {get;set;}
        public string SOLUTIE { get; set; }
        /* public SolutiiJson Solutie { get; set; } */

        public Sentinta() { }

        public Sentinta(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Sentinta(int _authenticatedUserId, string _connectionString, int _ID)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SENTINTEsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
            MySqlDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                SentintaConstructor(item);
                break;
            }
            r.Close(); r.Dispose();
        }

        public Sentinta(int _authenticatedUserId, string _connectionString, IDataRecord item)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            SentintaConstructor(item);
        }

        public void SentintaConstructor(IDataRecord sentinta)
        {
            try { this.ID = Convert.ToInt32(sentinta["ID"]); }
            catch { }
            try { this.NR_SENTINTA = sentinta["NR_SENTINTA"].ToString(); }
            catch { }
            try { this.DATA_SENTINTA = Convert.ToDateTime(sentinta["DATA_SENTINTA"]); }
            catch { }
            try { this.DATA_COMUNICARE = Convert.ToDateTime(sentinta["DATA_COMUNICARE"]); }
            catch { }
            try { this.SOLUTIE = sentinta["SOLUTIE"].ToString(); }
            catch { }
            /*
            try { this.ID_SOLUTIE = Convert.ToInt32(stadiu["ID_SOLUTIE"]); }
            catch { }
            */
            /*
            try { this.Solutie = GetSolutie(); }
            catch { }
            */
        }

        public response Insert()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();

            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "sentinte");
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
                        if (propName.ToUpper() != "ID") // il vom folosi doar la Edit!
                            _parameters.Add(new MySqlParameter(String.Format("_{0}", propName.ToUpper()), propValue));
                    }
                }
            }
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SENTINTEsp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status) this.ID = toReturn.InsertedId;

            return toReturn;
        }

        public response Insert(int _ID_DOSAR_STADIU)
        {
            response toReturn = Insert();
            if (toReturn.Status)
            {
                this.ID = toReturn.InsertedId;
                DosarStadiuSentinta dssj = new DosarStadiuSentinta();
                dssj.ID_DOSAR_STADIU = _ID_DOSAR_STADIU;
                dssj.ID_SENTINTA = Convert.ToInt32(this.ID);
                dssj.Insert();
            }
            /*
            if (this.Solutie != null)
            {
                response toReturnS = Solutie.Insert();
                if (toReturnS.Status && toReturnS.InsertedId != null)
                {
                    this.ID_SOLUTIE = toReturnS.InsertedId;
                    //this.UpdateSentinta();
                }
            }
            */

            return toReturn;
        }

        public response Update()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "sentinte");
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
                        _parameters.Add(new MySqlParameter(String.Format("_{0}", propName.ToUpper()), propValue));
                    }
                }
            }
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SENTINTEsp_update", _parameters.ToArray());
            toReturn = da.ExecuteUpdateQuery();

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

        public response Delete()
        {
            response toReturn = new response(false, "", null, null, new List<Error>()); ;
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SENTINTEsp_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        public response Delete(int _ID_DOSAR_STADIU)
        {
            response toReturn = new response(true, "", null, null, new List<Error>()); ;

            DosarStadiuSentinta dssj = new DosarStadiuSentinta();
            dssj.ID_DOSAR_STADIU = _ID_DOSAR_STADIU;
            dssj.ID_SENTINTA = Convert.ToInt32(this.ID);
            toReturn = dssj.Delete();

            if (toReturn.Status)
            {
                toReturn = Delete();
                /* this.Solutie.Delete(); */
            }
            return toReturn;
        }

        public response Validare()
        {
            response toReturn = new response(true, "", null, null, new List<Error>()); ;
            Error err = new Error();
            if (this.NR_SENTINTA == null || this.NR_SENTINTA.Trim() == "")
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyNrSentinta");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            if (this.DATA_SENTINTA == null || this.DATA_SENTINTA == new DateTime())
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyDataSentinta");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            /*
            if (this.ID_SOLUTIE == null || this.ID_SOLUTIE <= 0)
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptySolutieSentinta");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            */
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

        public response HasChildrens(string tableName)
        {
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "sentinte", tableName);
        }

        public response HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "sentinte", tableName, childrenId);
        }

        public response GetChildrens(string tableName)
        {
            return CommonFunctions.GetChildrens(this, tableName);
        }

        public response GetChildren(string tableName, int childrenId)
        {
            return CommonFunctions.GetChildren(this, tableName, childrenId);
        }
    }
}