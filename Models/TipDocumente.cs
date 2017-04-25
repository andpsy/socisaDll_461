using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SOCISA.Models
{
    public class TipDocument
    {
        const string _TABLE_NAME = "tip_document";
        int authenticatedUserId { get; set; }
        string connectionString { get; set; }

        [Key]
        public int? ID { get; set; }

        [Required]
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string DENUMIRE { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string DETALII { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string QINFO { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public bool MANDATORY { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public int? DISPLAY_ORDER { get; set; }

        public TipDocument() { }

        public TipDocument(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public TipDocument(int _authenticatedUserId, string _connectionString, int _ID)
        {
            try
            {
                authenticatedUserId = _authenticatedUserId;
                connectionString = _connectionString;
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TIP_DOCUMENTsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    IDataRecord item = (IDataRecord)r;
                    TipDocumentConstructor(item);
                    break;
                }
                r.Close(); r.Dispose();
            }
            catch (Exception exp) { throw exp; }
        }

        public TipDocument(int _authenticatedUserId, string _connectionString, string _DENUMIRE)
        {
            try
            {
                authenticatedUserId = _authenticatedUserId;
                connectionString = _connectionString;
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TIP_DOCUMENTsp_GetByName", new object[] { new MySqlParameter("_DENUMIRE", _DENUMIRE) });
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    IDataRecord item = (IDataRecord)r;
                    TipDocumentConstructor(item);
                    break;
                }
                r.Close(); r.Dispose();
            }
            catch (Exception exp) { throw exp; }
        }

        public TipDocument(int _authenticatedUserId, string _connectionString, IDataRecord item)
        {
            try
            {
                authenticatedUserId = _authenticatedUserId;
                connectionString = _connectionString;
                TipDocumentConstructor(item);
            }
            catch (Exception exp) { throw exp; }
        }

        public void TipDocumentConstructor(IDataRecord item)
        {
            try { this.ID = Convert.ToInt32(item["ID"]); }
            catch { }
            try { this.DENUMIRE = item["DENUMIRE"].ToString(); }
            catch { }
            try { this.DETALII = item["DETALII"].ToString(); }
            catch { }
            try { this.QINFO = item["QINFO"].ToString(); }
            catch { }
            try { this.MANDATORY = Convert.ToBoolean( item["MANDATORY"]); }
            catch { }
            try { this.DISPLAY_ORDER = Convert.ToInt32(item["DISPLAY_ORDER"]); }
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

            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "tip_document");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TIP_DOCUMENTsp_insert", _parameters.ToArray());
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
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "tip_document");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TIP_DOCUMENTsp_update", _parameters.ToArray());
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
                Dictionary<string, string> changes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldValueCollection, CommonFunctions.JsonDeserializerSettings);
                foreach (string fieldName in changes.Keys)
                {
                    PropertyInfo[] props = this.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        //var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "actions");
                        //if (col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1 && fieldName.ToUpper() == prop.Name.ToUpper()) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                        if (fieldName.ToUpper() == prop.Name.ToUpper())
                        {
                            var tmpVal = prop.PropertyType.FullName.IndexOf("System.Nullable") > -1 && changes[fieldName] == null ? null : prop.PropertyType.FullName.IndexOf("System.String") > -1 ? changes[fieldName] : prop.PropertyType.FullName.IndexOf("System.DateTime") > -1 ? CommonFunctions.SwitchBackFormatedDate(changes[fieldName]) : ((prop.PropertyType.FullName.IndexOf("Double") > -1) ? CommonFunctions.BackDoubleValue(changes[fieldName]) : Newtonsoft.Json.JsonConvert.DeserializeObject(changes[fieldName], prop.PropertyType));
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TIP_DOCUMENTsp_soft_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        public response Validare()
        {
            bool succes;
            response toReturn = Validator.Validate(authenticatedUserId, connectionString, this, _TABLE_NAME, out succes);
            if (!succes) // daca nu s-au putut citi validarile din fisier, sau nu sunt definite in fisier, mergem pe varianta hardcodata
            {
                toReturn = new response(true, "", null, null, new List<Error>()); ;
                Error err = new Error();
                if (this.DENUMIRE == null || this.DENUMIRE.Trim() == "")
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyDenumireTipDocument");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
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

        public response HasChildrens(string tableName)
        {
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "tip_document", tableName);
        }

        public response HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "tip_document", tableName, childrenId);
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
