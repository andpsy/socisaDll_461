using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace SOCISA.Models
{
    public class UtilizatorSetare
    {
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }

        public int? ID { get; set; }
        public int ID_UTILIZATOR { get; set; }
        public int ID_SETARE { get; set; }
        public string VALOARE { get; set; }

        public UtilizatorSetare() { }

        public UtilizatorSetare(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public UtilizatorSetare(int _authenticatedUserId, string _connectionString, int _ID)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_ACTIONSsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
            DbDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                UtilizatorSetareConstructor(item);
                break;
            }
        }

        public UtilizatorSetare(int _authenticatedUserId, string _connectionString, IDataRecord item)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            UtilizatorSetareConstructor(item);
        }

        public void UtilizatorSetareConstructor(IDataRecord item)
        {
            try { this.ID = Convert.ToInt32(item["ID"]); }
            catch { }
            try { this.ID_UTILIZATOR = Convert.ToInt32(item["ID_UTILIZATOR"]); }
            catch { }
            try { this.ID_SETARE = Convert.ToInt32(item["ID_SETARE"]); }
            catch { }
            try { this.VALOARE = item["VALOARE"].ToString(); }
            catch { }
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

            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "utilizatori_setari");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SETARIsp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status) this.ID = toReturn.InsertedId;

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
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "utilizatori_setari");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SETARIsp_update", _parameters.ToArray());
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
            DataAccess da = new DataAccess();
            if (this.ID != null)
            {
                _parameters.Add(new MySqlParameter("_ID", this.ID));
                da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SETARIsp_delete", _parameters.ToArray());
            }
            else
            {
                _parameters.Add(new MySqlParameter("_ID_UTILIZATOR", this.ID_UTILIZATOR));
                _parameters.Add(new MySqlParameter("_ID_SETARE", this.ID_SETARE));
                da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SETARIsp_deleteByFields", _parameters.ToArray());
            }
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        public response Validare()
        {
            response toReturn = new response(true, "", null, null, new List<Error>());
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
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "utilizatori_setari", tableName);
        }

        public response HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "utilizatori_setari", tableName, childrenId);
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