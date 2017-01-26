using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface IDosareStadiiSentinteRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response Find(int _id);
        response Insert(DosarStadiuSentinta item);
        response Update(DosarStadiuSentinta item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response Delete(DosarStadiuSentinta item);
        response HasChildrens(DosarStadiuSentinta item, string tableName);
        response HasChildren(DosarStadiuSentinta item, string tableName, int childrenId);
        response GetChildrens(DosarStadiuSentinta item, string tableName);
        response GetChildren(DosarStadiuSentinta item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
    }

    public class DosareStadiiSentinteRepository : IDosareStadiiSentinteRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public DosareStadiiSentinteRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADII_SENTINTEsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarStadiuSentinta a = new DosarStadiuSentinta(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarStadiuSentinta[] toReturn = new DosarStadiuSentinta[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarStadiuSentinta)aList[i];
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
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(DosarStadiuSentinta), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADII_SENTINTEsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarStadiuSentinta a = new DosarStadiuSentinta(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarStadiuSentinta[] toReturn = new DosarStadiuSentinta[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarStadiuSentinta)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                DosarStadiuSentinta item = new DosarStadiuSentinta(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(DosarStadiuSentinta item)
        {
            return item.Insert();
        }

        public response Update(DosarStadiuSentinta item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            //DosarStadiuSentinta item = JsonConvert.DeserializeObject<DosarStadiuSentinta>(Find(id).Message);
            DosarStadiuSentinta item = (DosarStadiuSentinta)(Find(id).Result);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            DosarStadiuSentinta tmpItem = JsonConvert.DeserializeObject<DosarStadiuSentinta>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<DosarStadiuSentinta>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((DosarStadiuSentinta)(Find(Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
        }

        public response Delete(DosarStadiuSentinta item)
        {
            return item.Delete();
        }

        public response HasChildrens(DosarStadiuSentinta item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(DosarStadiuSentinta item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(DosarStadiuSentinta item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(DosarStadiuSentinta item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }
        public response Delete(int _id)
        {
            response obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiuSentinta>(obj.Message).Delete();
            return ((DosarStadiuSentinta)obj.Result).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiuSentinta>(obj.Message).HasChildrens(tableName);
            return ((DosarStadiuSentinta)obj.Result).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiuSentinta>(obj.Message).HasChildren(tableName, childrenId);
            return ((DosarStadiuSentinta)obj.Result).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiuSentinta>(obj.Message).GetChildrens(tableName);
            return ((DosarStadiuSentinta)obj.Result).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<DosarStadiuSentinta>(obj.Message).GetChildren(tableName, childrenId);
            return ((DosarStadiuSentinta)obj.Result).GetChildren(tableName, childrenId);
        }
    }
}
