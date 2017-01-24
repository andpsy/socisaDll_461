using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface IDosareProceseRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response Find(int _id);
        response Insert(DosarProces item);
        response Update(DosarProces item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response Delete(DosarProces item);
        response HasChildrens(DosarProces item, string tableName);
        response HasChildren(DosarProces item, string tableName, int childrenId);
        response GetChildrens(DosarProces item, string tableName);
        response GetChildren(DosarProces item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
    }

    public class DosareProceseRepository : IDosareProceseRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public DosareProceseRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_PROCESEsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarProces a = new DosarProces(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarProces[] toReturn = new DosarProces[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarProces)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(DosarProces), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_PROCESEsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarProces a = new DosarProces(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarProces[] toReturn = new DosarProces[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarProces)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                DosarProces item = new DosarProces(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(DosarProces item)
        {
            return item.Insert();
        }

        public response Update(DosarProces item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            DosarProces item = JsonConvert.DeserializeObject<DosarProces>(Find(id).Message);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            DosarProces tmpItem = JsonConvert.DeserializeObject<DosarProces>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            return JsonConvert.DeserializeObject<DosarProces>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
        }

        public response Delete(DosarProces item)
        {
            return item.Delete();
        }

        public response HasChildrens(DosarProces item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(DosarProces item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(DosarProces item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(DosarProces item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }
        public response Delete(int _id)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<DosarProces>(obj.Message).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<DosarProces>(obj.Message).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<DosarProces>(obj.Message).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<DosarProces>(obj.Message).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<DosarProces>(obj.Message).GetChildren(tableName, childrenId);
        }
    }
}

