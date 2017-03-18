using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SOCISA.Models
{
    public interface IProceseRepository
    {
        response GetAll(); response CountAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response GetFiltered(string _json);
        response GetFiltered(JObject _json);
        response Find(int _id);
        response Insert(Proces item);
        response Insert(Proces item, int _ID_DOSAR);
        response Update(Proces item);
        response Update(Proces item, int _ID_DOSAR);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response Delete(Proces item);
        response Delete(Proces item, int _ID_DOSAR);
        response HasChildrens(Proces item, string tableName);
        response HasChildren(Proces item, string tableName, int childrenId);
        response GetChildrens(Proces item, string tableName);
        response GetChildren(Proces item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
        response GetInstanta(Proces item);
        response GetComplet(Proces item);
        response GetTipProces(Proces item);
    }

    public class ProceseRepository : IProceseRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public ProceseRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "PROCESEsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Proces a = new Proces(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Proces[] toReturn = new Proces[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Proces)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response CountAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "PROCESEsp_count");
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
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(Proces), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "PROCESEsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Proces a = new Proces(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Proces[] toReturn = new Proces[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Proces)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                Proces item = new Proces(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(Proces item)
        {
            return item.Insert();
        }

        public response Insert(Proces item, int _ID_DOSAR)
        {
            return item.Insert(_ID_DOSAR);
        }

        public response Update(Proces item)
        {
            return item.Update();
        }

        public response Update(Proces item, int _ID_DOSAR)
        {
            return item.Update(_ID_DOSAR);
        }

        public response Update(int id, string fieldValueCollection)
        {
            //Proces item = JsonConvert.DeserializeObject<Proces>(Find(id).Message);
            Proces item = (Proces)(Find(id).Result);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            Proces tmpItem = JsonConvert.DeserializeObject<Proces>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<Proces>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((Proces)(Find(Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
        }

        public response Delete(Proces item)
        {
            return item.Delete();
        }

        public response Delete(Proces item, int _ID_DOSAR)
        {
            return item.Delete(_ID_DOSAR);
        }

        public response HasChildrens(Proces item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(Proces item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(Proces item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(Proces item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }

        public response GetInstanta(Proces item)
        {
            return item.GetInstanta();
        }
        public response GetComplet(Proces item)
        {
            return item.GetComplet();
        }
        public response GetTipProces(Proces item)
        {
            return item.GetTipProces();
        }
        public response Delete(int _id)
        {
            response obj = Find(_id);
            //return JsonConvert.DeserializeObject<Proces>(obj.Message).Delete();
            return ((Proces)obj.Result).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Proces>(obj.Message).HasChildrens(tableName);
            return ((Proces)obj.Result).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Proces>(obj.Message).HasChildren(tableName, childrenId);
            return ((Proces)obj.Result).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Proces>(obj.Message).GetChildrens(tableName);
            return ((Proces)obj.Result).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Proces>(obj.Message).GetChildren(tableName, childrenId);
            return ((Proces)obj.Result).GetChildren(tableName, childrenId);
        }
    }
}
