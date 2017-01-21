using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IMesajeRepository
    {
        Mesaj[] GetAll();
        Mesaj[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        Mesaj Find(int _id);
        response Insert(Mesaj item);
        response Update(Mesaj item);
        response Update(int id, string fieldValueCollection);
        response Delete(Mesaj item);
        bool HasChildrens(Mesaj item, string tableName);
        bool HasChildren(Mesaj item, string tableName, int childrenId);
        object[] GetChildrens(Mesaj item, string tableName);
        object GetChildren(Mesaj item, string tableName, int childrenId);
        void GenerateAndSendMessage(int? IdDosar, DateTime Data, string TipMesaj, int IdSender, int Importanta);
        Utilizator[] GetReceiversByIdDosar(Mesaj item);
        void SendToInvolvedParties(Mesaj item);
        Dosar GetDosar(Mesaj item);
        Nomenclator GetTipMesaj(Mesaj item);
        response SetMessageReadDate(Mesaj item, int idUtilizator, DateTime ReadDate);
        Utilizator[] GetReceivers(Mesaj item);
        Utilizator GetSender(Mesaj item);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class MesajeRepository : IMesajeRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        //public MesajeRepository(int _authenticatedUserId, string _connectionString)
        public MesajeRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Mesaj[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJEsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Mesaj a = new Mesaj(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Mesaj[] toReturn = new Mesaj[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Mesaj)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public Mesaj[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = CommonFunctions.GenerateFilterFromJsonObject(typeof(Mesaj), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJEsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    Mesaj a = new Mesaj(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                Mesaj[] toReturn = new Mesaj[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (Mesaj)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public Mesaj Find(int _id)
        {
            Mesaj item = new Mesaj(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(Mesaj item)
        {
            return item.Insert();
        }

        public response Update(Mesaj item)
        {
            return item.Update();
        }

        public response Update(int id, string fieldValueCollection)
        {
            Mesaj item = Find(id);
            return item.Update(fieldValueCollection);
        }

        public response Delete(Mesaj item)
        {
            return item.Delete();
        }

        public bool HasChildrens(Mesaj item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(Mesaj item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(Mesaj item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(Mesaj item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }

        public void GenerateAndSendMessage(int? IdDosar, DateTime Data, string TipMesaj, int IdSender, int Importanta)
        {
            Mesaj mesaj = new Mesaj(authenticatedUserId, connectionString, IdDosar, Data, TipMesaj, IdSender, Importanta);
            response r = mesaj.Insert();
            if(r.Status && r.InsertedId != null)
                mesaj.SendToInvolvedParties();
        }

        public Utilizator[] GetReceiversByIdDosar(Mesaj item)
        {
            return item.GetReceiversByIdDosar();
        }

        public void SendToInvolvedParties(Mesaj item)
        {
            item.SendToInvolvedParties();
        }

        public Dosar GetDosar(Mesaj item)
        {
            return item.GetDosar();
        }
        public Nomenclator GetTipMesaj(Mesaj item)
        {
            return item.GetTipMesaj();
        }
        public response SetMessageReadDate(Mesaj item, int idUtilizator, DateTime ReadDate)
        {
            return item.SetMessageReadDate(idUtilizator, ReadDate);
        }
        public Utilizator[] GetReceivers(Mesaj item)
        {
            return item.GetReceivers();
        }
        public Utilizator GetSender(Mesaj item)
        {
            return item.GetSender();
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
