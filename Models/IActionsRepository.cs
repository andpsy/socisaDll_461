using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
//using System.Reflection;

namespace SOCISA.Models
{
    public interface IActionsRepository
    {
        Action[] GetAll();
        Action[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Action Find(int _id);
        response Insert(Action item);
        response Update(Action item);
        response Update(int id, string fieldValueCollection);
        response Delete(Action item);
        bool HasChildrens(Action item, string tableName);
        bool HasChildren(Action item, string tableName, int childrenId);
        object[] GetChildrens(Action item, string tableName);
        object GetChildren(Action item, string tableName, int childrenId);

        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class ActionsRepository : IActionsRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public ActionsRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Action[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "ACTIONSsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Action a = new Action(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Action[] toReturn = new Action[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Action)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); throw exp; }
        }

        public Action[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                #region -- initial --
                /*
                string newFilter = null;
                try
                {
                    try
                    {
                        Action x = Newtonsoft.Json.JsonConvert.DeserializeObject<Action>(_filter);
                        newFilter = x.GenerateFilterFromJsonObject();
                    }
                    catch
                    {
                        try
                        {
                            dynamic jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(_filter);
                            Action x = new Action(authenticatedUserId, connectionString);
                            PropertyInfo[] pisX = x.GetType().GetProperties();
                            PropertyInfo[] pisJObj = jObj.GetType().GetProperties();
                            foreach (PropertyInfo piX in pisX)
                            {
                                foreach (PropertyInfo piJObj in pisJObj)
                                {
                                    if (piX.Name == piJObj.Name)
                                    {
                                        piX.SetValue(x, piJObj.GetValue(jObj));
                                        break;
                                    }
                                }
                            }
                            newFilter = x.GenerateFilterFromJsonObject();
                        }
                        catch { }
                    }
                }
                catch { }
                if (newFilter != null) _filter = newFilter;
                */
                #endregion
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(Action), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "ACTIONSsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Action a = new Action(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Action[] toReturn = new Action[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Action)aList[i];
                return toReturn;
            }
            catch(Exception exp) { LogWriter.Log(exp); throw exp; }
        }

        public Action Find(int _id)
        {
            Action item = new Action(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(Action item)
        {
            return item.Insert();
        }

        public response Update(Action item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Action item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(Action item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Action item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Action item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Action item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Action item, string tableName, int childrenId)
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
