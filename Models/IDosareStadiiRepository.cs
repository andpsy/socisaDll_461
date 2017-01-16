using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IDosareStadiiRepository
    {
        DosarStadiu[] GetAll();
        DosarStadiu[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        DosarStadiu Find(int _id);
        response Insert(DosarStadiu item);
        response Update(DosarStadiu item);
        response Update(int id, string fieldValueCollection);
        response Delete(DosarStadiu item);
        bool HasChildrens(DosarStadiu item, string tableName);
        bool HasChildren(DosarStadiu item, string tableName, int childrenId);
        object[] GetChildrens(DosarStadiu item, string tableName);
        object GetChildren(DosarStadiu item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class DosareStadiiRepository : IDosareStadiiRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public DosareStadiiRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public DosarStadiu[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarStadiu a = new DosarStadiu(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarStadiu[] toReturn = new DosarStadiu[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarStadiu)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public DosarStadiu[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarStadiu a = new DosarStadiu(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarStadiu[] toReturn = new DosarStadiu[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarStadiu)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public DosarStadiu Find(int _id)
        {
            DosarStadiu item = new DosarStadiu(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(DosarStadiu item)
        {
            return item.Insert();
        }

        public response Update(DosarStadiu item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            DosarStadiu item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(DosarStadiu item)
        {
            return item.Delete();
        }

        public bool HasChildrens(DosarStadiu item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(DosarStadiu item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(DosarStadiu item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(DosarStadiu item, string tableName, int childrenId)
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
