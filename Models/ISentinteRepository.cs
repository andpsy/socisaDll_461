using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface ISentinteRepository
    {
        Sentinta[] GetAll();
        Sentinta[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Sentinta Find(int _id);
        response Insert(Sentinta item);
        response Update(Sentinta item);
        response Update(int id, string fieldValueCollection);
        response Delete(Sentinta item);
        bool HasChildrens(Sentinta item, string tableName);
        bool HasChildren(Sentinta item, string tableName, int childrenId);
        object[] GetChildrens(Sentinta item, string tableName);
        object GetChildren(Sentinta item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class SentinteRepository : ISentinteRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public SentinteRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Sentinta[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SENTINTEsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Sentinta a = new Sentinta(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Sentinta[] toReturn = new Sentinta[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Sentinta)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Sentinta[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SENTINTEsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Sentinta a = new Sentinta(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Sentinta[] toReturn = new Sentinta[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Sentinta)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public Sentinta Find(int _id)
        {
            Sentinta item = new Sentinta(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(Sentinta item)
        {
            return item.Insert();
        }

        public response Update(Sentinta item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Sentinta item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(Sentinta item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Sentinta item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Sentinta item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Sentinta item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Sentinta item, string tableName, int childrenId)
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
