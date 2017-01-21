using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace SOCISA.Models
{
    public class Mesaj
    {
        public enum Importanta { Low = 0, Medium, High };
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }

        public int? ID { get; set; }
        public int ID_SENDER { get; set; }
        /* public UtilizatoriJson Sender { get; set; } */
        //public int ID_RECEIVER { get; set; }
        /* public UtilizatoriJson Receiver { get; set; } */
        public string SUBIECT { get; set; }
        public string BODY { get; set; }
        public DateTime DATA { get; set; }
        public int? ID_DOSAR { get; set; }
        /* public DosareJson Dosar { get; set; } */
        public int? IMPORTANTA { get; set; }
        public int? ID_TIP_MESAJ { get; set; }
        /* public NomenclatorJson TipMesaj { get; set; } */
        //public DateTime DATA_CITIRE { get; set; }
        //public MesajeAttachmentsJson[] MesajeAttachments { get; set; }
        //public AttachmentsJson[] Attachments { get; set; }

        public Mesaj() { }

        public Mesaj(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Mesaj(int _authenticatedUserId, string _connectionString, int _ID)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJEsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
            DbDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                MesajConstructor(item);
                break;
            }
        }

        public Mesaj(int _authenticatedUserId, string _connectionString, IDataRecord item)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            MesajConstructor(item);
        }

        /// <summary>
        /// Functie pentru popularea obiectului Mesaj, folosita de diferiti constructori
        /// </summary>
        /// <param name="mesaj">Inregistrare din DB cu informatiile mesajului curent</param>
        public void MesajConstructor(IDataRecord mesaj)
        {
            try { this.ID = Convert.ToInt32(mesaj["ID"]); }
            catch { }
            try { this.ID_SENDER = Convert.ToInt32(mesaj["ID_SENDER"]); }
            catch { }
            //try { this.ID_RECEIVER = Convert.ToInt32(mesaj["ID_RECEIVER"]); }
            //catch { }
            try { this.SUBIECT = mesaj["SUBIECT"].ToString(); }
            catch { }
            try { this.BODY = mesaj["BODY"].ToString(); }
            catch { }
            try { this.DATA = Convert.ToDateTime(mesaj["DATA"]); }
            catch { }
            try { this.ID_DOSAR = Convert.ToInt32(mesaj["ID_DOSAR"]); }
            catch { }
            try { this.IMPORTANTA = Convert.ToInt32(mesaj["IMPORTANTA"]); }
            catch { }
            try { this.ID_TIP_MESAJ = Convert.ToInt32(mesaj["ID_TIP_MESAJ"]); }
            catch { }
            //try { this.DATA_CITIRE = Convert.ToDateTime(mesaj["DATA_CITIRE"]); }
            //catch { }
            /*
            try { this.Sender = GetSender(); }
            catch { }
            try { this.Receiver = GetReceiver(); }
            catch { }
            try { this.Dosar = GetDosar(); }
            catch { }
            try { this.TipMesaj = GetTipMesaj(); }
            catch { }
            */
        }

        public response Insert()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();

            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "mesaje");
            foreach (PropertyInfo prop in props)
            {
                if (col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                {
                    string propName = prop.Name;
                    string propType = prop.PropertyType.ToString();
                    object propValue = prop.GetValue(this, null);
                    propValue = propValue == null ? DBNull.Value : propValue;
                    if (propType != null)
                    {
                        if (propName.ToUpper() != "ID") // il vom folosi doar la Edit!
                            _parameters.Add(new MySqlParameter(String.Format("_{0}", propName.ToUpper()), propValue));
                    }
                }
            }
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJEsp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status) this.ID = toReturn.InsertedId;

            return toReturn;
        }

        public response Update()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "mesaje");
            foreach (PropertyInfo prop in props)
            {
                if (col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                {
                    string propName = prop.Name;
                    string propType = prop.PropertyType.ToString();
                    object propValue = prop.GetValue(this, null);
                    propValue = propValue == null ? DBNull.Value : propValue;
                    if (propType != null)
                    {
                        _parameters.Add(new MySqlParameter(String.Format("_{0}", propName.ToUpper()), propValue));
                    }
                }
            }
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJEsp_update", _parameters.ToArray());
            toReturn = da.ExecuteUpdateQuery();

            return toReturn;
        }

        public response Update(string fieldValueCollection)
        {
            response r = ValidareColoane(fieldValueCollection);
            if (!r.Status)
            {
                return r;
            }
            else
            {
                Dictionary<string, string> changes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldValueCollection);
                foreach (string fieldName in changes.Keys)
                {
                    PropertyInfo[] props = this.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        //var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "actions");
                        //if (col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1 && fieldName.ToUpper() == prop.Name.ToUpper()) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                        if (fieldName.ToUpper() == prop.Name.ToUpper())
                        {
                            var tmpVal = prop.PropertyType.FullName.IndexOf("System.String") > -1 ? changes[fieldName] : Newtonsoft.Json.JsonConvert.DeserializeObject(changes[fieldName], prop.PropertyType);
                            prop.SetValue(this, tmpVal);
                            break;
                        }
                    }

                }
                return this.Update();
            }
        }

        public response Delete()
        {
            response toReturn = new response(false, "", null, new List<Error>()); ;
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJEsp_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        public response Validare()
        {
            response toReturn = new response(true, "", null, new List<Error>()); ;
            Error err = new Error();
            if (this.ID_SENDER <= 0)
            {
                toReturn.Status = false;
                err = CommonFunctions.ErrorMessage("emptyMessageSender");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            if (this.GetReceiversByIdDosar().Length == 0)
            {
                toReturn.Status = false;
                err = CommonFunctions.ErrorMessage("emptyMessageReceiver");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            if (this.SUBIECT == null || this.SUBIECT.Trim() == "")
            {
                toReturn.Status = false;
                err = CommonFunctions.ErrorMessage("emptyMessageSubject");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            if (this.BODY == null || this.BODY.Trim() == "")
            {
                toReturn.Status = false;
                err = CommonFunctions.ErrorMessage("emptyMessageBody");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            return toReturn;
        }

        public response ValidareColoane(string fieldValueCollection)
        {
            return CommonFunctions.ValidareColoane(this, fieldValueCollection);
        }

        public string GenerateFilterFromJsonObject()
        {
            return CommonFunctions.GenerateFilterFromJsonObject(this);
        }

        public bool HasChildrens(string tableName)
        {
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "mesaje", tableName);
        }

        public bool HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "mesaje", tableName, childrenId);
        }

        public object[] GetChildrens(string tableName)
        {
            return null;
        }

        public object GetChildren(string tableName, int childrenId)
        {
            return null;
        }


        /// <summary>
        /// Metoda pt. popularea Senderului din dosar
        /// </summary>
        /// <returns>SOCISA.UtilizatoriJson</returns>
        public Utilizator GetSender()
        {
            try
            {
                Utilizator toReturn = new Utilizator(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_SENDER));
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea Destinatarilor mesajului
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriJson</returns>
        public Utilizator[] GetReceivers()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJE_UTILIZATORIsp_GetByIdMesaj", new object[] { new MySqlParameter("_ID_MESAJ", this.ID) });
                IDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_UTILIZATOR"]));
                    aList.Add(a);
                }
                Utilizator[] toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Utilizator)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea Destinatarilor mesajului
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriJson</returns>
        public Utilizator[] GetReceiversByIdDosar()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID_DOSAR), new MySqlParameter("_ID_SENDER", this.ID_SENDER) });
                IDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_UTILIZATOR"]));
                    aList.Add(a);
                }
                Utilizator[] toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Utilizator)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. setarea datei citirii unui mesaj de catre un Utilizator (destinatar)
        /// </summary>
        /// <param name="IdUtilizator">Utilizatorul (Destinatarul) care citeste mesajul curent</param>
        /// <param name="ReadDate">Data citirii</param>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response SetMessageReadDate(int idUtilizator, DateTime ReadDate)
        {
            response toReturn = new response(false, "", null, new List<Error>()); ;
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJE_UTILIZATORIsp_GetByIdMesajIdUtilizator", new object[] { new MySqlParameter("_ID_MESAJ", this.ID), new MySqlParameter("_ID_UTILIZATOR", idUtilizator) });
                IDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    MesajUtilizator mesajUtilizator = new MesajUtilizator(authenticatedUserId, connectionString, (IDataRecord)r);
                    mesajUtilizator.DATA_CITIRE = ReadDate;
                    toReturn = mesajUtilizator.Update();
                    break;
                }
            }
            catch { }
            return toReturn;
        }

        /// <summary>
        /// Metoda pt. popularea Tipului mesajului
        /// </summary>
        /// <returns>SOCISA.NomenclatorJson</returns>
        public Nomenclator GetTipMesaj()
        {
            try
            {
                Nomenclator toReturn = new Nomenclator(authenticatedUserId, connectionString, "tip_mesaje", (Convert.ToInt32(this.ID_TIP_MESAJ)));
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea Dosarului la care este atasat mesajul
        /// </summary>
        /// <returns>SOCISA.DosareJson</returns>
        public Dosar GetDosar()
        {
            try
            {
                Dosar toReturn = new Dosar(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_DOSAR));
                return toReturn;
            }
            catch { return null; }
        }

        public Mesaj(int _authenticatedUserId, string _connectionString, int? IdDosar, DateTime Data, string TipMesaj, int IdSender, int Importanta)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            ID_DOSAR = IdDosar;
            DATA = Data;
            SUBIECT = TipMesaj;
            BODY = TipMesaj;
            ID_SENDER = IdSender;
            ID_TIP_MESAJ = new Nomenclator(authenticatedUserId, connectionString, "tip_mesaje").GetIdByName("tip_mesaje", TipMesaj);
            IMPORTANTA = Importanta;
        }

        public void SendToInvolvedParties()
        {
            Dosar d = this.GetDosar();
            Utilizator[] utilizatori = d.GetInvolvedParties();
            foreach (Utilizator utilizator in utilizatori)
            {
                MesajUtilizator mesajUtilizator = new MesajUtilizator(authenticatedUserId, connectionString) { ID_UTILIZATOR = Convert.ToInt32(utilizator.ID), ID_MESAJ = Convert.ToInt32(this.ID) };
                mesajUtilizator.Insert();
            }
        }
    }
}