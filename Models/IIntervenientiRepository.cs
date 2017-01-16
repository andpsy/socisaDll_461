using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IIntervenientiRepository
    {
        Intervenient[] GetAll();
        Intervenient[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Intervenient Find(int _id);
        response Insert(Intervenient item);
        response Update(Intervenient item);
        response Update(int id, string fieldValueCollection);
        response Delete(Intervenient item);
        bool HasChildrens(Intervenient item, string tableName);
        bool HasChildren(Intervenient item, string tableName, int childrenId);
        object[] GetChildrens(Intervenient item, string tableName);
        object GetChildren(Intervenient item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
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

        public Intervenient[] GetAll()
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
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Intervenient[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
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
                return toReturn;
            }
            catch { return null; }
        }

        public Intervenient Find(int _id)
        {
            Intervenient item = new Intervenient(authenticatedUserId, connectionString, _id);
            return item;
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
            Intervenient item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(Intervenient item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Intervenient item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Intervenient item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Intervenient item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Intervenient item, string tableName, int childrenId)
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
