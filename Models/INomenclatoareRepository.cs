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
    public interface INomenclatoareRepository
    {
        response GetAll(string tableName); response CountAll(string tableNazme);
        response GetFiltered(string tableName, string _sort, string _order, string _filter, string _limit);
        response GetFiltered(string tableName, string _json);
        response GetFiltered(string tableName, JObject _json);

        response Find(string tableName, int _id);
        response Find(string tableName, string _denumire);
        response Insert(Nomenclator item);
        response Update(Nomenclator item);
        response Update(string tableName, int id, string fieldValueCollection);
        response Update(string tableName, string fieldValueCollection);

        response Delete(Nomenclator item);
        response HasChildrens(Nomenclator item, string tableName);
        response HasChildren(Nomenclator item, string tableName, int childrenId);
        response GetChildrens(Nomenclator item, string tableName);
        response GetChildren(Nomenclator item, string tableName, int childrenId);
        int? GetIdByName(Nomenclator item, string tableName, string denumire);
        response Delete(string tableName, int _id);
        response HasChildrens(string tableName, int _id, string childTableName);
        response HasChildren(string tableName, int _id, string childTableName, int childrenId);
        response GetChildrens(string tableName, int _id, string childTableName);
        response GetChildren(string tableName, int _id, string childTableName, int childrenId);
    }

    public class NomenclatoareRepository : INomenclatoareRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public NomenclatoareRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll(string tableName)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format("{0}sp_select", tableName.ToUpper()), new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Nomenclator a = new Nomenclator(authenticatedUserId, connectionString, tableName, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Nomenclator[] toReturn = new Nomenclator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Nomenclator)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response CountAll(string tableName)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format("{0}sp_count", tableName));
                object count = da.ExecuteScalarQuery().Result;
                if (count == null)
                    return new response(true, "0", 0, null, null);
                return new response(true, count.ToString(), Convert.ToInt32(count), null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }
        public response GetFiltered(string _tableName, string _json)
        {
            JObject jObj = JObject.Parse(_json);
            return GetFiltered(_tableName, jObj);
        }

        public response GetFiltered(string _tableName, JObject _json)
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
                return GetFiltered(_tableName, f.Sort, f.Order, f.Filtru, f.Limit);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }
        public response GetFiltered(string tableName, string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(Nomenclator), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format("{0}sp_select", tableName.ToUpper()), new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                MySqlDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Nomenclator a = new Nomenclator(authenticatedUserId, connectionString, tableName, (IDataRecord)r);
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Nomenclator[] toReturn = new Nomenclator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Nomenclator)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(string tableName, int _id)
        {
            try
            {
                Nomenclator item = new Nomenclator(authenticatedUserId, connectionString, tableName, _id);
                return new response(true, JsonConvert.SerializeObject(item, CommonFunctions.JsonSerializerSettings), item, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }
        public response Find(string tableName, string _denumire)
        {
            try
            {
                Nomenclator item = new Nomenclator(authenticatedUserId, connectionString, tableName, _denumire);
                return new response(true, JsonConvert.SerializeObject(item, CommonFunctions.JsonSerializerSettings), item, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(Nomenclator item)
        {
            return item.Insert();
        }

        public response Update(Nomenclator item)
        {
            return item.Update();
        }

        public response Update(string tableName, int id, string fieldValueCollection)
        {
            //Nomenclator item = JsonConvert.DeserializeObject<Nomenclator>(Find(tableName, id).Message);
            Nomenclator item = (Nomenclator)(Find(tableName, id).Result);
            return item.Update(fieldValueCollection);
        }
        public response Update(string tableName, string fieldValueCollection)
        {
            Nomenclator tmpItem = JsonConvert.DeserializeObject<Nomenclator>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<Nomenclator>(Find(tableName, Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((Nomenclator)(Find(tableName, Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
        }

        public response Delete(Nomenclator item)
        {
            return item.Delete();
        }

        public response HasChildrens(Nomenclator item, string childTableName)
        {
            return item.HasChildrens(childTableName);
        }

        public response HasChildren(Nomenclator item, string childTableName, int childrenId)
        {
            return item.HasChildren(childTableName, childrenId);
        }

        public response GetChildrens(Nomenclator item, string childTableName)
        {
            return item.GetChildrens(childTableName);
        }

        public response GetChildren(Nomenclator item, string childTableName, int childrenId)
        {
            return item.GetChildren(childTableName, childrenId);
        }

        public int? GetIdByName(Nomenclator item, string tableName, string denumire)
        {
            return item.GetIdByName(tableName, denumire);
        }
        public response Delete(string tableName, int _id)
        {
            var obj = Find(tableName, _id);
            //return JsonConvert.DeserializeObject<Nomenclator>(obj.Message).Delete();
            return ((Nomenclator)(obj.Result)).Delete();
        }

        public response HasChildrens(string tableName, int _id, string childTableName)
        {
            var obj = Find(tableName, _id);
            //return JsonConvert.DeserializeObject<Nomenclator>(obj.Message).HasChildrens(childTableName);
            return ((Nomenclator)(obj.Result)).HasChildrens(childTableName);
        }
        public response HasChildren(string tableName, int _id, string childTableName, int childrenId)
        {
            var obj = Find(tableName, _id);
            //return JsonConvert.DeserializeObject<Nomenclator>(obj.Message).HasChildren(childTableName, childrenId);
            return ((Nomenclator)(obj.Result)).HasChildren(childTableName, childrenId);
        }
        public response GetChildrens(string tableName, int _id, string childTableName)
        {
            var obj = Find(tableName, _id);
            //return JsonConvert.DeserializeObject<Nomenclator>(obj.Message).GetChildrens(childTableName);
            return ((Nomenclator)(obj.Result)).GetChildrens(childTableName);
        }
        public response GetChildren(string tableName, int _id, string childTableName, int childrenId)
        {
            var obj = Find(tableName, _id);
            //return JsonConvert.DeserializeObject<Nomenclator>(obj.Message).GetChildren(childTableName, childrenId);
            return ((Nomenclator)(obj.Result)).GetChildren(childTableName, childrenId);
        }
    }
}
