using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IDosareStadiiSentinteRepository
    {
        DosarStadiuSentinta[] GetAll();
        DosarStadiuSentinta[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        DosarStadiuSentinta Find(int _id);
        response Insert(DosarStadiuSentinta item);
        response Update(DosarStadiuSentinta item);
        response Update(int id, string fieldValueCollection);
        response Delete(DosarStadiuSentinta item);
        bool HasChildrens(DosarStadiuSentinta item, string tableName);
        bool HasChildren(DosarStadiuSentinta item, string tableName, int childrenId);
        object[] GetChildrens(DosarStadiuSentinta item, string tableName);
        object GetChildren(DosarStadiuSentinta item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class DosareStadiiSentinteRepository : IDosareStadiiSentinteRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public DosareStadiiSentinteRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public DosarStadiuSentinta[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADII_SENTINTEsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarStadiuSentinta a = new DosarStadiuSentinta(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarStadiuSentinta[] toReturn = new DosarStadiuSentinta[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarStadiuSentinta)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public DosarStadiuSentinta[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADII_SENTINTEsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DosarStadiuSentinta a = new DosarStadiuSentinta(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DosarStadiuSentinta[] toReturn = new DosarStadiuSentinta[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DosarStadiuSentinta)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public DosarStadiuSentinta Find(int _id)
        {
            DosarStadiuSentinta item = new DosarStadiuSentinta(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(DosarStadiuSentinta item)
        {
            return item.Insert();
        }

        public response Update(DosarStadiuSentinta item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            DosarStadiuSentinta item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(DosarStadiuSentinta item)
        {
            return item.Delete();
        }

        public bool HasChildrens(DosarStadiuSentinta item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(DosarStadiuSentinta item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(DosarStadiuSentinta item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(DosarStadiuSentinta item, string tableName, int childrenId)
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
