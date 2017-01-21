using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface ISetariRepository
    {
        Setare[] GetAll();
        Setare[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Setare Find(int _id);
        response Insert(Setare item);
        response Update(Setare item);
        response Update(int id, string fieldValueCollection);
        response Delete(Setare item);
        bool HasChildrens(Setare item, string tableName);
        bool HasChildren(Setare item, string tableName, int childrenId);
        object[] GetChildrens(Setare item, string tableName);
        object GetChildren(Setare item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class SetariRepository : ISetariRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public SetariRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Setare[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SETARIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Setare a = new Setare(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Setare[] toReturn = new Setare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Setare)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Setare[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = CommonFunctions.GenerateFilterFromJsonObject(typeof(Setare), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SETARIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Setare a = new Setare(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Setare[] toReturn = new Setare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Setare)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public Setare Find(int _id)
        {
            Setare item = new Setare(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(Setare item)
        {
            return item.Insert();
        }

        public response Update(Setare item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Setare item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(Setare item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Setare item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Setare item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Setare item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Setare item, string tableName, int childrenId)
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
