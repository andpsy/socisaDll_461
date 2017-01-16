using System;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Xml;

namespace SOCISA
{
    public struct ThumbNailSize
    {
        public int Width, Height;

        public ThumbNailSize(int _width, int _height)
        {
            Width = _width;
            Height = _height;
        }
    }

    /// <summary>
    /// Clasa pentru definirea unei erori
    /// </summary>
    public class Error
    {
        /// <summary>
        /// ID-ul uni pentru identificarea erorii
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Cod unic pentru identificarea erorii
        /// </summary>
        public string ERROR_CODE { get; set; }
        /// <summary>
        /// Mesajul de eroare
        /// </summary>
        public string ERROR_MESSAGE {get; set; }
        /// <summary>
        /// Denumirea obiectului care a generat eroarea
        /// </summary>
        public string ERROR_OBJECT { get; set; }
        /// <summary>
        /// Tipul erorii
        /// </summary>
        public string ERROR_TYPE { get; set; } // Critical, Warning, Information

        /// <summary>
        /// Constructorul default
        /// </summary>
        public Error() { }
    }

    /// <summary>
    /// Clasa care defineste un Filtru pentru selectia inregistrarilor din baza de date
    /// </summary>
    public class Filtru
    {
        /// <summary>
        /// Coloana din tabel pe care se aplica filtrul
        /// </summary>
        public string Coloana { get; set; }
        /// <summary>
        /// Conditia pe care trebuie sa o indeplineasca valoarea din coloana
        /// </summary>
        public string Conditie { get; set; }
        /// <summary>
        /// Valoarea cu care se compara valorile din coloana
        /// </summary>
        public string Valoare { get; set; }
        /// <summary>
        /// Operatorul pentru crearea unui filtru compus (SI, SAU)
        /// </summary>
        public string Operator { get; set; }
    }

    public class Filter
    {
        public string Sort { get; set; }
        public string Order { get; set; }
        public string Filtru { get; set; }
        public string Limit { get; set; }

        public Filter(string _sort, string _order, string _filtru, string _limit)
        {
            Sort = _sort; Order = _order; Filtru = _filtru; Limit = _limit;
        }
    }

    /// <summary>
    /// Summary description for SOCISA.CommonFunctions
    /// </summary>
    public static class CommonFunctions
    {
        public static IEnumerable<Dictionary<string, object>> Serialize(DbDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }
        private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                        DbDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        public static string DbReaderToJson(DbDataReader r)
        {
            var dict = Serialize(r);
            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            return json;
        }

        public static object[] Conditii = new object[]{
          "egal cu",
          "diferit de",
          "mai mic decat",
          "mai mic sau egal cu",
          "mai mare decat",
          "mai mare sau egal cu",
          "incepe cu",
          "nu incepe cu",
          "se termina cu",
          "nu se termina cu",
          "contine",
          "nu contine"
        };

        public static string ToMySqlFormatDate(DateTime dt)
        {
            return dt.Year + "-" + dt.Month + "-" + dt.Day;
        }

        public static string ToMySqlFormatDateWithTime(DateTime dt)
        {
            return dt.Year + "-" + dt.Month + "-" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second;
        }
        public static DateTime FromMySqlFormatDate(string dt)
        {
            try
            {
                string[] dtVals = dt.Split('-');
                return new DateTime(Convert.ToInt32(dtVals[0]), Convert.ToInt32(dtVals[1]), Convert.ToInt32(dtVals[2]));
            }
            catch
            {
                return new DateTime();
            }
        }

