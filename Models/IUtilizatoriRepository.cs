using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SOCISA.Models
{
    public interface IUtilizatoriRepository
    {
        response GetAll(); response CountAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response GetFiltered(string _json);
        response GetFiltered(JObject _json);

        response Find(int _id);
        response Insert(Utilizator action);
        response Update(Utilizator item);
        response Delete(Utilizator item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response HasChildrens(Utilizator item, string tableName);
        response HasChildren(Utilizator item, string tableName, int childrenId);
        response GetChildrens(Utilizator item, string tableName);
        response GetChildren(Utilizator item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
        response Login(string user_name, string password);
        response Login(string user_name, string password, string ip);
        response Find(string email);
    }

    public class UtilizatoriRepository : IUtilizatoriRepository
    {
        private string connectionString;
        private int? authenticatedUserId;

        public UtilizatoriRepository(int? _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(Convert.ToInt32(authenticatedUserId), connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Utilizator[] toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Utilizator)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response CountAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_count");
                object count = da.ExecuteScalarQuery().Result;
                if (count == null)
                    return new response(true, "0", 0, null, null);
                return new response(true, count.ToString(), Convert.ToInt32(count), null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }
        public response GetFiltered(string _json)
        {
            JObject jObj = JObject.Parse(_json);
            return GetFiltered(jObj);
        }

        public response GetFiltered(JObject _json)
        {
            try
            {
                Filter f = new Filter();
                foreach (var t in _json)
                {
                    JToken j = t.Value;
                    switch (t.Key.ToLower())
                    {
                        case "sort":
                            f.Sort = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j);
                            break;
                        case "order":
                            f.Order = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j);
                            break;
                        case "filter":
                            f.Filtru = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j);
                            break;
                        case "limit":
                            f.Limit = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j);
                            break;
                    }
                }
                return GetFiltered(f.Sort, f.Order, f.Filtru, f.Limit);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }
        public response GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(Utilizator), _filter, Convert.ToInt32(authenticatedUserId), connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(Convert.ToInt32(authenticatedUserId), connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Utilizator[] toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Utilizator)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }


        public response Find(int _id)
        {
            try
            {
                Utilizator item = new Utilizator(Convert.ToInt32(authenticatedUserId), connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(Utilizator item)
        {
            return item.Insert();
        }

        public response Update(Utilizator item)
        {
            return item.Update();
        }

        public response Delete(Utilizator item)
        {
            return item.Delete();
        }

        public response Update(int id, string fieldValueCollection)
        {
            //Utilizator item = JsonConvert.DeserializeObject<Utilizator>(Find(id).Message);
            Utilizator item = (Utilizator)(Find(id).Result);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            Utilizator tmpItem = JsonConvert.DeserializeObject<Utilizator>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<Utilizator>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((Utilizator)(Find(Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
        }

        public response HasChildrens(Utilizator item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(Utilizator item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(Utilizator item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(Utilizator item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }
        public response Delete(int _id)
        {
            response obj = Find(_id);
            //return JsonConvert.DeserializeObject<Utilizator>(obj.Message).Delete();
            return ((Utilizator)obj.Result).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Utilizator>(obj.Message).HasChildrens(tableName);
            return ((Utilizator)obj.Result).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Utilizator>(obj.Message).HasChildren(tableName, childrenId);
            return ((Utilizator)obj.Result).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Utilizator>(obj.Message).GetChildrens(tableName);
            return ((Utilizator)obj.Result).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Utilizator>(obj.Message).GetChildren(tableName, childrenId);
            return ((Utilizator)obj.Result).GetChildren(tableName, childrenId);
        }

        public response Login(string user_name, string password, string ip)
        {
            try
            {
                Utilizator u = null;
                // singura metoda, impreuna cu Find(email), care nu foloseste DataAccess pt. ca nu avem authenticatedUserId
                MD5 md5h = MD5.Create();
                string md5p = CommonFunctions.GetMd5Hash(md5h, password);
                MySqlConnection con = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LOGINsp";
                cmd.Parameters.Add(new MySqlParameter("_username", user_name));
                cmd.Parameters.Add(new MySqlParameter("_password", md5p));
                cmd.Parameters.Add(new MySqlParameter("_ip", ip));
                con.Open();
                MySqlDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (r.Read())
                {
                    authenticatedUserId = Convert.ToInt32(r["ID"]);
                    u = new Utilizator(Convert.ToInt32(authenticatedUserId), connectionString, r);
                    break;
                }
                r.Close(); r.Dispose();
                if(u != null)
                    return new response(true, JsonConvert.SerializeObject(u), u, null, null);

                Error err = ErrorParser.ErrorMessage("unauthorisedUser");
                return new response(true, err.ERROR_MESSAGE, null, null, new System.Collections.Generic.List<Error>() { err });
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Login(string user_name, string password)
        {
            return Login(user_name, password, null);
        }

        public response Find(string email)
        {
            try
            {
                // singura metoda, impreuna cu Login, care nu foloseste DataAccess pt. ca nu avem authenticatedUserId
                Utilizator u = null;
                MySqlConnection con = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UTILIZATORIsp_GetByEmail";
                cmd.Parameters.Add(new MySqlParameter("_EMAIL", email));
                con.Open();
                MySqlDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (r.Read())
                {
                    authenticatedUserId = Convert.ToInt32(r["ID"]);
                    u = new Utilizator(Convert.ToInt32(authenticatedUserId), connectionString, r);
                    break;
                }
                r.Close();r.Dispose();
                if(u != null)
                    return new response(true, JsonConvert.SerializeObject(u), u, null, null);

                Error err = ErrorParser.ErrorMessage("unauthorisedUser");
                return new response(false, err.ERROR_MESSAGE, null, null, new List<Error>() { err });
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }
    }
}
