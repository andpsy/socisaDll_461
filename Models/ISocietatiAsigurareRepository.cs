using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface ISocietatiAsigurareRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response Find(int _id);
        response Insert(SocietateAsigurare item);
        response Update(SocietateAsigurare item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response Delete(SocietateAsigurare item);
        response HasChildrens(SocietateAsigurare item, string tableName);
        response HasChildren(SocietateAsigurare item, string tableName, int childrenId);
        response GetChildrens(SocietateAsigurare item, string tableName);
        response GetChildren(SocietateAsigurare item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
    }

    public class SocietatiAsigurareRepository : ISocietatiAsigurareRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public SocietatiAsigurareRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SOCIETATI_ASIGURAREsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    SocietateAsigurare a = new SocietateAsigurare(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                SocietateAsigurare[] toReturn = new SocietateAsigurare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (SocietateAsigurare)aList[i];
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
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(SocietateAsigurare), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SOCIETATI_ASIGURAREsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    SocietateAsigurare a = new SocietateAsigurare(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                SocietateAsigurare[] toReturn = new SocietateAsigurare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (SocietateAsigurare)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                SocietateAsigurare item = new SocietateAsigurare(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(SocietateAsigurare item)
        {
            return item.Insert();
        }

        public response Update(SocietateAsigurare item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            //SocietateAsigurare item = JsonConvert.DeserializeObject<SocietateAsigurare>(Find(id).Message);
            SocietateAsigurare item = (SocietateAsigurare)(Find(id).Result);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            SocietateAsigurare tmpItem = JsonConvert.DeserializeObject<SocietateAsigurare>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<SocietateAsigurare>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((SocietateAsigurare)(Find(Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
        }

        public response Delete(SocietateAsigurare item)
        {
            return item.Delete();
        }

        public response HasChildrens(SocietateAsigurare item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(SocietateAsigurare item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(SocietateAsigurare item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(SocietateAsigurare item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }
        public response Delete(int _id)
        {
            response obj = Find(_id);
            //return JsonConvert.DeserializeObject<SocietateAsigurare>(obj.Message).Delete();
            return ((SocietateAsigurare)obj.Result).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<SocietateAsigurare>(obj.Message).HasChildrens(tableName);
            return ((SocietateAsigurare)obj.Result).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<SocietateAsigurare>(obj.Message).HasChildren(tableName, childrenId);
            return ((SocietateAsigurare)obj.Result).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<SocietateAsigurare>(obj.Message).GetChildrens(tableName);
            return ((SocietateAsigurare)obj.Result).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<SocietateAsigurare>(obj.Message).GetChildren(tableName, childrenId);
            return ((SocietateAsigurare)obj.Result).GetChildren(tableName, childrenId);
        }
    }
}
