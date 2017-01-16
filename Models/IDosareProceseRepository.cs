using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IDosareProceseRepository
    {
        DosarProces[] GetAll();
        DosarProces[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        DosarProces Find(int _id);
        response Insert(DosarProces item);
        response Update(DosarProces item);
        response Update(int id, string fieldValueCollection);
        response Delete(DosarProces item);
        bool HasChildrens(DosarProces item, string tableName);
        bool HasChildren(DosarProces item, string tableName, int childrenId);
        object[] GetChildrens(DosarProces item, string tableName);
        object GetChildren(DosarProces item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class DosareProceseRepository : IDosareProceseRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public DosareProceseRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public DosarProces[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_PROCESEsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarProces a = new DosarProces(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarProces[] toReturn = new DosarProces[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarProces)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public DosarProces[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_PROCESEsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarProces a = new DosarProces(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarProces[] toReturn = new DosarProces[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarProces)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public DosarProces Find(int _id)
        {
            DosarProces item = new DosarProces(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(DosarProces item)
        {
            return item.Insert();
        }

        public response Update(DosarProces item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            DosarProces item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(DosarProces item)
        {
            return item.Delete();
        }

        public bool HasChildrens(DosarProces item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(DosarProces item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(DosarProces item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(DosarProces item, string tableName, int childrenId)
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