        public static DateTime SwitchBackFormatedDate(string dt)
        {
            DateTime toReturn = new DateTime();
            if (dt.IndexOf('.') > 0)
            {
                try
                {
                    string[] dly = dt.Split('.');
                    //string newDate = dly[1] + "/" + dly[0] + "/" + dly[2];
                    //return Convert.ToDateTime(newDate);
                    return new DateTime(Convert.ToInt32(dly[2]), Convert.ToInt32(dly[1]), Convert.ToInt32(dly[0]));
                }
                catch
                {
                    //return DateTime.Now.Date;
                    return new DateTime();
                }
            }
            else
            {
                try
                {
                    toReturn = Convert.ToDateTime(dt);
                    return toReturn;
                }
                catch
                {
                    //return DateTime.Now.Date;
                    return new DateTime();
                }
            }
            //return new DateTime();
        }

        public static string FromMySqlFormatDate(DateTime dt, string format)
        {
            //return dt.Day + separator + dt.Month + separator + dt.Year;
            return dt.ToString(format);
        }

        public static object DoubleParameterValue(object _value)
        {
            double toReturn = 0;
            try
            {
                if (_value.ToString().Trim() != "")
                {
                    toReturn = Convert.ToDouble(_value);
                    return toReturn;
                }
                return null;
            }
            catch { return null; }
        }

        public static MySqlParameter SetNull(DbColumn _dc, MySqlParameter _initialParam, object _initialValue)
        {
            MySqlParameter _newParam = _initialParam;
            if (Convert.ToBoolean(_dc.AllowDBNull) && _initialValue.ToString().Trim() == "")
                _newParam.Value = null;
            return _newParam;
        }

        public static MySqlParameter SetNull(MySqlParameter _initialParam)
        {
            MySqlParameter _newParam = _initialParam;
            if (_initialParam.Value != null && (_initialParam.Value.ToString().Trim() == "" || _initialParam.Value.ToString() == "1-1-1"))
                _newParam.Value = null;
            return _newParam;
        }

        public static string MySqlErrorParser(MySqlException mySqlException)
        {
            return SOCISA.ErrorParser.ParseError(mySqlException);
        }

        public static string getConditie(string _strConditie)
        {
            switch (_strConditie)
            {
                case "egal cu":
                    return " = ";
                //break;
                case "diferit de":
                    return " != ";
                //break;
                case "mai mic decat":
                    return " < ";
                //break;
                case "mai mic sau egal cu":
                    return " <= ";
                //break;
                case "mai mare decat":
                    return " > ";
                //break;
                case "mai mare sau egal cu":
                    return " >= ";
                //break;
                case "incepe cu":
                    return " LIKE <#VALUE#>% ";
                //break;
                case "nu incepe cu":
                    return " NOT LIKE <#VALUE#>% ";
                //break;
                case "se termina cu":
                    return " LIKE %<#VALUE#> ";
                //break;
                case "contine":
                    return " LIKE %<#VALUE#>% ";
                //break;
                case "nu contine":
                    return " NOT LIKE %<#VALUE#>% ";
                    //break;
            }
            return null;
        }

        public static string GenerateSimpleFilter(string _filtru, DbDataReader _table)
        {
            string _strFilter = "";
            int column_counter = 0;
            int counter = GetDbReaderRowCount(_table);
            while (_table.Read())
            {
                IDataRecord dr = (IDataRecord)_table;
                string columnName = dr["Field"].ToString().ToLower();
                string columnType = dr["Type"].ToString().ToLower();
                if (columnName != "id" && columnName.IndexOf("id_") == -1 && columnName.IndexOf("_id") == -1)
                {
                    if (_strFilter.Length > 0 && column_counter < counter)
                        _strFilter = String.Format("{0} OR ", _strFilter);

                    if (columnType.IndexOf("bool") > -1 || columnType.IndexOf("tinyint") > -1)
                        _strFilter = String.Format("{0}CAST(`{1}` AS CHAR) LIKE '%{2}%'", _strFilter, columnName, _filtru);
                    else if (columnType.IndexOf("int") > -1 || columnType.IndexOf("double") > -1 || columnType.IndexOf("decimal") > -1)
                        _strFilter = String.Format("{0}CAST(`{1}` AS CHAR) LIKE '%{2}%'", _strFilter, columnName, _filtru);
                    else if (columnType.IndexOf("date") > -1)
                        _strFilter = String.Format("{0}CAST(`{1}` AS CHAR) LIKE '%{2}%'", _strFilter, columnName, _filtru);
                    else if (columnType.IndexOf("char") > -1 || columnType.IndexOf("text") > -1)
                        _strFilter = String.Format("{0}`{1}` LIKE '%{2}%'", _strFilter, columnName, _filtru);
                }
                column_counter++;
            }
            return _strFilter;
        }

