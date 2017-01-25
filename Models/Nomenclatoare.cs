using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace SOCISA.Models
{
    /// <summary>
    /// Clasa care contine definitia obiectului ce mapeaza tabela cu Nomenclatoare din baza de date
    /// </summary>
    public class Nomenclator
    {
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }
        public string TableName { get; set; }
        public int? ID { get; set; }
        public string DENUMIRE { get; set; }
        public string DETALII { get; set; }
        public string QINFO { get; set; }

        /// <summary>
        /// Constructorul default
        /// </summary>
        public Nomenclator() { }

        /// <summary>
        /// Constructor pentru initializarea numelui tabelei din care se selecteaza nomenclatorul
        /// </summary>
        /// <param name="_TableName">Numele tabelei</param>
        public Nomenclator(int _authenticatedUserId, string _connectionString, string _TableName)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            TableName = _TableName;
        }

        /// <summary>
        /// Constructor pentru crearea unui obiect pe baza ID-ului unic si a tabelei cu nomenclatorul dorit
        /// </summary>
        /// <param name="_TableName">Numele tabelei</param>
        /// <param name="_ID">ID-ul unic din baza de date</param>
        public Nomenclator(int _authenticatedUserId, string _connectionString, string _TableName, int _ID)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            TableName = _TableName;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format( "{0}sp_GetById", TableName.ToUpper()), new object[] { new MySqlParameter("_ID", _ID) });
            DbDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                NomenclatorConstructor(item);
                break;
            }
        }

        public Nomenclator(int _authenticatedUserId, string _connectionString, string tableName, IDataRecord item)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            TableName = tableName;
            NomenclatorConstructor(item);
        }

        public void NomenclatorConstructor(IDataRecord item)
        {
            this.ID = Convert.ToInt32(item["ID"]);
            this.DENUMIRE = item["DENUMIRE"].ToString();
            this.DETALII = item["DETALII"].ToString();
            try
            {
                this.QINFO = item["QINFO"].ToString();
            }
            catch { }
        }

        /// <summary>
        /// Metoda pentru inserarea Nomenclatorului curent
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
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, TableName);
            foreach (PropertyInfo prop in props)
            {
                string propName = prop.Name;
                string propType = prop.PropertyType.ToString();
                object propValue = prop.GetValue(this, null);
                propValue = propValue == null ? DBNull.Value : propValue;
                if (propType != null)
                {
                    if (propName.ToUpper() != "ID" && propName != "TableName" && col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                        _parameters.Add(new MySqlParameter(String.Format("_{0}", propName.ToUpper()), propValue));
                }
            }
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format("{0}sp_insert", TableName.ToUpper()), _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status) this.ID = toReturn.InsertedId;

            return toReturn;
        }

        /// <summary>
        /// Metoda pentru modificarea Nomenclatorului curent
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
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, TableName);
            foreach (PropertyInfo prop in props)
            {
                string propName = prop.Name;
                string propType = prop.PropertyType.ToString();
                object propValue = prop.GetValue(this, null);
                propValue = propValue == null ? DBNull.Value : propValue;
                if (propType != null)
                {
                    if (propName != "TableName" && col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                        _parameters.Add(new MySqlParameter(String.Format("_{0}", propName.ToUpper()), propValue));
                }
            }
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format("{0}sp_update", TableName.ToUpper()), _parameters.ToArray());
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

        /// <summary>
        /// Metoda pentru stergerea Nomenclatorului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Delete()
        {
            response toReturn = new response(false, "", null, null, new List<Error>());;
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format("{0}sp_delete", this.TableName), _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru validarea Nomenclatorului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Validare()
        {
            response toReturn = new response(true, "", null, null, new List<Error>());;
            Error err = new Error();
            if (this.DENUMIRE == null || this.DENUMIRE.Trim() == "")
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyDenumireNomenclator");
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

        /// <summary>
        /// Metoda pentru generarea filtrului de cautare/filtrare pe baza coloanelor si a valorilor acestora.
        /// Folosita la cautarea cu TypeAhead
        /// </summary>
        /// <returns>string cu filtrul ce va fi trimis ca parametru in procedura stocata din BD pentru filtrare</returns>
        public string GenerateFilterFromJsonObject()
        {
            return Filtering.GenerateFilterFromJsonObject(this);
        }

        public response HasChildrens(string tableName)
        {
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, TableName, tableName);
        }

        public response HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, TableName, tableName, childrenId);
        }

        public response GetChildrens(string tableName)
        {
            return CommonFunctions.GetChildrens(this, tableName);
        }

        public response GetChildren(string tableName, int childrenId)
        {
            return CommonFunctions.GetChildren(this, tableName, childrenId);
        }

        public int? GetIdByName(string _tableName, string _Name)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format("{0}sp_GetIdByName", _tableName.ToUpper()), new object[] { new MySqlParameter("_DENUMIRE", _Name) });
                return Convert.ToInt32(da.ExecuteScalarQuery());
            }
            catch { return null; }
        }
    }
}