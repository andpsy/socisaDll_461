using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IUtilizatoriSetariRepository
    {
        UtilizatorSetare[] GetAll();
        UtilizatorSetare[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        UtilizatorSetare Find(int _id);
        response Insert(UtilizatorSetare item);
        response Update(UtilizatorSetare item);
        response Update(int id, string fieldValueCollection);
        response Delete(UtilizatorSetare item);
        bool HasChildrens(UtilizatorSetare item, string tableName);
        bool HasChildren(UtilizatorSetare item, string tableName, int childrenId);
        object[] GetChildrens(UtilizatorSetare item, string tableName);
        object GetChildren(UtilizatorSetare item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class UtilizatoriSetariRepository : IUtilizatoriSetariRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public UtilizatoriSetariRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public UtilizatorSetare[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SETARIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorSetare a = new UtilizatorSetare(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorSetare[] toReturn = new UtilizatorSetare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorSetare)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public UtilizatorSetare[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = CommonFunctions.GenerateFilterFromJsonObject(typeof(UtilizatorSetare), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SETARIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorSetare a = new UtilizatorSetare(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorSetare[] toReturn = new UtilizatorSetare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorSetare)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public UtilizatorSetare Find(int _id)
        {
            UtilizatorSetare item = new UtilizatorSetare(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(UtilizatorSetare item)
        {
            return item.Insert();
        }

        public response Update(UtilizatorSetare item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            UtilizatorSetare item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(UtilizatorSetare item)
        {
            return item.Delete();
        }

        public bool HasChildrens(UtilizatorSetare item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(UtilizatorSetare item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(UtilizatorSetare item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(UtilizatorSetare item, string tableName, int childrenId)
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