        public static int GetDbReaderRowCount(DbDataReader r)
        {
            int counter = 0;
            while (r.Read())
            {
                counter++;
            }
            return counter;
        }

        public static string GenerateFilterFromJson(string _filtru, DbDataReader _table)
        {
            Filtru[] filtru = JsonConvert.DeserializeObject<Filtru[]>(_filtru);
            //DataTable dt = full_table_columns(_table_name);
            string _strFilter = "";


            for (int i = 0; i < filtru.Length; i++)
            {
                {
                    string _col = filtru[i].Coloana;

                    string columnType = "";

                    if (_col == "EXTRA CONTRACT") _col = "D.EXTRACONTRACT";
                    else _col = _col.Replace(' ', '_');

                    while (_table.Read())
                    {
                        IDataRecord dr = (IDataRecord)_table;

                        if (dr["Field"].ToString() == _col)
                        {
                            columnType = dr["Type"].ToString();
                            break;
                        }

                        switch (_col)
                        {
                            case "ASIGURAT_CASCO":
                                _col = "ASIGC.DENUMIRE";
                                columnType = "VARCHAR";
                                break;
                            case "ASIGURAT_RCA":
                                _col = "ASIGR.DENUMIRE";
                                columnType = "VARCHAR";
                                break;
                            case "ASIGURATOR_CASCO":
                                _col = "SC.DENUMIRE";
                                columnType = "VARCHAR";
                                break;
                            case "ASIGURATOR_RCA":
                                _col = "SR.DENUMIRE";
                                columnType = "VARCHAR";
                                break;
                            case "INTERVENIENT":
                                _col = "I.DENUMIRE";
                                columnType = "VARCHAR";
                                break;
                            case "NR_AUTO_CASCO":
                                _col = "AC.NR_AUTO";
                                columnType = "VARCHAR";
                                break;
                            case "NR_AUTO_RCA":
                                _col = "AR.NR_AUTO";
                                columnType = "VARCHAR";
                                break;
                            case "TIP_DOSAR":
                                _col = "TD.DENUMIRE";
                                columnType = "VARCHAR";
                                break;
                            default:
                                _col = "D." + _col;
                                break;
                        }
                        string _tmpCond = getConditie(filtru[i].Conditie);
                        string _tmpVal = filtru[i].Valoare;

                        if (_tmpVal.ToLower() == "null" && (_tmpCond.Trim() == "=" || _tmpCond.Trim() == "!="))
                        {
                            _strFilter += (_col + (_tmpCond.Trim() == "=" ? " IS NULL " : " IS NOT NULL "));
                        }
                        else if (columnType.ToUpper().StartsWith("DATETIME"))
                        {
                            _tmpVal = ToMySqlFormatDate(SwitchBackFormatedDate(_tmpVal));
                            _strFilter += (_col + _tmpCond + "'" + _tmpVal + "' ");
                        }
                        else if (columnType.ToUpper().StartsWith("VARCHAR") || columnType.ToUpper().StartsWith("TEXT"))
                        {
                            if (_tmpCond.Replace("<#VALUE#>", "") != _tmpCond)
                            {
                                string _tC = _tmpCond.Replace("%<#VALUE#>%", "'%" + _tmpVal + "%'");
                                _tC = _tC.Replace("%<#VALUE#>", "'%" + _tmpVal + "'");
                                _tC = _tC.Replace("<#VALUE#>%", "'" + _tmpVal + "%'");
                                _tC = _tC.Replace("<#VALUE#>", "'" + _tmpVal + "'");
                                _strFilter += (_col + _tC + " ");
                            }
                            else
                                _strFilter += (_col + _tmpCond + " '" + _tmpVal + "' ");
                        }
                        else
                        {
                            if (_tmpCond.Replace("<#VALUE#>", "") != _tmpCond)
                            {
                                _strFilter += (_col + " = " + _tmpVal);
                            }
                            else
                                _strFilter += (_col + _tmpCond + " " + _tmpVal + " ");
                        }
                        if (filtru[i].Operator != null && filtru[i].Operator.Trim() != "" && i < filtru.Length - 1)
                            _strFilter += (filtru[i].Operator.Replace("SI", "AND").Replace("SAU", "OR") + " ");
                    }
                }
            }
            return _strFilter;
        }

