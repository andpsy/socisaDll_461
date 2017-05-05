using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SOCISA.Models
{
    public enum Importanta { Low = 0, Medium, High };

    public class Mesaj
    {
        const string _TABLE_NAME = "mesaje";
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }

        public int? ID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        [Required(ErrorMessage = "Campul \"De la\" este obligatoriu!")]
        [Display(Name = "De la")]
        public int ID_SENDER { get; set; }
        /* public UtilizatoriJson Sender { get; set; } */
        //public int ID_RECEIVER { get; set; }
        /* public UtilizatoriJson Receiver { get; set; } */
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        [Display(Name = "Subiect")]
        public string SUBIECT { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string BODY { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public DateTime DATA { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public int? ID_DOSAR { get; set; }
        /* public DosareJson Dosar { get; set; } */
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public int? IMPORTANTA { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public int? ID_TIP_MESAJ { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public int? REPLY_TO { get; set; }
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
            MySqlDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                MesajConstructor(item);
                break;
            }
            r.Close(); r.Dispose();
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
            try { this.REPLY_TO = Convert.ToInt32(mesaj["REPLY_TO"]); }
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
                Dictionary<string, string> changes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldValueCollection, CommonFunctions.JsonDeserializerSettings);
                foreach (string fieldName in changes.Keys)
                {
                    PropertyInfo[] props = this.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        //var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "actions");
                        //if (col != null && col.ToUpper().IndexOf(prop.Name.ToUpper()) > -1 && fieldName.ToUpper() == prop.Name.ToUpper()) // ca sa includem in Array-ul de parametri doar coloanele tabelei, nu si campurile externe si/sau alte proprietati
                        if (fieldName.ToUpper() == prop.Name.ToUpper())
                        {
                            var tmpVal = prop.PropertyType.FullName.IndexOf("System.Nullable") > -1 && changes[fieldName] == null ? null : prop.PropertyType.FullName.IndexOf("System.String") > -1 ? changes[fieldName] : prop.PropertyType.FullName.IndexOf("System.DateTime") > -1 ? CommonFunctions.SwitchBackFormatedDate(changes[fieldName]) : ((prop.PropertyType.FullName.IndexOf("Double") > -1) ? CommonFunctions.BackDoubleValue(changes[fieldName]) : Newtonsoft.Json.JsonConvert.DeserializeObject(changes[fieldName], prop.PropertyType));
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
            response toReturn = new response(false, "", null, null, new List<Error>()); ;
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJEsp_soft_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        public response Validare()
        {
            response toReturn = new response(true, "", null, null, new List<Error>()); ;
            Error err = new Error();
            if (this.ID_SENDER <= 0)
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyMessageSender");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            /*
            if (((Utilizator[])this.GetReceiversByIdDosar().Result).Length == 0)
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyMessageReceiver");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            */
            if (this.SUBIECT == null || this.SUBIECT.Trim() == "")
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyMessageSubject");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            /*
            if (this.BODY == null || this.BODY.Trim() == "")
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyMessageBody");
                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                toReturn.InsertedId = null;
                toReturn.Error.Add(err);
            }
            */
            return toReturn;
        }

        public response ValidareColoane(string fieldValueCollection)
        {
            return CommonFunctions.ValidareColoane(this, fieldValueCollection);
        }

        public string GenerateFilterFromJsonObject()
        {
            return Filtering.GenerateFilterFromJsonObject(this);
        }

        public response HasChildrens(string tableName)
        {
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "mesaje", tableName);
        }

        public response HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "mesaje", tableName, childrenId);
        }

        public response GetChildrens(string tableName)
        {
            return CommonFunctions.GetChildrens(this, tableName);
        }

        public response GetChildren(string tableName, int childrenId)
        {
            return CommonFunctions.GetChildren(this, tableName, childrenId);
        }


        /// <summary>
        /// Metoda pt. popularea Senderului din dosar
        /// </summary>
        /// <returns>SOCISA.UtilizatoriJson</returns>
        public response GetSender()
        {
            try
            {
                Utilizator toReturn = new Utilizator(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_SENDER));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Destinatarilor mesajului
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriJson</returns>
        public response GetReceivers()
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
                r.Close(); r.Dispose();
                Utilizator[] toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Utilizator)aList[i];
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Destinatarilor mesajului
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriJson</returns>
        public response GetReceiversByIdDosar()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORIsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID_DOSAR), new MySqlParameter("_ID_SENDER", this.ID_SENDER) });
                IDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(authenticatedUserId, connectionString, Convert.ToInt32(r["ID"]));
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Utilizator[] toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Utilizator)aList[i];
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. setarea datei citirii unui mesaj de catre un Utilizator (destinatar)
        /// </summary>
        /// <param name="IdUtilizator">Utilizatorul (Destinatarul) care citeste mesajul curent</param>
        /// <param name="ReadDate">Data citirii</param>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response SetMessageReadDate(int idUtilizator, DateTime ReadDate)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJE_UTILIZATORIsp_GetByIdMesajIdUtilizator", new object[] { new MySqlParameter("_ID_MESAJ", this.ID), new MySqlParameter("_ID_UTILIZATOR", idUtilizator) });
                IDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    MesajUtilizator mesajUtilizator = new MesajUtilizator(authenticatedUserId, connectionString, (IDataRecord)r);
                    mesajUtilizator.DATA_CITIRE = ReadDate;
                    return mesajUtilizator.Update();
                }
                r.Close(); r.Dispose();
                return new response(true, null, null, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public response GetMessageReadDate(int idUtilizator)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "MESAJE_UTILIZATORIsp_GetByIdMesajIdUtilizator", new object[] { new MySqlParameter("_ID_MESAJ", this.ID), new MySqlParameter("_ID_UTILIZATOR", idUtilizator) });
                IDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    MesajUtilizator mesajUtilizator = new MesajUtilizator(authenticatedUserId, connectionString, (IDataRecord)r);
                    return new response(true, mesajUtilizator.DATA_CITIRE.ToString(), mesajUtilizator.DATA_CITIRE, null, null);
                }
                r.Close(); r.Dispose();
                return new response(true, null, null, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Tipului mesajului
        /// </summary>
        /// <returns>SOCISA.NomenclatorJson</returns>
        public response GetTipMesaj()
        {
            try
            {
                Nomenclator toReturn = new Nomenclator(authenticatedUserId, connectionString, "tip_mesaje", (Convert.ToInt32(this.ID_TIP_MESAJ)));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Dosarului la care este atasat mesajul
        /// </summary>
        /// <returns>SOCISA.DosareJson</returns>
        public response GetDosar()
        {
            try
            {
                Dosar toReturn = new Dosar(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_DOSAR));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn, CommonFunctions.JsonSerializerSettings), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public Mesaj(int _authenticatedUserId, string _connectionString, int? IdDosar, DateTime Data, string Subiect, string Body, string TipMesaj, int IdSender, int Importanta)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            ID_DOSAR = IdDosar;
            DATA = Data;
            SUBIECT = Subiect;
            BODY = Body;
            ID_SENDER = IdSender;
            ID_TIP_MESAJ = new Nomenclator(authenticatedUserId, connectionString, "tip_mesaje").GetIdByName("tip_mesaje", TipMesaj);
            IMPORTANTA = Importanta;
        }

        public response SendToInvolvedParties()
        {
            try
            {
                response toReturn = new response(true, "", null, null, new List<Error>());
                Dosar d = (Dosar)this.GetDosar().Result;
                Utilizator[] utilizatori = (Utilizator[])d.GetInvolvedParties().Result;
                foreach (Utilizator utilizator in utilizatori)
                {
                    MesajUtilizator mesajUtilizator = new MesajUtilizator(authenticatedUserId, connectionString) { ID_UTILIZATOR = Convert.ToInt32(utilizator.ID), ID_MESAJ = Convert.ToInt32(this.ID) };
                    response r = mesajUtilizator.Insert();
                    toReturn.AddResponse(r);
                }
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }
    }
}