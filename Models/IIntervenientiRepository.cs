using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface IIntervenientiRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response Find(int _id);
        response Insert(Intervenient item);
        response Update(Intervenient item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response Delete(Intervenient item);
        response HasChildrens(Intervenient item, string tableName);
        response HasChildren(Intervenient item, string tableName, int childrenId);
        response GetChildrens(Intervenient item, string tableName);
        response GetChildren(Intervenient item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
    }

    public class IntervenientiRepository : IIntervenientiRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public IntervenientiRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "INTERVENIENTISsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Intervenient a = new Intervenient(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Intervenient[] toReturn = new Intervenient[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Intervenient)aList[i];
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
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(Intervenient), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "INTERVENIENTISsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Intervenient a = new Intervenient(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Intervenient[] toReturn = new Intervenient[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Intervenient)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                Intervenient item = new Intervenient(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(Intervenient item)
        {
            return item.Insert();
        }

        public response Update(Intervenient item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            //Intervenient item = JsonConvert.DeserializeObject<Intervenient>(Find(id).Message);
            Intervenient item = (Intervenient)(Find(id).Result);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            Intervenient tmpItem = JsonConvert.DeserializeObject<Intervenient>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<Intervenient>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((Intervenient)(Find(Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
        }

        public response Delete(Intervenient item)
        {
            return item.Delete();
        }

        public response HasChildrens(Intervenient item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(Intervenient item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(Intervenient item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(Intervenient item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }
        public response Delete(int _id)
        {
            response obj = Find(_id);
            //return JsonConvert.DeserializeObject<Intervenient>(obj.Message).Delete();
            return ((Intervenient)obj.Result).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Intervenient>(obj.Message).HasChildrens(tableName);
            return ((Intervenient)obj.Result).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Intervenient>(obj.Message).HasChildren(tableName, childrenId);
            return ((Intervenient)obj.Result).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Intervenient>(obj.Message).GetChildrens(tableName);
            return ((Intervenient)obj.Result).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<Intervenient>(obj.Message).GetChildren(tableName, childrenId);
            return ((Intervenient)obj.Result).GetChildren(tableName, childrenId);
        }
    }
}