        public static bool HasRight(object _dictionary, string _right)
        {
            try
            {
                Dictionary<string, string> UserRights = (Dictionary<string, string>)_dictionary;
                try
                {
                    if (UserRights["administrare"] != null) return true;
                }
                catch { }
                if (UserRights[_right.ToLower()] != null) return true;
                else return false;
            }
            catch (Exception exp)
            {
                exp.ToString();
                return false;
            }
        }

        public static string DoubleValue(string _value)
        {
            return _value.Replace(",", "");
        }

        public static double BackDoubleValue(string _value)
        {
            double toReturn;
            try
            {
                toReturn = Convert.ToDouble(_value);
                return toReturn;
            }
            catch
            {
                try
                {
                    toReturn = Convert.ToDouble(_value.Replace(",", ""));
                    return toReturn;
                }
                catch { return Double.NaN; }
            }
        }

        public static bool Like(this string str, string pattern)
        {
            return new Regex(
                "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            ).IsMatch(str);
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string table_columns(int _authenticatedUserId, string _connectionString, string _table)
        {
            try
            {
                DataAccess da = new DataAccess(_authenticatedUserId, _connectionString, CommandType.Text, String.Format("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'socisa' AND TABLE_NAME = '{0}';", _table));
                DbDataReader r = da.ExecuteSelectQuery();
                string toReturn = "";
                while (r.Read())
                {
                    toReturn += (((IDataRecord)r)["COLUMN_NAME"].ToString() + ",");
                }
                return toReturn;
            }
            catch (Exception exp) { throw exp; }
        }

        /// <summary>
        /// Proprietate care mapeaza tabela din fisierul .xml cu erori predefinite
        /// </summary>
        public static Dictionary<string, Error> ErrorMessages
        {
            get
            {
                Dictionary<string, Error> errorMessages = new Dictionary<string, Error>();
                XmlReader r = XmlReader.Create("ErrorMessages.xml");

                XmlDocument xdoc = new XmlDocument();//xml doc used for xml parsing

                xdoc.Load(r);//loading XML in xml doc

                XmlNodeList xNodelst = xdoc.DocumentElement.SelectNodes("ErrorMessages");//reading node so that we can traverse thorugh the XML

                foreach (XmlNode xNode in xNodelst)//traversing XML 
                {
                    if (xNode.Name == "ErrorMessage" && xNode.HasChildNodes) {
                        string errCode = "";
                        Error err = new Error();
                        foreach (XmlNode child in xNode.ChildNodes)
                        {
                            switch (child.Name)
                            {
                                case "ID":
                                    err.ID = Convert.ToInt32(child.Value);
                                    break;
                                case "ERROR_CODE":
                                    errCode = err.ERROR_CODE = child.Value.ToString();
                                    break;
                                case "ERROR_MESSAGE":
                                    err.ERROR_MESSAGE = child.Value.ToString();
                                    break;
                                case "ERROR_OBJECT":
                                    err.ERROR_OBJECT = child.Value.ToString();
                                    break;
                                case "ERROR_TYPE":
                                    err.ERROR_TYPE = child.Value.ToString();
                                    break;
                            }
                        }
                        errorMessages.Add(errCode, err);
                    }
                }
                return errorMessages;
            }
        }

        public static Error ErrorMessage(string errorCode)
        {
            try
            {
                Error error = new Error();
                ErrorMessages.TryGetValue(errorCode, out error);
                return error;
            }
            catch { return null; }
        }


        public static response ValidareColoane(object item, string fieldValueCollection)
        {
            response toReturn = new response(true, null, null, new List<Error>());
            try
            {
                Dictionary<string, string> changes = JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldValueCollection);
                foreach (string fieldName in changes.Keys)
                {
                    bool gasit = false;
                    PropertyInfo[] props = item.GetType().GetProperties();
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
                        Error err = CommonFunctions.ErrorMessage("campInexistentInTabela");
                        return new response(false, err.ERROR_MESSAGE, null, new List<Error>() { err });
                    }
                }
            }
            catch
            {
                Error err = CommonFunctions.ErrorMessage("cannotConvertStringToTableColumns");
                return new response(false, err.ERROR_MESSAGE, null, new List<Error>() { err });
            }
            return toReturn;
        }

