using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IAsiguratiRepository
    {
        Asigurat[] GetAll();
        Asigurat[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Asigurat Find(int _id);
        Asigurat Find(string _denumire);
        response Insert(Asigurat item);
        response Update(Asigurat item);
        response Update(int id, string fieldValueCollection);
        response Delete(Asigurat item);
        bool HasChildrens(Asigurat item, string tableName);
        bool HasChildren(Asigurat item, string tableName, int childrenId);
        object[] GetChildrens(Asigurat item, string tableName);
        object GetChildren(Asigurat item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class AsiguratiRepository : IAsiguratiRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public AsiguratiRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Asigurat[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "ASIGURATIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Asigurat a = new Asigurat(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Asigurat[] toReturn = new Asigurat[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Asigurat)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Asigurat[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = CommonFunctions.GenerateFilterFromJsonObject(typeof(Asigurat), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "ASIGURATIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Asigurat a = new Asigurat(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Asigurat[] toReturn = new Asigurat[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Asigurat)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public Asigurat Find(int _id)
        {
            Asigurat item = new Asigurat(authenticatedUserId, connectionString, _id);
            return item;
        }

        public Asigurat Find(string _nr_auto)
        {
            Asigurat item = new Asigurat(authenticatedUserId, connectionString, _nr_auto);
            return item;
        }

        public response Insert(Asigurat item)
        {
            return item.Insert();
        }

        public response Update(Asigurat item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Asigurat item = Find(id);
            return item.Update(fieldValueCollection);
        }
        public response Delete(Asigurat item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Asigurat item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Asigurat item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Asigurat item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Asigurat item, string tableName, int childrenId)
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
