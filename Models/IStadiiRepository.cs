using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IStadiiRepository
    {
        Stadiu[] GetAll();
        Stadiu[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Stadiu Find(int _id);
        response Insert(Stadiu item);
        response Update(Stadiu item);
        response Update(int id, string fieldValueCollection);
        response Delete(Stadiu item);
        bool HasChildrens(Stadiu item, string tableName);
        bool HasChildren(Stadiu item, string tableName, int childrenId);
        object[] GetChildrens(Stadiu item, string tableName);
        object GetChildren(Stadiu item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class StadiiRepository : IStadiiRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public StadiiRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Stadiu[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "STADIIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Stadiu a = new Stadiu(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Stadiu[] toReturn = new Stadiu[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Stadiu)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Stadiu[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(Stadiu), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "STADIIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Stadiu a = new Stadiu(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Stadiu[] toReturn = new Stadiu[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Stadiu)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public Stadiu Find(int _id)
        {
            Stadiu item = new Stadiu(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(Stadiu item)
        {
            return item.Insert();
        }

        public response Update(Stadiu item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Stadiu item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(Stadiu item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Stadiu item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Stadiu item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Stadiu item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Stadiu item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }
        public response Delete(int _id)
        {
            var obj = Find(_id);
            return obj.Delete();
        }

        public bool HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return obj.HasChildrens(tableName);
        }
        public bool HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return obj.HasChildren(tableName, childrenId);
        }
        public object[] GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return obj.GetChildrens(tableName);
        }
        public object GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return obj.GetChildren(tableName, childrenId);
        }
    }
}
