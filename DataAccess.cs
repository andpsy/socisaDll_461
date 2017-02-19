using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SOCISA
{
    /// <summary>
    /// Status: true = success, false = error
    /// Message: "" if success, Error message if error
    /// InsertedId: id-ul generat la insert if succes, null if error
    /// </summary>
    public class response {
        /// <summary>
        /// Constructorul default
        /// </summary>
        public response()
        {
            Status = true;
            Message = "";
            Result = null;
            InsertedId = null;
            Error = null;
        }

        /// <summary>
        /// Constructorul pentru initializarea responsului cu parametri primiti
        /// </summary>
        /// <param name="_status">Statusul operatiei efectuate: true = Succes, false = Eroare</param>
        /// <param name="_message">Mesajul rezultat in urma operatiei: "" pentru Succes, Mesaj pentru Eroare</param>
        /// <param name="_inserted_id">ID-ul unic rezultat in cazul operatiilor de insert, null pentru celelalte</param>
        /// <param name="_error">SOCISA.Error in caz de eroare, null pentru Succes</param>
        public response(bool _status, string _message, object _result, int? _inserted_id, List<Error> _error)
        {
            Status = _status;
            Message = _message;
            Result = _result;
            InsertedId = _inserted_id;
            Error = _error;
        }

        /// <summary>
        /// Statusul operatiei efectuate: true = Succes, false = Eroare
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Mesajul rezultat in urma operatiei: "" pentru Succes, Mesaj pentru Eroare
        /// </summary>
        public string Message { get; set; }

        public object Result { get; set; }
        /// <summary>
        /// ID-ul unic rezultat in cazul operatiilor de insert, null pentru celelalte
        /// </summary>
        public int? InsertedId { get; set; }

        /// <summary>
        /// SOCISA.Error in caz de eroare, null pentru Succes
        /// </summary>
        public List<Error> Error { get; set; }

        public void AddResponse(response _toAdd)
        {
            this.Status = _toAdd.Status;
            this.Message = String.Format("{0};{1}", this.Message == null ? "" : this.Message, _toAdd.Message == null ? "" : _toAdd.Message);
            foreach (Error err in _toAdd.Error)
            {
                this.Error.Add(err);
            }
            if (_toAdd.InsertedId != null)
                this.InsertedId = _toAdd.InsertedId;
        }
    }

    /// <summary>
    /// Clasa pentru manipularea datelor
    /// </summary>
    public class DataAccess
    {
        readonly string ConnectionString;
        readonly MySqlConnection mySqlConnection = new MySqlConnection();
        readonly MySqlCommand mySqlCommand = new MySqlCommand();
        int _id_utilizator;
        /// <summary>
        /// ID-ul unic al utilizatorului logat, folosit pt. logarea actiunilor
        /// </summary>
        int ID_UTILIZATOR
        {
            get { return _id_utilizator; }
            set { _id_utilizator = value; }
        }

        /// <summary>
        /// Constructorul default
        /// </summary>
        public DataAccess()
        {
        }

        /// <summary>
        /// Constructor pentru initializarea tipului de acces la baza de date inclusiv cu logarea utilizatorului autentificat
        /// </summary>
        /// <param name="_authenticated_user_id">ID-ul unic al utilizatorului autentificat</param>
        /// <param name="_connectionString">Datele de logare la BD</param>
        /// <param name="_commandType">Tipul comenzii (procedura stocata, text script, etc.)</param>
        /// <param name="_commandText">Comanda propriu-zisa (Numele procedurii stocate, scriptul de executat, etc.)</param>
        public DataAccess(object _authenticated_user_id, string _connectionString, CommandType _commandType, string _commandText)
        {
            try
            {
                ID_UTILIZATOR = Convert.ToInt32(_authenticated_user_id);
                ConnectionString = _connectionString;
                mySqlConnection.ConnectionString = _connectionString;
                mySqlCommand.Connection = mySqlConnection;
                mySqlCommand.CommandType = _commandType;
                mySqlCommand.CommandText = _commandText;
                MySqlParameter _AUTHENTICATED_USER_ID = new MySqlParameter("_AUTHENTICATED_USER_ID", _authenticated_user_id);
                mySqlCommand.Parameters.Add(_AUTHENTICATED_USER_ID);
                //mySqlConnection.Open();
            }catch(Exception exp) { throw exp; }
        }

        /// <summary>
        /// Constructor pentru initializarea tipului de acces la baza de date cu diversi parametri
        /// </summary>
        /// <param name="_authenticated_user_id">ID-ul unic al utilizatorului autentificat</param>
        /// <param name="_connectionString">Datele de logare la BD</param>
        /// <param name="_commandType">Tipul comenzii (procedura stocata, text script, etc.)</param>
        /// <param name="_commandText">Comanda propriu-zisa (Numele procedurii stocate, scriptul de executat, etc.)</param>
        /// <param name="_commandParameters">vector de MySqlParameter</param>
        public DataAccess(object _authenticated_user_id, string _connectionString, CommandType _commandType, string _commandText, object[] _commandParameters)
        {
            try
            {
                ID_UTILIZATOR = Convert.ToInt32(_authenticated_user_id);
                ConnectionString = _connectionString;
                mySqlConnection.ConnectionString = _connectionString;
                mySqlCommand.Connection = mySqlConnection;
                mySqlCommand.CommandType = _commandType;
                mySqlCommand.CommandText = _commandText;
                MySqlParameter _AUTHENTICATED_USER_ID = new MySqlParameter("_AUTHENTICATED_USER_ID", _authenticated_user_id);
                mySqlCommand.Parameters.Add(_AUTHENTICATED_USER_ID);
                foreach (MySqlParameter mySqlParameter in _commandParameters)
                {
                    mySqlCommand.Parameters.Add(mySqlParameter);
                }
                //mySqlConnection.Open();
            }
            catch (Exception exp) { throw exp; }
        }

        /// <summary>
        /// Metoda pt. selectia inregistrarilor din BD
        /// </summary>
        /// <returns>MySqlDataReader</returns>
        public MySqlDataReader ExecuteSelectQuery()
        {
            MySqlDataReader ds;
            try
            {
                mySqlConnection.Open();
                ds = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                //mySqlConnection.Close();
            }
            catch (Exception exp) { throw exp; }

            return ds;
        }

        /// <summary>
        /// Metoda pentru executarea operatiei de stergere a unei inregistrari din baza de date
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response ExecuteDeleteQuery()
        {
            try
            {
                mySqlConnection.Open();
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                // LOG INFO --
                try
                {
                    string action = "";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("insert") > 0) action = "INSERT";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("update") > 0) action = "UPDATE";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("delete") > 0) action = "DELETE";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("import") > 0) action = "IMPORT";
                    string table = action != "IMPORT" ? mySqlCommand.CommandText.ToUpper().Replace("SP_", "").Replace(action, "") : mySqlCommand.CommandText.ToUpper().Replace("SP", "").Replace("REGULARIMPORT", "");
                    string detalii_before = "";
                    if (mySqlCommand.Parameters.Contains("_ID") && mySqlCommand.Parameters["_ID"].Direction == ParameterDirection.Input && (action == "UPDATE" || action == "DELETE")) // pt. Update / Delete
                    {
                        try
                        {
                            detalii_before = GetDetaliiBefore(table, Convert.ToInt32(mySqlCommand.Parameters["_ID"].Value));
                        }
                        catch { detalii_before = ""; }
                    }
                    string detalii_after = "";
                    try
                    {
                        foreach (MySqlParameter mp in mySqlCommand.Parameters)
                            detalii_after += (mp.ParameterName + " = " + mp.Value.ToString() + ", ");
                    }
                    catch { }

                    SaveLog(DateTime.Now, action, table, detalii_before, detalii_after);
                }
                catch (Exception exp) { throw exp; }
                // END LOG ---
                return new response(true, "", null, null, new List<Error>());;
            }
            catch (Exception exp)
            {
                //mySqlConnection.Close();
                //return new response(false, SOCISA.CommonFunctions.MySqlErrorParser(mySqlException), null, null);
                return new response(false, exp.ToString(), null, null, new List<Error>() {new Error(exp) });
                //throw exp;
            }
        }

        /// <summary>
        /// Metoda pentru executarea operatiei de update a unei inregistrari din baza de date
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response ExecuteUpdateQuery()
        {
            try
            {
                mySqlConnection.Open();
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                // LOG INFO --
                try
                {
                    string action = "";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("insert") > 0) action = "INSERT";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("update") > 0) action = "UPDATE";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("delete") > 0) action = "DELETE";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("import") > 0) action = "IMPORT";
                    string table = action != "IMPORT" ? mySqlCommand.CommandText.ToUpper().Replace("SP_", "").Replace(action, "") : mySqlCommand.CommandText.ToUpper().Replace("SP", "").Replace("REGULARIMPORT", "");
                    string detalii_before = "";
                    if (mySqlCommand.Parameters.Contains("_ID"))
                    {
                        try
                        {
                            detalii_before = GetDetaliiBefore(table, Convert.ToInt32(mySqlCommand.Parameters["_ID"].Value));
                        }
                        catch { detalii_before = ""; }
                    }
                    string detalii_after = "";
                    try
                    {
                        foreach (MySqlParameter mp in mySqlCommand.Parameters)
                            detalii_after += (mp.ParameterName + " = " + mp.Value.ToString() + ", ");
                    }
                    catch { }

                    SaveLog(DateTime.Now, action, table, detalii_before, detalii_after);
                }
                catch (Exception exp) { throw exp; }
                // END LOG ---
                return new response(true, "", null, null, null);
            }
            catch (Exception exp)
            {
                //mySqlConnection.Close();
                //return new response(false, SOCISA.CommonFunctions.MySqlErrorParser(mySqlException), null, null);
                //throw exp;
                return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) });
            }
        }

        /// <summary>
        /// Metoda pentru executarea operatiei de adaugare a unei inregistrari din baza de date
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response ExecuteInsertQuery()
        {
            try
            {
                MySqlParameter _ID = new MySqlParameter();
                _ID.ParameterName = "_ID";
                _ID.DbType = DbType.Int32;
                _ID.Direction = ParameterDirection.Output;
                mySqlCommand.Parameters.Add(_ID);
                mySqlConnection.Open();
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                // LOG INFO --
                try
                {
                    string action = "";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("insert") > 0) action = "INSERT";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("update") > 0) action = "UPDATE";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("delete") > 0) action = "DELETE";
                    if (mySqlCommand.CommandText.ToLower().IndexOf("import") > 0) action = "IMPORT";
                    string table = action != "IMPORT" ? mySqlCommand.CommandText.ToUpper().Replace("SP_", "").Replace(action, "") : mySqlCommand.CommandText.ToUpper().Replace("SP", "").Replace("REGULARIMPORT", "");
                    string detalii_before = "";
                    if (mySqlCommand.Parameters.Contains("_ID") && mySqlCommand.Parameters["_ID"].Direction != ParameterDirection.Output)
                    {
                        try
                        {
                            detalii_before = GetDetaliiBefore(table, Convert.ToInt32(mySqlCommand.Parameters["_ID"].Value));
                        }
                        catch { detalii_before = ""; }
                    }
                    string detalii_after = "";
                    try
                    {
                        foreach (MySqlParameter mp in mySqlCommand.Parameters)
                            detalii_after += (mp.ParameterName + " = " + mp.Value.ToString() + ", ");
                    }
                    catch { }

                    SaveLog(DateTime.Now, action, table, detalii_before, detalii_after);
                }
                catch (Exception exp) { throw exp; }
                // END LOG ---
                return new response(true, "", Convert.ToInt32(mySqlCommand.Parameters["_ID"].Value), Convert.ToInt32(mySqlCommand.Parameters["_ID"].Value), null);
            }
            catch (Exception exp)
            {
                //mySqlConnection.Close();
                //return new response(false, SOCISA.CommonFunctions.MySqlErrorParser(mySqlException), null, null);
                //throw exp;
                return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) });
            }
        }

        /// <summary>
        /// Metoda pentru executarea operatiilor de selectie a unui scalar din baza de date
        /// </summary>
        /// <returns>obiect scalar rezultat din selectie</returns>
        public response ExecuteScalarQuery()
        {
            response toReturn = new response();
            try
            {
                mySqlConnection.Open();
                object ret = mySqlCommand.ExecuteScalar();
                toReturn = new response(true, "", ret, null, null);
                mySqlConnection.Close();
            }
            catch (Exception exp)
            {
                //mySqlException.ToString();
                //toReturn = null;
                //throw exp;
                toReturn = new response(false, exp.Message, null, null, new List<Error>() { new Error(exp) });
            }
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru generarea unui sir de parametri pentru operatiile de insert/update/delete
        /// </summary>
        /// <param name="_dt">Tabela in care se efectueaza operatia</param>
        /// <param name="_object">obiect cu valorile ce vor fi updatate</param>
        /// <returns>vector de MySqlParameters</returns>
        public object[] GenerateMySqlParameters(MySqlDataReader _dt, object[] _object)
        {
            ArrayList _alist = new ArrayList();
            System.Collections.ObjectModel.ReadOnlyCollection<DbColumn> _columns = _dt.GetColumnSchema();

            for (int i = 0; i < _columns.Count; i++)
            {
                string dcName = _columns[i].ColumnName;

                if (dcName.ToLower() != "id" && dcName.ToLower() != "extension")
                {
                    _alist.Add(new MySqlParameter("_" + dcName, _object[i].ToString()));
                }
            }
            return _alist.ToArray();
        }

        /// <summary>
        /// Metoda pentru logarea unei operatii de insert/update/delete
        /// </summary>
        /// <param name="_data">data operatiei</param>
        /// <param name="_actiune">actiune (insert/update/delete/import</param>
        /// <param name="_tabela">tabela in care se face operatia</param>
        /// <param name="_detalii_before">valorile din baza de date inainte de modificare</param>
        /// <param name="_detalii_after">valorile modificate de operatia efectuata</param>
        public void SaveLog(DateTime _data, string _actiune, string _tabela, string _detalii_before, string _detalii_after)
        {
            SaveLog(_data, _actiune, _tabela, _detalii_before, _detalii_after, ID_UTILIZATOR);
        }

        /// <summary>
        /// Metoda pentru logarea unei operatii de insert/update/delete inclusiv cu salvarea utilizatorului autentificat
        /// </summary>
        /// <param name="_data">data operatiei</param>
        /// <param name="_actiune">actiune (insert/update/delete/import</param>
        /// <param name="_tabela">tabela in care se face operatia</param>
        /// <param name="_detalii_before">valorile din baza de date inainte de modificare</param>
        /// <param name="_detalii_after">valorile modificate de operatia efectuata</param>
        /// <param name="_id_utilizator">ID-ul unic al utilizatorului autentificat</param>
        public void SaveLog(DateTime _data, string _actiune, string _tabela, string _detalii_before, string _detalii_after, int _id_utilizator)
        {
            try
            {
                MySqlConnection mc = new MySqlConnection();
                mc.ConnectionString = ConnectionString;
                MySqlCommand m = new MySqlCommand();
                m.Connection = mc;
                m.CommandType = CommandType.StoredProcedure;
                m.CommandText = "LOGsp_insert";
                MySqlParameter _DATA = new MySqlParameter("_DATA", _data);
                m.Parameters.Add(_DATA);
                MySqlParameter _ACTIUNE = new MySqlParameter("_ACTIUNE", _actiune);
                m.Parameters.Add(_ACTIUNE);
                MySqlParameter _TABELA = new MySqlParameter("_TABELA", _tabela);
                m.Parameters.Add(_TABELA);
                MySqlParameter _DETALII_BEFORE = new MySqlParameter("_DETALII_BEFORE", _detalii_before);
                m.Parameters.Add(_DETALII_BEFORE);
                MySqlParameter _DETALII_AFTER = new MySqlParameter("_DETALII_AFTER", _detalii_after);
                m.Parameters.Add(_DETALII_AFTER);
                MySqlParameter _ID_UTILIZATOR = new MySqlParameter("_ID_UTILIZATOR", _id_utilizator);
                m.Parameters.Add(_ID_UTILIZATOR);
                mc.Open();
                m.ExecuteNonQuery();
                mc.Close();
            }
            catch (Exception exp) { throw exp; }
        }

        /// <summary>
        /// Metoda pentru selectarea valorilor existente in baza de date inainte de efectuarea unei operatii de insert/update/delete/import
        /// </summary>
        /// <param name="_tabela">tabela in care urmeaza sa se efectueze operatia</param>
        /// <param name="_id">ID-ul uni al inregistrarii asupra careia se efectueaza operatia</param>
        /// <returns></returns>
        public string GetDetaliiBefore(string _tabela, int _id)
        {
            string toReturn = "";
            MySqlConnection mc = new MySqlConnection();
            mc.ConnectionString = ConnectionString;
            MySqlCommand m = new MySqlCommand();
            m.Connection = mySqlConnection;
            m.CommandType = CommandType.StoredProcedure;
            m.CommandText = _tabela.ToUpper() + "sp_GetById";
            MySqlParameter _AUTHENTICATED_USER_ID = new MySqlParameter("_AUTHENTICATED_USER_ID", this.ID_UTILIZATOR);
            m.Parameters.Add(_AUTHENTICATED_USER_ID);
            MySqlParameter _ID = new MySqlParameter("_ID", _id);
            m.Parameters.Add(_ID);
            mc.Open();

            MySqlDataReader mdr = m.ExecuteReader();
            System.Collections.ObjectModel.ReadOnlyCollection<DbColumn> _columns = mdr.GetColumnSchema();

            while (mdr.Read())
            {
                for (int i = 0; i < _columns.Count; i++)
                {
                    string dcName = _columns[i].ColumnName;
                    toReturn += (dcName.ToUpper() + " = " + mdr[dcName].ToString());
                }
                break;
            }
            mc.Close();

            return toReturn;
        }
    }
}