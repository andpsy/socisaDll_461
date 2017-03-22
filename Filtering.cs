using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace SOCISA
{
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

        public Filter() { }
        public Filter(string _sort, string _order, string _filtru, string _limit)
        {
            Sort = _sort; Order = _order; Filtru = _filtru; Limit = _limit;
        }
    }

    public static class Filtering
    {
        public static string GenerateFilterFromJsonObject(Type T, string _filter, int authenticatedUserId, string connectionString)
        {
            string toReturn = "";

            try
            {
                //dynamic jObj = JsonConvert.DeserializeObject(_filter);
                JObject jObj = JObject.Parse(_filter);
                var x = Activator.CreateInstance(T, new object[] { authenticatedUserId, connectionString });
                string className = x.GetType().Name; // to check if it returns only the short name
                string tableAllias = CommonFunctions.ClassNamesTableNamesAlliases[className].ToUpper();
                PropertyInfo[] pisX = x.GetType().GetProperties();
                //PropertyInfo[] pisJObj = jObj.GetType().GetProperties();
                //foreach (PropertyInfo piJObj in pisJObj)
                foreach(var t in jObj)
                {
                    string key = t.Key;
                    JToken j = t.Value;
                    bool propertyInMasterObject = false;
                    foreach (PropertyInfo piX in pisX)
                    {
                        //if (piX.Name == piJObj.Name)
                        if (piX.Name == key)
                        {
                            /* -- folosim la toate selectia cui [*]zzzzz[*]
                            switch (piX.Name)
                            {
                                case "NR_SCA":
                                    if (j.ToString().IndexOf('*') == j.ToString().Length - 1)
                                        toReturn += String.Format("{2}DOSARE.`{0}` LIKE '{1}%'", "NR_SCA", j.ToString().Remove(j.ToString().Length - 1), (toReturn == "" ? "" : " AND "));
                                    else
                                        toReturn += String.Format("{2}DOSARE.`{0}` = '{1}'", "NR_SCA", j.ToString(), (toReturn == "" ? "" : " AND "));
                                    break;
                                case "NR_DOSAR_CASCO":
                                    if (j.ToString().IndexOf('*') == j.ToString().Length - 1)
                                        toReturn += String.Format("{2}DOSARE.`{0}` LIKE '{1}%'", "NR_DOSAR_CASCO", j.ToString().Remove(j.ToString().Length - 1), (toReturn == "" ? "" : " AND "));
                                    else
                                        toReturn += String.Format("{2}DOSARE.`{0}` = '{1}'", "NR_DOSAR_CASCO", j.ToString(), (toReturn == "" ? "" : " AND "));
                                    break;
                                default:
                                    //piX.SetValue(x, piJObj.GetValue(jObj));
                                    switch (piX.PropertyType.ToString())
                                    {
                                        case "System.DateTime":
                                            //toReturn += String.Format("{2}{3}.`{0}` = '{1}'", piX.Name, piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "), tableAllias);
                                            toReturn += String.Format("{2}{3}.`{0}` = '{1}'", piX.Name, j.ToString(), (toReturn == "" ? "" : " AND "), tableAllias);
                                            break;
                                        default:
                                            //toReturn += String.Format("{2}{3}.`{0}` like '%{1}%'", piX.Name, piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "), tableAllias);
                                            toReturn += String.Format("{2}{3}.`{0}` like '{1}%'", piX.Name, j.ToString(), (toReturn == "" ? "" : " AND "), tableAllias);
                                            break;
                                    }
                                    break;
                            }
                            */
                            string op = "";
                            switch (piX.PropertyType.ToString())
                            {
                                case "System.DateTime":
                                case "System.Nullable`1[System.DateTime]":
                                    if (j.ToString().IndexOf('?') > -1)
                                    {
                                        DateTime dStart = Convert.ToDateTime(j.ToString().Split('?')[0]);
                                        DateTime dEnd = Convert.ToDateTime(j.ToString().Split('?')[1]);
                                        toReturn += String.Format("{0}({1}.`{2}` >= '{3}' AND {1}.`{2}` <= '{4}')", (toReturn == "" ? " " : " AND "), tableAllias, piX.Name, CommonFunctions.ToMySqlFormatDate(dStart), CommonFunctions.ToMySqlFormatDate(dEnd));
                                    }
                                    else
                                    {
                                        DateTime d = Convert.ToDateTime(j.ToString());
                                        toReturn += String.Format("{0} {1}.`{2}` = '{3}'", (toReturn == "" ? " " : " AND "), tableAllias, piX.Name, CommonFunctions.ToMySqlFormatDate(d));
                                    }
                                    break;
                                case "System.Double":
                                case "System.Nullable`1[System.Double]":
                                case "System.Decimal":
                                case "System.Nullable`1[System.Decimal]":
                                    op = j.ToString().StartsWith("*") && j.ToString().EndsWith("*") ? String.Format("like '%{0}%'", CommonFunctions.BackDoubleValue(j.ToString().Substring(1, j.ToString().Length - 2)).ToString(CultureInfo.InvariantCulture) ) : 
                                        j.ToString().StartsWith("*") ? String.Format("like '%{0}'", CommonFunctions.BackDoubleValue( j.ToString().Substring(1, j.ToString().Length - 1)).ToString(CultureInfo.InvariantCulture) ) : 
                                        j.ToString().EndsWith("*") ? String.Format("like '{0}%'", CommonFunctions.BackDoubleValue( j.ToString().Substring(0, j.ToString().Length - 1)).ToString(CultureInfo.InvariantCulture) ) : 
                                        String.Format("= '{0}'", CommonFunctions.BackDoubleValue( j.ToString()).ToString(CultureInfo.InvariantCulture) );
                                    toReturn += String.Format("{0}{1}.`{2}` {3}", (toReturn == "" ? "" : " AND "), tableAllias, piX.Name, op);
                                    break;
                                default:
                                    op = j.ToString().StartsWith("*") && j.ToString().EndsWith("*") ? String.Format("like '%{0}%'", j.ToString().Substring(1, j.ToString().Length - 2)) : j.ToString().StartsWith("*") ? String.Format("like '%{0}'", j.ToString().Substring(1, j.ToString().Length - 1)) : j.ToString().EndsWith("*") ? String.Format("like '{0}%'", j.ToString().Substring(0, j.ToString().Length - 1)) : String.Format("= '{0}'", j.ToString());
                                    toReturn += String.Format("{0}{1}.`{2}` {3}", (toReturn == "" ? "" : " AND "), tableAllias, piX.Name, op);
                                    break;
                            }

                            propertyInMasterObject = true;
                            break;
                        }
                    }
                    if (!propertyInMasterObject) // pt. campuri externe trimise pt. filtrare - ex. Nume societate sau asigurat din Dosar
                    {
                        string op = j.ToString().StartsWith("*") && j.ToString().EndsWith("*") ? String.Format("like '%{0}%'", j.ToString().Substring(1, j.ToString().Length - 2)) : j.ToString().StartsWith("*") ? String.Format("like '%{0}'", j.ToString().Substring(1, j.ToString().Length - 1)) : j.ToString().EndsWith("*") ? String.Format("like '{0}%'", j.ToString().Substring(0, j.ToString().Length - 2)) : String.Format("= '{0}'", j.ToString());
                        //switch (piJObj.Name.ToLower().Replace("_", "").Replace(" ", ""))
                        switch (key.ToLower().Replace("_", "").Replace(" ", ""))
                        {
                            /* -- nu e cazul, ca nu sunt externe
                            case "nrsca":
                                if(j.ToString().IndexOf('*') == j.ToString().Length-1)
                                    toReturn += String.Format("{2}DOSARE.`{0}` LIKE '{1}%'", "NR_SCA", j.ToString().Remove(j.ToString().Length-1), (toReturn == "" ? "" : " AND "));
                                else
                                    toReturn += String.Format("{2}DOSARE.`{0}` = '{1}'", "NR_SCA", j.ToString(), (toReturn == "" ? "" : " AND "));
                                break;
                            case "nrdosarcasco":
                                if (j.ToString().IndexOf('*') == j.ToString().Length - 1)
                                    toReturn += String.Format("{2}DOSARE.`{0}` LIKE '{1}%'", "NR_DOSAR_CASCO", j.ToString().Remove(j.ToString().Length - 1), (toReturn == "" ? "" : " AND "));
                                else
                                    toReturn += String.Format("{2}DOSARE.`{0}` = '{1}'", "NR_DOSAR_CASCO", j.ToString(), (toReturn == "" ? "" : " AND "));
                                break;
                            */
                            case "asiguratcasco":
                                //toReturn += String.Format("{2}ASIGC.`{0}` LIKE '%{1}%'", "DENUMIRE", piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "));
                                //toReturn += String.Format("{2}ASIGC.`{0}` LIKE '{1}%'", "DENUMIRE", j.ToString(), (toReturn == "" ? "" : " AND "));
                                toReturn += String.Format("{2}ASIGC.`{0}` {1}", "DENUMIRE", op, (toReturn == "" ? "" : " AND "));
                                break;
                            case "asiguratrca":
                                //toReturn += String.Format("{2}ASIGR.`{0}` LIKE '%{1}%'", "DENUMIRE", piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "));
                                //toReturn += String.Format("{2}ASIGR.`{0}` LIKE '{1}%'", "DENUMIRE", j.ToString(), (toReturn == "" ? "" : " AND "));
                                toReturn += String.Format("{2}ASIGR.`{0}` {1}", "DENUMIRE", op, (toReturn == "" ? "" : " AND "));
                                break;
                            case "autocasco":
                                //toReturn += String.Format("{2}AC.`{0}` LIKE '%{1}%'", "NR_AUTO", piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "));
                                //toReturn += String.Format("{2}AC.`{0}` LIKE '{1}%'", "NR_AUTO", j.ToString(), (toReturn == "" ? "" : " AND "));
                                toReturn += String.Format("{2}AC.`{0}` {1}", "NR_AUTO", op, (toReturn == "" ? "" : " AND "));
                                break;
                            case "autorca":
                                //toReturn += String.Format("{2}AR.`{0}` LIKE '%{1}%'", "NR_AUTO", piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "));
                                //toReturn += String.Format("{2}AR.`{0}` LIKE '{1}%'", "NR_AUTO", j.ToString(), (toReturn == "" ? "" : " AND "));
                                toReturn += String.Format("{2}AR.`{0}` {1}", "NR_AUTO", op, (toReturn == "" ? "" : " AND "));
                                break;
                            case "societatecasco":
                            case "asiguratorcasco":
                                //toReturn += String.Format("{2}SC.`{0}` LIKE '%{1}%'", "DENUMIRE_SCURTA", piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "));
                                //toReturn += String.Format("{2}SC.`{0}` LIKE '{1}%'", "DENUMIRE_SCURTA", j.ToString(), (toReturn == "" ? "" : " AND "));
                                toReturn += String.Format("{2}SC.`{0}` {1}", "DENUMIRE_SCURTA", op, (toReturn == "" ? "" : " AND "));
                                break;
                            case "societaterca":
                            case "asiguratorrca":
                                //toReturn += String.Format("{2}SR.`{0}` LIKE '%{1}%'", "DENUMIRE_SCURTA", piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "));
                                //toReturn += String.Format("{2}SR.`{0}` LIKE '{1}%'", "DENUMIRE_SCURTA", j.ToString(), (toReturn == "" ? "" : " AND "));
                                toReturn += String.Format("{2}SR.`{0}` {1}", "DENUMIRE_SCURTA", op, (toReturn == "" ? "" : " AND "));
                                break;
                            case "intervenient":
                                //toReturn += String.Format("{2}I.`{0}` LIKE '%{1}%'", "DENUMIRE", piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "));
                                //toReturn += String.Format("{2}I.`{0}` LIKE '{1}%'", "DENUMIRE", j.ToString(), (toReturn == "" ? "" : " AND "));
                                toReturn += String.Format("{2}I.`{0}` {1}", "DENUMIRE", op, (toReturn == "" ? "" : " AND "));
                                break;
                            case "tipdosar":
                                //toReturn += String.Format("{2}TD.`{0}` LIKE '%{1}%'", "DENUMIRE", piJObj.GetValue(jObj, null), (toReturn == "" ? "" : " AND "));
                                //toReturn += String.Format("{2}TD.`{0}` LIKE '{1}%'", "DENUMIRE", j.ToString(), (toReturn == "" ? "" : " AND "));
                                toReturn += String.Format("{2}TD.`{0}` {1}", "DENUMIRE", op, (toReturn == "" ? "" : " AND "));
                                break;
                        }
                    }
                }
                /*
                MethodInfo mi = x.GetType().GetMethod("ValidareColoane");
                response res = (response)mi.Invoke(x, new object[] { _filter });
                if (res.Status)
                {
                    mi = x.GetType().GetMethod("GenerateFilterFromJsonObject");
                    var r = mi.Invoke(x, null);
                    toReturn = r.ToString();
                }
                */
            }
            catch { }
            return toReturn;
        }

        public static string GenerateFilterFromJsonObject(object item)
        {
            string toReturn = "";
            string className = item.GetType().Name;
            string tableAllias = CommonFunctions.ClassNamesTableNamesAlliases[className].ToUpper();

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
                        switch (propType)
                        {
                            case "System.DateTime":
                                toReturn += String.Format("{2}{3}.`{0}` = '{1}'", propName, propValue, (toReturn == "" ? "" : " AND "), tableAllias);
                                break;
                            default:
                                toReturn += String.Format("{2}{3}.`{0}` like '{1}%'", propName, propValue, (toReturn == "" ? "" : " AND "), tableAllias);
                                break;
                        }
                    }
                    catch { }
                }
            }
            return toReturn;
        }

        #region -- older --
        /*
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

        public static string GenerateSimpleFilter(string _filtru, MySqlDataReader _table)
        {
            string _strFilter = "";
            int column_counter = 0;
            int counter = CommonFunctions.GetDbReaderRowCount(_table);
            
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

        public static string GenerateFilterFromJson(string _filtru, MySqlDataReader _table)
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
                            _tmpVal = CommonFunctions.ToMySqlFormatDate(CommonFunctions.SwitchBackFormatedDate(_tmpVal));
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

        public static bool Like(this string str, string pattern)
        {
            return new Regex(
                "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            ).IsMatch(str);
        }
        */
        #endregion
    }
}
