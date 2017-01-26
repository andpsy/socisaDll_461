using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface IDosareStadiiRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response Find(int _id);
        response Insert(DosarStadiu item);
        response Update(DosarStadiu item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response Delete(DosarStadiu item);
        response HasChildrens(DosarStadiu item, string tableName);
        response HasChildren(DosarStadiu item, string tableName, int childrenId);
        response GetChildrens(DosarStadiu item, string tableName);
        response GetChildren(DosarStadiu item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
    }

    public class DosareStadiiRepository : IDosareStadiiRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public DosareStadiiRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarStadiu a = new DosarStadiu(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarStadiu[] toReturn = new DosarStadiu[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarStadiu)aList[i];
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
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(DosarStadiu), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarStadiu a = new DosarStadiu(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarStadiu[] toReturn = new DosarStadiu[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarStadiu)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                DosarStadiu item = new DosarStadiu(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(DosarStadiu item)
        {
            return item.Insert();
        }

        public response Update(DosarStadiu item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            //DosarStadiu item = JsonConvert.DeserializeObject<DosarStadiu>(Find(id).Message);
            DosarStadiu item = (DosarStadiu)(Find(id).Result);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            DosarStadiu tmpItem = JsonConvert.DeserializeObject<DosarStadiu>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<DosarStadiu>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((DosarStadiu)(Find(Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
        }

        public response Delete(DosarStadiu item)
        {
            return item.Delete();
        }

        public response HasChildrens(DosarStadiu item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(DosarStadiu item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(DosarStadiu item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(DosarStadiu item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }
        public response Delete(int _id)
        {
            response obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiu>(obj.Message).Delete();
            return ((DosarStadiu)obj.Result).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiu>(obj.Message).HasChildrens(tableName);
            return ((DosarStadiu)obj.Result).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiu>(obj.Message).HasChildren(tableName, childrenId);
            return ((DosarStadiu)obj.Result).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiu>(obj.Message).GetChildrens(tableName);
            return ((DosarStadiu)obj.Result).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiu>(obj.Message).GetChildren(tableName, childrenId);
            return ((DosarStadiu)obj.Result).GetChildren(tableName, childrenId);
        }
    }
}