        public static bool HasChildrens(int authenticatedUserId, string connectionString, object item, string parentTableName, string childTableName)
        {
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TABLEsp_GetReferences", new object[] { new MySqlParameter("_PARENT_TABLE", parentTableName), new MySqlParameter("_CHILD_TABLE", childTableName) });
            DbDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                if (r["REFERENCED_TABLE_NAME"].ToString().ToUpper() == childTableName.ToUpper())
                {
                    PropertyInfo[] props = item.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.Name.ToUpper() == r["COLUMN_NAME"].ToString().ToUpper())
                        {
                            da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "CHILDRENSsp_Get", new object[] { new MySqlParameter("_PRIMARY_KEY_VALUE", prop.GetValue(item)), new MySqlParameter("_EXTERNAL_ID", r["REFERENCED_COLUMN_NAME"].ToString()), new MySqlParameter("_EXTERNAL_TABLE", r["REFERENCED_TABLE_NAME"].ToString()) });
                            object counter = da.ExecuteScalarQuery();
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
                    PropertyInfo pi = item.GetType().GetProperty("ID");
                    da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "CHILDRENSsp_Get", new object[] { new MySqlParameter("_PRIMARY_KEY_VALUE", pi.GetValue(item)), new MySqlParameter("_EXTERNAL_ID", r["COLUMN_NAME"].ToString()), new MySqlParameter("_EXTERNAL_TABLE", r["TABLE_NAME"].ToString()) });
                    object counter = da.ExecuteScalarQuery();
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
            return false;
        }

