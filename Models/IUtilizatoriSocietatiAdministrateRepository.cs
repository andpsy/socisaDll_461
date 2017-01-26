using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface IUtilizatoriSocietatiAdministrateRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response Find(int _id);
        response Insert(UtilizatorSocietateAdministrata item);
        response Update(UtilizatorSocietateAdministrata item);
        response Update(int id, string fieldValueCollection);
        response Update(string fieldValueCollection);

        response Delete(UtilizatorSocietateAdministrata item);
        response HasChildrens(UtilizatorSocietateAdministrata item, string tableName);
        response HasChildren(UtilizatorSocietateAdministrata item, string tableName, int childrenId);
        response GetChildrens(UtilizatorSocietateAdministrata item, string tableName);
        response GetChildren(UtilizatorSocietateAdministrata item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
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

        public response GetAll()
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
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                UtilizatorSocietateAdministrata item = new UtilizatorSocietateAdministrata(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
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
            //UtilizatorSocietateAdministrata item = JsonConvert.DeserializeObject<UtilizatorSocietateAdministrata>(Find(id).Message);
            UtilizatorSocietateAdministrata item = (UtilizatorSocietateAdministrata)(Find(id).Result);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            UtilizatorSocietateAdministrata tmpItem = JsonConvert.DeserializeObject<UtilizatorSocietateAdministrata>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            //return JsonConvert.DeserializeObject<UtilizatorSocietateAdministrata>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
            return ((UtilizatorSocietateAdministrata)(Find(Convert.ToInt32(tmpItem.ID)).Result)).Update(fieldValueCollection);
        }
        public response Delete(UtilizatorSocietateAdministrata item)
        {
            return item.Delete();
        }

        public response HasChildrens(UtilizatorSocietateAdministrata item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(UtilizatorSocietateAdministrata item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(UtilizatorSocietateAdministrata item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(UtilizatorSocietateAdministrata item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }
        public response Delete(int _id)
        {
            response obj = Find(_id);
            //return JsonConvert.DeserializeObject<UtilizatorSocietateAdministrata>(obj.Message).Delete();
            return ((UtilizatorSocietateAdministrata)obj.Result).Delete();
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<UtilizatorSocietateAdministrata>(obj.Message).HasChildrens(tableName);
            return ((UtilizatorSocietateAdministrata)obj.Result).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<UtilizatorSocietateAdministrata>(obj.Message).HasChildren(tableName, childrenId);
            return ((UtilizatorSocietateAdministrata)obj.Result).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<UtilizatorSocietateAdministrata>(obj.Message).GetChildrens(tableName);
            return ((UtilizatorSocietateAdministrata)obj.Result).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            //return JsonConvert.DeserializeObject<UtilizatorSocietateAdministrata>(obj.Message).GetChildren(tableName, childrenId);
            return ((UtilizatorSocietateAdministrata)obj.Result).GetChildren(tableName, childrenId);
        }
    }
}
