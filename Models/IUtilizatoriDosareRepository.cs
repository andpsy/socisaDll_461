using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IUtilizatoriDosareRepository
    {
        UtilizatorDosar[] GetAll();
        UtilizatorDosar[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        UtilizatorDosar Find(int _id);
        response Insert(UtilizatorDosar item);
        response Update(UtilizatorDosar item);
        response Update(int id, string fieldValueCollection);
        response Delete(UtilizatorDosar item);
        bool HasChildrens(UtilizatorDosar item, string tableName);
        bool HasChildren(UtilizatorDosar item, string tableName, int childrenId);
        object[] GetChildrens(UtilizatorDosar item, string tableName);
        object GetChildren(UtilizatorDosar item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class UtilizatoriDosareRepository : IUtilizatoriDosareRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public UtilizatoriDosareRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public UtilizatorDosar[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_DOSAREsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorDosar a = new UtilizatorDosar(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorDosar[] toReturn = new UtilizatorDosar[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorDosar)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public UtilizatorDosar[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_DOSAREsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorDosar a = new UtilizatorDosar(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorDosar[] toReturn = new UtilizatorDosar[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorDosar)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public UtilizatorDosar Find(int _id)
        {
            UtilizatorDosar item = new UtilizatorDosar(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(UtilizatorDosar item)
        {
            return item.Insert();
        }

        public response Update(UtilizatorDosar item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            UtilizatorDosar item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(UtilizatorDosar item)
        {
            return item.Delete();
        }

        public bool HasChildrens(UtilizatorDosar item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(UtilizatorDosar item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(UtilizatorDosar item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(UtilizatorDosar item, string tableName, int childrenId)
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
