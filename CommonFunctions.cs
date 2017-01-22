using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

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
    /// Summary description for SOCISA.CommonFunctions
    /// </summary>
    public static class CommonFunctions
    {
        public static Dictionary<string, string> ClassNamesTableNamesAlliases = new Dictionary<string, string>()
        {
            {"Action", "actions" },
            {"Asigurat","asigurati" },
            {"Auto","auto" },
            { "DocumentScanat","documente_scanate"},
            {"Dosar", "dosare" },
            {"DosarProces","dosare_procese" },
            {"DosarStadiu","dosare_stadii" },
            {"DosarStadiuSentinta", "dosare_stadii_sentinte" },
            {"Drept","drepturi" },
            {"Intervenient", "intervenienti" },
            {"Mesaj","mesaje" },
            {"MesajUtilizator","mesaje_utilizatori" },
            {"Nomenclator","nomenclatoare" },
            {"Proces","procese" },
            {"Sentinta", "sentinte" },
            {"Setare", "setari" },
            {"SocietateAsigurare", "societati_asigurare" },
            {"Stadiu", "stadii" },
            {"Utilizator", "utilizatori" },
            {"UtilizatorAction", "utilizatori_actions" },
            {"UtilizatorDosar", "utilizatori_dosare" },
            {"UtilizatorDrept", "utilizatori_drepturi" },
            {"UtilizatorSetare", "utilizatori_setari" },
            {"UtilizatorSocietateAdministrata", "utilizatori_societati" },
            {"UtilizatorSocietate", "utilizatori_societati" }
        };
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
        private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols, DbDataReader reader)
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

        public static int GetDbReaderRowCount(DbDataReader r)
        {
            int counter = 0;
            while (r.Read())
            {
                counter++;
            }
            return counter;
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
                        Error err = ErrorParser.ErrorMessage("campInexistentInTabela");
                        return new response(false, err.ERROR_MESSAGE, null, new List<Error>() { err });
                    }
                }
            }
            catch
            {
                Error err = ErrorParser.ErrorMessage("cannotConvertStringToTableColumns");
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