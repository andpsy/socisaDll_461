using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IAutoRepository
    {
        Auto[] GetAll();
        Auto[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Auto Find(int _id);
        Auto Find(string _nr_auto);
        response Insert(Auto item);
        response Update(Auto item);
        response Update(int id, string fieldValueCollection);
        response Delete(Auto item);
        bool HasChildrens(Auto item, string tableName);
        bool HasChildren(Auto item, string tableName, int childrenId);
        object[] GetChildrens(Auto item, string tableName);
        object GetChildren(Auto item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class AutoRepository : IAutoRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public AutoRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Auto[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "AUTOsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Auto a = new Auto(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Auto[] toReturn = new Auto[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Auto)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Auto[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "AUTOsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Auto a = new Auto(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Auto[] toReturn = new Auto[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Auto)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public Auto Find(int _id)
        {
            Auto item = new Auto(authenticatedUserId, connectionString, _id);
            return item;
        }

        public Auto Find(string _nr_auto)
        {
            Auto item = new Auto(authenticatedUserId, connectionString, _nr_auto);
            return item;
        }

        public response Insert(Auto item)
        {
            return item.Insert();
        }

        public response Update(Auto item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Auto item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(Auto item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Auto item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Auto item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Auto item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Auto item, string tableName, int childrenId)
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
