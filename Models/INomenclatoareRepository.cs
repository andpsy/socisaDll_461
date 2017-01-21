using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface INomenclatoareRepository
    {
        Nomenclator[] GetAll(string tableName);
        Nomenclator[] GetFiltered(string tableName, string _sort, string _order, string _filter, string _limit);
        Nomenclator Find(string tableName, int _id);
        response Insert(Nomenclator item);
        response Update(Nomenclator item);
        response Update(string tableName, int id, string fieldValueCollection);
        response Delete(Nomenclator item);
        bool HasChildrens(Nomenclator item, string tableName);
        bool HasChildren(Nomenclator item, string tableName, int childrenId);
        object[] GetChildrens(Nomenclator item, string tableName);
        object GetChildren(Nomenclator item, string tableName, int childrenId);
        int? GetIdByName(Nomenclator item, string tableName, string denumire);
        response Delete(string tableName, int _id);
        bool HasChildrens(string tableName, int _id, string childTableName);
        bool HasChildren(string tableName, int _id, string childTableName, int childrenId);
        object[] GetChildrens(string tableName, int _id, string childTableName);
        object GetChildren(string tableName, int _id, string childTableName, int childrenId);
    }

    public class NomenclatoareRepository : INomenclatoareRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public NomenclatoareRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Nomenclator[] GetAll(string tableName)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format("{0}sp_select", tableName.ToUpper()), new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Nomenclator a = new Nomenclator(authenticatedUserId, connectionString, tableName, (IDataRecord)r);
                    aList.Add(a);
                }
                Nomenclator[] toReturn = new Nomenclator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Nomenclator)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Nomenclator[] GetFiltered(string tableName, string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = CommonFunctions.GenerateFilterFromJsonObject(typeof(Nomenclator), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, String.Format("{0}sp_select", tableName.ToUpper()), new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Nomenclator a = new Nomenclator(authenticatedUserId, connectionString, tableName, (IDataRecord)r);
                    aList.Add(a);
                }
                Nomenclator[] toReturn = new Nomenclator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Nomenclator)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public Nomenclator Find(string tableName, int _id)
        {
            Nomenclator item = new Nomenclator(authenticatedUserId, connectionString, tableName, _id);
            return item;
        }

        public response Insert(Nomenclator item)
        {
            return item.Insert();
        }

        public response Update(Nomenclator item)
        {
            return item.Update();
        }

        public response Update(string tableName, int id, string fieldValueCollection)
        {
            Nomenclator item = Find(tableName, id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(Nomenclator item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Nomenclator item, string childTableName)
        {
            return item.HasChildrens(childTableName);
        }

        public bool HasChildren(Nomenclator item, string childTableName, int childrenId)
        {
            return item.HasChildren(childTableName, childrenId);
        }

        public object[] GetChildrens(Nomenclator item, string childTableName)
        {
            return item.GetChildrens(childTableName);
        }

        public object GetChildren(Nomenclator item, string childTableName, int childrenId)
        {
            return item.GetChildren(childTableName, childrenId);
        }

        public int? GetIdByName(Nomenclator item, string tableName, string denumire)
        {
            return item.GetIdByName(tableName, denumire);
        }
        public response Delete(string tableName, int _id)
        {
            var obj = Find(tableName, _id);
            return obj.Delete();
        }

        public bool HasChildrens(string tableName, int _id, string childTableName)
        {
            var obj = Find(tableName, _id);
            return obj.HasChildrens(childTableName);
        }
        public bool HasChildren(string tableName, int _id, string childTableName, int childrenId)
        {
            var obj = Find(tableName, _id);
            return obj.HasChildren(childTableName, childrenId);
        }
        public object[] GetChildrens(string tableName, int _id, string childTableName)
        {
            var obj = Find(tableName, _id);
            return obj.GetChildrens(childTableName);
        }
        public object GetChildren(string tableName, int _id, string childTableName, int childrenId)
        {
            var obj = Find(tableName, _id);
            return obj.GetChildren(childTableName, childrenId);
        }
    }
}
