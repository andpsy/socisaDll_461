using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IUtilizatoriRepository
    {
        Utilizator[] GetAll();
        Utilizator[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Utilizator Find(int _id);
        response Insert(Utilizator action);
        response Update(Utilizator item);
        response Delete(Utilizator item);
        response Update(int id, string fieldValueCollection);
        bool HasChildrens(Utilizator item, string tableName);
        bool HasChildren(Utilizator item, string tableName, int childrenId);
        object[] GetChildrens(Utilizator item, string tableName);
        object GetChildren(Utilizator item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class UtilizatoriRepository : IUtilizatoriRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public UtilizatoriRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Utilizator[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Utilizator[] toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Utilizator)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Utilizator[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Utilizator[] toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Utilizator)aList[i];
                return toReturn;
            }
            catch { return null; }
        }


        public Utilizator Find(int _id)
        {
            Utilizator item = new Utilizator(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(Utilizator item)
        {
            return item.Insert();
        }

        public response Update(Utilizator item)
        {
            return item.Update();
        }

        public response Delete(Utilizator item)
        {
            return item.Delete();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Utilizator item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public bool HasChildrens(Utilizator item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Utilizator item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Utilizator item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Utilizator item, string tableName, int childrenId)
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
