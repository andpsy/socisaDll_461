using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IDrepturiRepository
    {
        Drept[] GetAll();
        Drept[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Drept Find(int _id);
        response Insert(Drept item);
        response Update(Drept item);
        response Update(int id, string fieldValueCollection);
        response Delete(Drept item);
        bool HasChildrens(Drept item, string tableName);
        bool HasChildren(Drept item, string tableName, int childrenId);
        object[] GetChildrens(Drept item, string tableName);
        object GetChildren(Drept item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    /// <summary>
    /// Clasa statica pentru selectia Drepturilor din baza de date
    /// </summary>
    public class DrepturiRepository:IDrepturiRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public DrepturiRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Drept[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DREPTURIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Drept a = new Drept(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Drept[] toReturn = new Drept[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Drept)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Drept[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DREPTURIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Drept a = new Drept(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Drept[] toReturn = new Drept[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Drept)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public Drept Find(int _id)
        {
            Drept item = new Drept(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(Drept item)
        {
            return item.Insert();
        }

        public response Update(Drept item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Drept item = Find(id);
            return item.Update(fieldValueCollection);
        }
        public response Delete(Drept item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Drept item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Drept item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Drept item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Drept item, string tableName, int childrenId)
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
