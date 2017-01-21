using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IMesajeUtilizatoriRepository
    {
        MesajUtilizator[] GetAll();
        MesajUtilizator[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        MesajUtilizator Find(int _id);
        response Insert(MesajUtilizator item);
        response Update(MesajUtilizator item);
        response Update(int id, string fieldValueCollection);
        response Delete(MesajUtilizator item);
        bool HasChildrens(MesajUtilizator item, string tableName);
        bool HasChildren(MesajUtilizator item, string tableName, int childrenId);
        object[] GetChildrens(MesajUtilizator item, string tableName);
        object GetChildren(MesajUtilizator item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class MesajeUtilizatoriRepository : IMesajeUtilizatoriRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public MesajeUtilizatoriRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public MesajUtilizator[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJE_UTILIZATORIsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    MesajUtilizator a = new MesajUtilizator(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                MesajUtilizator[] toReturn = new MesajUtilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (MesajUtilizator)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public MesajUtilizator[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = CommonFunctions.GenerateFilterFromJsonObject(typeof(MesajUtilizator), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJE_UTILIZATORIsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    MesajUtilizator a = new MesajUtilizator(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                MesajUtilizator[] toReturn = new MesajUtilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (MesajUtilizator)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public MesajUtilizator Find(int _id)
        {
            MesajUtilizator item = new MesajUtilizator(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(MesajUtilizator item)
        {
            return item.Insert();
        }

        public response Update(MesajUtilizator item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            MesajUtilizator item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(MesajUtilizator item)
        {
            return item.Delete();
        }

        public bool HasChildrens(MesajUtilizator item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(MesajUtilizator item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(MesajUtilizator item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(MesajUtilizator item, string tableName, int childrenId)
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

