using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace SOCISA.Models
{
    public class GenericDbObject
    {
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }
        public string TableName;
        int? ID;


        public GenericDbObject() { }

        public GenericDbObject(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public virtual response Insert()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();

            foreach (PropertyInfo prop in props)
            {
                var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, TableName);
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, TableName.ToUpper() + "sp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status) this.ID = toReturn.InsertedId;

            return toReturn;
        }

        public virtual response Update()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            foreach (PropertyInfo prop in props)
            {
                var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, TableName);
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, TableName.ToUpper() + "sp_update", _parameters.ToArray());
            toReturn = da.ExecuteUpdateQuery();

            return toReturn;
        }

        public virtual response Update(string fieldValueCollection)
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
                        //var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, TableName);
                        //if (col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1 && fieldName.ToUpper() == prop.Name.ToUpper()) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                        if (fieldName.ToUpper() == prop.Name.ToUpper())
                        {
                            var tmpVal = prop.PropertyType.FullName.IndexOf("System.String") > -1 ? changes[fieldName] : prop.PropertyType.FullName.IndexOf("System.DateTime") > -1 ? Convert.ToDateTime(changes[fieldName]) : Newtonsoft.Json.JsonConvert.DeserializeObject(changes[fieldName], prop.PropertyType);
                            prop.SetValue(this, tmpVal);
                            break;
                        }
                    }

                }
                return this.Update();
            }
        }

        public virtual response Delete()
        {
            response toReturn = new response(false, "", null, null, new List<Error>()); ;
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, TableName.ToUpper() + "sp_soft_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        public virtual response Validare()
        {
            response toReturn = new response(true, "", null, null, new List<Error>()); ;
            return toReturn;
        }

        public virtual response ValidareColoane(string fieldValueCollection)
        {
            response toReturn = new response(true, null, null, null, new List<Error>());
            try
            {
                Dictionary<string, string> changes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldValueCollection);
                foreach (string fieldName in changes.Keys)
                {
                    bool gasit = false;
                    PropertyInfo[] props = this.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (fieldName.ToUpper() == prop.Name.ToUpper())
                        {
                            gasit = true;
                            break;
                        }
                    }
                    if (!gasit)
                    {
                        Error err = ErrorParser.ErrorMessage("campInexistentInTabela");
                        return new response(false, err.ERROR_MESSAGE, null, null, new List<Error>() { err });
                    }
                }
            }
            catch
            {
                Error err = ErrorParser.ErrorMessage("cannotConvertStringToTableColumns");
                return new response(false, err.ERROR_MESSAGE, null, null, new List<Error>() { err });
            }
            return toReturn;
        }

        public virtual string GenerateFilterFromJsonObject()
        {
            string toReturn = "";

            PropertyInfo[] props = this.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string propName = prop.Name;
                string propType = prop.PropertyType.ToString();
                object propValue = prop.GetValue(this, null);
                propValue = propValue == null ? DBNull.Value : propValue;
                if (propType != null && propValue != null && propValue != System.DBNull.Value)
                {
                    try
                    {
                        //if (propName.ToUpper().IndexOf("ID") != 0) // nu cautam si dupa ID-uri
                        {
                            switch (propType)
                            {
                                case "System.DateTime":
                                    toReturn += String.Format("{2}{0} >= '{1}'", propName, propValue, (toReturn == "" ? "" : " AND "));
                                    break;
                                default:
                                    toReturn += String.Format("{2}{0} like '{1}%'", propName, propValue, (toReturn == "" ? "" : " AND "));
                                    break;
                            }
                        }
                    }
                    catch { }
                }
            }
            return toReturn;
        }

        public virtual bool HasChildrens(string tableName)
        {
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TABLEsp_GetReferences", new object[] { new MySqlParameter("_PARENT_TABLE", "actions"), new MySqlParameter("_CHILD_TABLE", tableName) });
            MySqlDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                if (r["REFERENCED_TABLE_NAME"].ToString().ToUpper() == tableName.ToUpper())
                {
                    PropertyInfo[] props = this.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.Name.ToUpper() == r["COLUMN_NAME"].ToString().ToUpper())
                        {
                            da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "CHILDRENSsp_Get", new object[] { new MySqlParameter("_PRIMARY_KEY_VALUE", prop.GetValue(this)), new MySqlParameter("_EXTERNAL_ID", r["REFERENCED_COLUMN_NAME"].ToString()), new MySqlParameter("_EXTERNAL_TABLE", r["REFERENCED_TABLE_NAME"].ToString()) });
                            object counter = da.ExecuteScalarQuery().Result;
                            try
                            {
                                if (Convert.ToInt32(counter) > 0)
                                    return true;
                            }
                            catch { }
                            break;
                        }
                    }
                }
                else
                {
                    da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "CHILDRENSsp_Get", new object[] { new MySqlParameter("_PRIMARY_KEY_VALUE", this.ID), new MySqlParameter("_EXTERNAL_ID", r["COLUMN_NAME"].ToString()), new MySqlParameter("_EXTERNAL_TABLE", r["TABLE_NAME"].ToString()) });
                    object counter = da.ExecuteScalarQuery().Result;
                    try
                    {
                        return Convert.ToInt32(counter) > 0;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            r.Close(); r.Dispose();
            return false;
        }

        public virtual bool HasChildren(string tableName, int childrenId)
        {
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TABLEsp_GetReferences", new object[] { new MySqlParameter("_PARENT_TABLE", "actions"), new MySqlParameter("_CHILD_TABLE", tableName) });
            MySqlDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                if (r["REFERENCED_TABLE_NAME"].ToString().ToUpper() == tableName.ToUpper())
                {
                    PropertyInfo[] props = this.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.Name.ToUpper() == r["COLUMN_NAME"].ToString().ToUpper())
                        {
                            da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "CHILDRENsp_Get", new object[] { new MySqlParameter("_PRIMARY_KEY_VALUE", prop.GetValue(this)), new MySqlParameter("_EXTERNAL_ID", r["REFERENCED_COLUMN_NAME"].ToString()), new MySqlParameter("_EXTERNAL_TABLE", r["REFERENCED_TABLE_NAME"].ToString()), new MySqlParameter("_CHILDREN_ID_FIELD", "1"), new MySqlParameter("_CHILDREN_ID_VALUE", "1") });
                            object counter = da.ExecuteScalarQuery().Result;
                            try
                            {
                                if (Convert.ToInt32(counter) > 0)
                                    return true;
                            }
                            catch { }
                            break;
                        }
                    }
                }
                else
                {
                    da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TABLEsp_GetReferences", new object[] { new MySqlParameter("_PARENT_TABLE", r["TABLE_NAME"].ToString()), new MySqlParameter("_CHILD_TABLE", tableName) });
                    MySqlDataReader rc = da.ExecuteSelectQuery();
                    while (rc.Read())
                    {
                        da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "CHILDRENsp_Get", new object[] { new MySqlParameter("_PRIMARY_KEY_VALUE", this.ID), new MySqlParameter("_EXTERNAL_ID", r["REFERENCED_COLUMN_NAME"].ToString()), new MySqlParameter("_EXTERNAL_TABLE", r["REFERENCED_TABLE_NAME"].ToString()), new MySqlParameter("_CHILDREN_ID_FIELD", rc["COLUMN_NAME"].ToString()), new MySqlParameter("_CHILDREN_ID_VALUE", childrenId) });
                        object counter = da.ExecuteScalarQuery().Result;
                        try
                        {
                            return Convert.ToInt32(counter) > 0;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    rc.Close(); rc.Dispose();
                }
            }
            r.Close(); r.Dispose();
            return false;
        }

        public virtual object[] GetChildrens(string tableName)
        {
            return null;
        }

        public virtual object GetChildren(string tableName, int childrenId)
        {
            return null; 
        }
    }
}
