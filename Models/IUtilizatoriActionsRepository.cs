﻿using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface IUtilizatoriActionsRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response Find(int _id);
        response Insert(UtilizatorAction item);
        response Update(UtilizatorAction item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response Delete(UtilizatorAction item);
        response HasChildrens(UtilizatorAction item, string tableName);
        response HasChildren(UtilizatorAction item, string tableName, int childrenId);
        response GetChildrens(UtilizatorAction item, string tableName);
        response GetChildren(UtilizatorAction item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
    }

    public class UtilizatoriActionsRepository : IUtilizatoriActionsRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public UtilizatoriActionsRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public response GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_ACTIONSsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorAction a = new UtilizatorAction(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorAction[] toReturn = new UtilizatorAction[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorAction)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(UtilizatorAction), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_ACTIONSsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    UtilizatorAction a = new UtilizatorAction(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                UtilizatorAction[] toReturn = new UtilizatorAction[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (UtilizatorAction)aList[i];
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                UtilizatorAction item = new UtilizatorAction(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Insert(UtilizatorAction item)
        {
            return item.Insert();
        }

        public response Update(UtilizatorAction item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            UtilizatorAction item = JsonConvert.DeserializeObject<UtilizatorAction>(Find(id).Message);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            UtilizatorAction tmpItem = JsonConvert.DeserializeObject<UtilizatorAction>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            return JsonConvert.DeserializeObject<UtilizatorAction>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
        }

        public response Delete(UtilizatorAction item)
        {
            return item.Delete();
        }

        public response HasChildrens(UtilizatorAction item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(UtilizatorAction item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(UtilizatorAction item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(UtilizatorAction item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }
        public response Delete(int _id)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<UtilizatorAction>(obj.Message).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<UtilizatorAction>(obj.Message).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<UtilizatorAction>(obj.Message).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<UtilizatorAction>(obj.Message).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<UtilizatorAction>(obj.Message).GetChildren(tableName, childrenId);
        }
    }
}
