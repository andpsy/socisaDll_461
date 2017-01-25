using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface IUtilizatoriRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
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
    }

    public class UtilizatoriRepository : IUtilizatoriRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public UtilizatoriRepository(int _authenticatedUserId, string _connectionString)
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
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Utilizator[] toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Utilizator)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(Utilizator), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
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
                Utilizator item = new Utilizator(authenticatedUserId, connectionString, _id);
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
            Utilizator item = JsonConvert.DeserializeObject<Utilizator>(Find(id).Message);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            Utilizator tmpItem = JsonConvert.DeserializeObject<Utilizator>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            return JsonConvert.DeserializeObject<Utilizator>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
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
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Utilizator>(obj.Message).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Utilizator>(obj.Message).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Utilizator>(obj.Message).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Utilizator>(obj.Message).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<Utilizator>(obj.Message).GetChildren(tableName, childrenId);
        }
    }
}
