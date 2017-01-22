using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface ISocietatiAsigurareRepository
    {
        SocietateAsigurare[] GetAll();
        SocietateAsigurare[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        SocietateAsigurare Find(int _id);
        response Insert(SocietateAsigurare item);
        response Update(SocietateAsigurare item);
        response Update(int id, string fieldValueCollection);
        response Delete(SocietateAsigurare item);
        bool HasChildrens(SocietateAsigurare item, string tableName);
        bool HasChildren(SocietateAsigurare item, string tableName, int childrenId);
        object[] GetChildrens(SocietateAsigurare item, string tableName);
        object GetChildren(SocietateAsigurare item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class SocietatiAsigurareRepository : ISocietatiAsigurareRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public SocietatiAsigurareRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public SocietateAsigurare[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SOCIETATI_ASIGURAREsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    SocietateAsigurare a = new SocietateAsigurare(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                SocietateAsigurare[] toReturn = new SocietateAsigurare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (SocietateAsigurare)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public SocietateAsigurare[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(SocietateAsigurare), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "SOCIETATI_ASIGURAREsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    SocietateAsigurare a = new SocietateAsigurare(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                SocietateAsigurare[] toReturn = new SocietateAsigurare[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (SocietateAsigurare)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public SocietateAsigurare Find(int _id)
        {
            SocietateAsigurare item = new SocietateAsigurare(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(SocietateAsigurare item)
        {
            return item.Insert();
        }

        public response Update(SocietateAsigurare item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            SocietateAsigurare item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(SocietateAsigurare item)
        {
            return item.Delete();
        }

        public bool HasChildrens(SocietateAsigurare item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(SocietateAsigurare item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(SocietateAsigurare item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(SocietateAsigurare item, string tableName, int childrenId)
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
