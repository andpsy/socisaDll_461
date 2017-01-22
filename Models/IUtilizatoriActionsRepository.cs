using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IUtilizatoriActionsRepository
    {
        UtilizatorAction[] GetAll();
        UtilizatorAction[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        UtilizatorAction Find(int _id);
        response Insert(UtilizatorAction item);
        response Update(UtilizatorAction item);
        response Update(int id, string fieldValueCollection);
        response Delete(UtilizatorAction item);
        bool HasChildrens(UtilizatorAction item, string tableName);
        bool HasChildren(UtilizatorAction item, string tableName, int childrenId);
        object[] GetChildrens(UtilizatorAction item, string tableName);
        object GetChildren(UtilizatorAction item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class UtilizatoriActionsRepository : IUtilizatoriActionsRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public UtilizatoriActionsRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public UtilizatorAction[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_ACTIONSsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorAction a = new UtilizatorAction(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorAction[] toReturn = new UtilizatorAction[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorAction)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public UtilizatorAction[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(UtilizatorAction), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_ACTIONSsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorAction a = new UtilizatorAction(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorAction[] toReturn = new UtilizatorAction[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorAction)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public UtilizatorAction Find(int _id)
        {
            UtilizatorAction item = new UtilizatorAction(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(UtilizatorAction item)
        {
            return item.Insert();
        }

        public response Update(UtilizatorAction item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            UtilizatorAction item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(UtilizatorAction item)
        {
            return item.Delete();
        }

        public bool HasChildrens(UtilizatorAction item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(UtilizatorAction item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(UtilizatorAction item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(UtilizatorAction item, string tableName, int childrenId)
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
