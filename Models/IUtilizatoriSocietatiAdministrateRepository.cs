using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IUtilizatoriSocietatiAdministrateRepository
    {
        UtilizatorSocietateAdministrata[] GetAll();
        UtilizatorSocietateAdministrata[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        UtilizatorSocietateAdministrata Find(int _id);
        response Insert(UtilizatorSocietateAdministrata item);
        response Update(UtilizatorSocietateAdministrata item);
        response Update(int id, string fieldValueCollection);
        response Delete(UtilizatorSocietateAdministrata item);
        bool HasChildrens(UtilizatorSocietateAdministrata item, string tableName);
        bool HasChildren(UtilizatorSocietateAdministrata item, string tableName, int childrenId);
        object[] GetChildrens(UtilizatorSocietateAdministrata item, string tableName);
        object GetChildren(UtilizatorSocietateAdministrata item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class UtilizatoriSocietatiAdministrateRepository : IUtilizatoriSocietatiAdministrateRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public UtilizatoriSocietatiAdministrateRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public UtilizatorSocietateAdministrata[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SOCIETATI_ADMINISTRATEsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorSocietateAdministrata a = new UtilizatorSocietateAdministrata(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorSocietateAdministrata[] toReturn = new UtilizatorSocietateAdministrata[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorSocietateAdministrata)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public UtilizatorSocietateAdministrata[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(UtilizatorSocietateAdministrata), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_SOCIETATI_ADMINISTRATEsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorSocietateAdministrata a = new UtilizatorSocietateAdministrata(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorSocietateAdministrata[] toReturn = new UtilizatorSocietateAdministrata[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorSocietateAdministrata)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public UtilizatorSocietateAdministrata Find(int _id)
        {
            UtilizatorSocietateAdministrata item = new UtilizatorSocietateAdministrata(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(UtilizatorSocietateAdministrata item)
        {
            return item.Insert();
        }

        public response Update(UtilizatorSocietateAdministrata item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            UtilizatorSocietateAdministrata item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(UtilizatorSocietateAdministrata item)
        {
            return item.Delete();
        }

        public bool HasChildrens(UtilizatorSocietateAdministrata item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(UtilizatorSocietateAdministrata item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(UtilizatorSocietateAdministrata item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(UtilizatorSocietateAdministrata item, string tableName, int childrenId)
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