        public static bool HasChildren(int authenticatedUserId, string connectionString, object item, string parentTableName, string childTableName, int childrenId)
        {
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TABLEsp_GetReferences", new object[] { new MySqlParameter("_PARENT_TABLE", parentTableName), new MySqlParameter("_CHILD_TABLE", childTableName) });
            DbDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                if (r["REFERENCED_TABLE_NAME"].ToString().ToUpper() == childTableName.ToUpper())
                {
                    PropertyInfo[] props = item.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.Name.ToUpper() == r["COLUMN_NAME"].ToString().ToUpper())
                        {
                            da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "CHILDRENsp_Get", new object[] { new MySqlParameter("_PRIMARY_KEY_VALUE", prop.GetValue(item)), new MySqlParameter("_EXTERNAL_ID", r["REFERENCED_COLUMN_NAME"].ToString()), new MySqlParameter("_EXTERNAL_TABLE", r["REFERENCED_TABLE_NAME"].ToString()), new MySqlParameter("_CHILDREN_ID_FIELD", "1"), new MySqlParameter("_CHILDREN_ID_VALUE", "1") });
                            object counter = da.ExecuteScalarQuery();
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
                    da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TABLEsp_GetReferences", new object[] { new MySqlParameter("_PARENT_TABLE", r["TABLE_NAME"].ToString()), new MySqlParameter("_CHILD_TABLE", childTableName) });
                    DbDataReader rc = da.ExecuteSelectQuery();
                    while (rc.Read())
                    {
                        PropertyInfo pi = item.GetType().GetProperty("ID");
                        //da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "CHILDRENsp_Get", new object[] { new MySqlParameter("_PRIMARY_KEY_VALUE", pi.GetValue(item)), new MySqlParameter("_EXTERNAL_ID", r["REFERENCED_COLUMN_NAME"].ToString()), new MySqlParameter("_EXTERNAL_TABLE", r["REFERENCED_TABLE_NAME"].ToString()), new MySqlParameter("_CHILDREN_ID_FIELD", rc["COLUMN_NAME"].ToString()), new MySqlParameter("_CHILDREN_ID_VALUE", childrenId) });
                        da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "CHILDRENsp_Get", new object[] { new MySqlParameter("_PRIMARY_KEY_VALUE", pi.GetValue(item)), new MySqlParameter("_EXTERNAL_ID", r["COLUMN_NAME"].ToString()), new MySqlParameter("_EXTERNAL_TABLE", r["TABLE_NAME"].ToString()), new MySqlParameter("_CHILDREN_ID_FIELD", rc["COLUMN_NAME"].ToString()), new MySqlParameter("_CHILDREN_ID_VALUE", childrenId) });
                        object counter = da.ExecuteScalarQuery();
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
            }
            return false;
        }

        public static string GenerateFilterFromJsonObject(object item)
        {
            string toReturn = "";

            PropertyInfo[] props = item.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string propName = prop.Name;
                string propType = prop.PropertyType.ToString();
                object propValue = prop.GetValue(item, null);
                propValue = propValue == null ? DBNull.Value : propValue;
                if (propType != null && propValue != null && propValue != DBNull.Value)
                {
                    try
                    {
                        //if (propName.ToUpper().IndexOf("ID") != 0) // nu cautam si dupa ID-uri
                        {
                            switch (propType)
                            {
                                case "SOCISA.AsiguratiJson":
                                    toReturn += String.Format("{2}{0} like '%{1}%'", propName == "AsiguratCasco" ? "ASIGC.DENUMIRE" : "ASIGR.DENUMIRE", ((Models.Asigurat)propValue).DENUMIRE, (toReturn == "" ? "" : " AND "));
                                    break;
                                case "SOCISA.AutoJson":
                                    toReturn += String.Format("{2}{0} like '%{1}%'", propName == "AutoCasco" ? "AC.NR_AUTO" : "AR.NR_AUTO", ((Models.Auto)propValue).NR_AUTO, (toReturn == "" ? "" : " AND "));
                                    break;
                                case "SOCISA.SocietatiAsigurareJson":
                                    toReturn += String.Format("{2}{0} like '%{1}%'", propName == "SocietateCasco" ? "SC.DENUMIRE_SCURTA" : "SR.DENUMIRE_SCURTA", ((Models.SocietateAsigurare)propValue).DENUMIRE_SCURTA, (toReturn == "" ? "" : " AND "));
                                    break;
                                case "SOCISA.IntervenientiJson":
                                    toReturn += String.Format("{2}{0} like '%{1}%'", "I.DENUMIRE", ((Models.Intervenient)propValue).DENUMIRE, (toReturn == "" ? "" : " AND "));
                                    break;
                                case "SOCISA.NomenclatorJson":
                                    toReturn += String.Format("{2}{0} like '%{1}%'", "TD.DENUMIRE", ((Models.Nomenclator)propValue).DENUMIRE, (toReturn == "" ? "" : " AND "));
                                    break;

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

        public static object GetChildrens(object item, string table_name)
        {
            try
            {
                MethodInfo methodToRun = null;
                MethodInfo[] mis = item.GetType().GetMethods();
                foreach (MethodInfo mi in mis)
                {
                    if (mi.Name.ToLower().IndexOf(table_name.ToLower()) > -1)
                    {
                        methodToRun = mi;
                        break;
                    }
                }
                dynamic r = methodToRun.Invoke(item, null);
                return r;
            }catch { return null; }
        }

        public static object GetChildren(object item, string table_name, int childrenId)
        {
            try
            {
                object childrens = GetChildrens(item, table_name);
                if (childrens is Array)
                {
                    foreach (object it in (Array)childrens)
                    {
                        PropertyInfo pi = it.GetType().GetProperty("ID");
                        if (Convert.ToInt32(pi.GetValue(it)) == childrenId)
                        {
                            return it;
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public static bool LoadTemplateFileIntoDb(int _authenticatedUserId, string _connectionString, string filePath, string _DETALII)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                int FileSize;
                byte[] rawData;
                FileStream fs;
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                FileSize = (int)fs.Length;

                rawData = new byte[FileSize];
                fs.Read(rawData, 0, FileSize);
                DataAccess da = new DataAccess(_authenticatedUserId, _connectionString, CommandType.StoredProcedure, "TEMPLATESsp_insert", new object[]
                {
                    new MySqlParameter("_DENUMIRE_FISIER", fi.Name),
                    new MySqlParameter("_EXTENSIE_FISIER", fi.Extension),
                    new MySqlParameter("_DIMENSIUNE_FISIER", FileSize),
                    new MySqlParameter("_FILE_CONTENT", rawData),
                    new MySqlParameter("_DETALII", _DETALII)
                });
                response r = da.ExecuteInsertQuery();
                return r.Status;
            }catch { return false; }
        }

        public static byte[] GetTemplateFileIntoDb(int _authenticatedUserId, string _connectionString, string fileName)
        {
            try
            {
                byte[] rawData;
                DataAccess da = new DataAccess(_authenticatedUserId, _connectionString, CommandType.StoredProcedure, "TEMPLATESsp_GetByName", new object[]
                {
                    new MySqlParameter("_DENUMIRE_FISIER", fileName)
                });
                IDataReader r = da.ExecuteSelectQuery();
                r.Read();
                rawData = (byte[])r["FILE_CONTENT"];
                return rawData;
            }
            catch { return null; }
        }

        public static byte[] GetTemplateFileFromDb(int _authenticatedUserId, string _connectionString, int templateId)
        {
            try
            {
                byte[] rawData;
                DataAccess da = new DataAccess(_authenticatedUserId, _connectionString, CommandType.StoredProcedure, "TEMPLATESsp_GetById", new object[]
                {
                    new MySqlParameter("_ID", templateId)
                });
                IDataReader r = da.ExecuteSelectQuery();
                r.Read();
                rawData = (byte[])r["FILE_CONTENT"];
                return rawData;
            }
            catch { return null; }
        }

        public static byte[] GetFileContentFromFile(string fileName)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                byte[] toReturn = new byte[fs.Length];
                fs.Read(toReturn, 0, (int)fs.Length);
                return toReturn;
            }catch { return null; }
        }

        public static byte[] GetFileContentFromDb(int _authenticatedUserId, string _connectionString, int documentScanatId)
        {
            try
            {
                byte[] rawData;
                DataAccess da = new DataAccess(_authenticatedUserId, _connectionString, CommandType.StoredProcedure, "DOCUMENTE_SCANATEsp_GetById", new object[]
                {
                    new MySqlParameter("_ID", documentScanatId)
                });
                IDataReader r = da.ExecuteSelectQuery();
                r.Read();
                rawData = (byte[])r["FILE_CONTENT"];
                return rawData;
            }
            catch { return null; }
        }
    }
}