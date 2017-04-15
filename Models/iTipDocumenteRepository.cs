using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
//using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SOCISA.Models
{
    public interface ITipDocumenteRepository
    {
        //Action[] GetAll();
        response GetAll(); response CountAll();
        //Action[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response GetFiltered(string _json);
        response GetFiltered(JObject _json);

        //TipDocument Find(int _id);
        response Find(int _id);
        response Insert(TipDocument item);
        response Update(TipDocument item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response Delete(TipDocument item);
        //bool HasChildrens(TipDocument item, string tableName);
        response HasChildrens(TipDocument item, string tableName);
        //bool HasChildren(TipDocument item, string tableName, int childrenId);
        response HasChildren(TipDocument item, string tableName, int childrenId);
        //object[] GetChildrens(TipDocument item, string tableName);
        response GetChildrens(TipDocument item, string tableName);
        //object GetChildren(TipDocument item, string tableName, int childrenId);
        response GetChildren(TipDocument item, string tableName, int childrenId);

        response Delete(int _id);
        //bool HasChildrens(int _id, string tableName);
        response HasChildrens(int _id, string tableName);
        //bool HasChildren(int _id, string tableName, int childrenId);
        response HasChildren(int _id, string tableName, int childrenId);
        //object[] GetChildrens(int _id, string tableName);
        response GetChildrens(int _id, string tableName);
        //object GetChildren(int _id, string tableName, int childrenId);
        response GetChildren(int _id, string tableName, int childrenId);
    }

    public class TipDocumenteRepository : ITipDocumenteRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public TipDocumenteRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TIP_DOCUMENTsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    TipDocument a = new TipDocument(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                TipDocument[] toReturn = new TipDocument[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (TipDocument)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }
        public response CountAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TIP_DOCUMENTsp_count");
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
                            f.Sort = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j, CommonFunctions.JsonSerializerSettings);
                            break;
                        case "order":
                            f.Order = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j, CommonFunctions.JsonSerializerSettings);
                            break;
                        case "filter":
                            f.Filtru = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j, CommonFunctions.JsonSerializerSettings);
                            break;
                        case "limit":
                            f.Limit = CommonFunctions.IsNullOrEmpty(j) ? null : JsonConvert.SerializeObject(j, CommonFunctions.JsonSerializerSettings);
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
                #region -- initial --
                /*
                string newFilter = null;
                try
                {
                    try
                    {
                        Action x = Newtonsoft.Json.JsonConvert.DeserializeObject<Action>(_filter);
                        newFilter = x.GenerateFilterFromJsonObject();
                    }
                    catch
                    {
                        try
                        {
                            dynamic jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(_filter);
                            Action x = new Action(authenticatedUserId, connectionString);
                            PropertyInfo[] pisX = x.GetType().GetProperties();
                            PropertyInfo[] pisJObj = jObj.GetType().GetProperties();
                            foreach (PropertyInfo piX in pisX)
                            {
                                foreach (PropertyInfo piJObj in pisJObj)
                                {
                                    if (piX.Name == piJObj.Name)
                                    {
                                        piX.SetValue(x, piJObj.GetValue(jObj));
                                        break;
                                    }
                                }
                            }
                            newFilter = x.GenerateFilterFromJsonObject();
                        }
                        catch { }
                    }
                }
                catch { }
                if (newFilter != null) _filter = newFilter;
                */
                #endregion
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(TipDocument), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "TIP_DOCUMENTsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    TipDocument a = new TipDocument(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                TipDocument[] toReturn = new TipDocument[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (TipDocument)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                TipDocument item = new TipDocument(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item, CommonFunctions.JsonSerializerSettings), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(TipDocument item)
        {
            return item.Insert();
        }

        public response Update(TipDocument item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            //TipDocument item = JsonConvert.DeserializeObject<TipDocument>(Find(id).Message);
            TipDocument item = (TipDocument)(Find(id).Result);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            TipDocument tmpItem = JsonConvert.DeserializeObject<TipDocument>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<TipDocument>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((TipDocument)(Find(Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
        }
        public response Delete(TipDocument item)
        {
            return item.Delete();
        }

        public response HasChildrens(TipDocument item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(TipDocument item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(TipDocument item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(TipDocument item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }

        public response Delete(int _id)
        {
            response obj = Find(_id);
            //return JsonConvert.DeserializeObject<TipDocument>(obj.Message).Delete();
            return ((TipDocument)obj.Result).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<TipDocument>(obj.Message).HasChildrens(tableName);
            return ((TipDocument)obj.Result).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Action>(obj.Message).HasChildren(tableName, childrenId);
            return ((TipDocument)obj.Result).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<TipDocument>(obj.Message).GetChildrens(tableName);
            return ((TipDocument)obj.Result).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<TipDocument>(obj.Message).GetChildren(tableName, childrenId);
            return ((TipDocument)obj.Result).GetChildren(tableName, childrenId);
        }
    }
}
