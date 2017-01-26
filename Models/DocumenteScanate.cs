using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.IO;

namespace SOCISA.Models
{
    /// <summary>
    /// Clasa care contine definitia obiectului ce mapeaza tabela cu Documente scanate din baza de date
    /// </summary>
    public class DocumentScanat
    {
        const string _TABLE_NAME = "documente_scanate";
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }
        public int? ID { get; set; }
        public string DENUMIRE_FISIER { get; set; }
        public string EXTENSIE_FISIER { get; set; }
        public DateTime? DATA_INCARCARE { get; set; }
        public long DIMENSIUNE_FISIER { get; set; }
        public int ID_TIP_DOCUMENT { get; set; }
        public int ID_DOSAR { get; set; }
        public bool VIZA_CASCO { get; set; }
        public string DETALII { get; set; }
        public byte[] FILE_CONTENT { get; set; }
        public byte[] SMALL_ICON { get; set; }
        public byte[] MEDIUM_ICON { get; set; }
        public string CALE_FISIER { get; set; }

        /// <summary>
        /// Constructorul default
        /// </summary>
        public DocumentScanat() { }

        public DocumentScanat(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        /// <summary>
        /// Constructor pentru crearea unui obiect pe baza ID-ului unic
        /// </summary>
        /// <param name="_ID">ID-ul unic din baza de date</param>
        public DocumentScanat(int _authenticatedUserId, string _connectionString, int _ID)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOCUMENTE_SCANATEsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
            DbDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord document_scanat = (IDataRecord)r;
                DocumentScanatConstructor(document_scanat);
                break;
            }
        }

        /// <summary>
        /// Constructor cu valorile coloanelor
        /// </summary>
        /// <param name="_ID">ID-ul unic</param>
        /// <param name="_DENUMIRE_FISIER">denumirea unica a fisierului (de pe server - GUID)</param>
        /// <param name="_EXTENSIE_FISIER">extensia fisierului</param>
        /// <param name="_CALE_FISIER">calea fisierului (de pe server)</param>
        /// <param name="_DATA_INCARCARE">data uploadarii fisierului = creation date</param>
        /// <param name="_DENUMIRE_INITIALA">denumirea initiala a fisierului (de la client)</param>
        /// <param name="_DIMENSIUNE_FISIER">dimensiunea in bytes a fisierului</param>
        /// <param name="_ID_TIP_DOCUMENT">id-ul de legatura cu tabela cu tipurile de documente</param>
        public DocumentScanat(int? _ID, string _DENUMIRE_FISIER, string _EXTENSIE_FISIER, DateTime? _DATA_INCARCARE, long _DIMENSIUNE_FISIER, int _ID_TIP_DOCUMENT, int _ID_DOSAR, bool _VIZA_CASCO, string _DETALII, byte[] _FILE_CONTENT, byte[] _SMALL_ICON, byte[] _MEDIUM_ICON, string _CALE_FISIER)
        {
            this.ID = _ID;
            this.DENUMIRE_FISIER = _DENUMIRE_FISIER;
            this.EXTENSIE_FISIER = _EXTENSIE_FISIER;
            this.DATA_INCARCARE = _DATA_INCARCARE;
            this.DIMENSIUNE_FISIER = _DIMENSIUNE_FISIER;
            this.ID_TIP_DOCUMENT = _ID_TIP_DOCUMENT;
            this.ID_DOSAR = _ID_DOSAR;
            this.VIZA_CASCO = _VIZA_CASCO;
            this.DETALII = _DETALII;
            this.SMALL_ICON = _SMALL_ICON;
            this.MEDIUM_ICON = _MEDIUM_ICON;
            this.CALE_FISIER = _CALE_FISIER;
        }

        public DocumentScanat(int _authenticatedUserId, string _connectionString, IDataRecord document_scanat)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DocumentScanatConstructor(document_scanat);
        }

        public void DocumentScanatConstructor(IDataRecord documentScanat)
        {
            try { this.ID = Convert.ToInt32(documentScanat["ID"]); }
            catch { }
            try { this.DENUMIRE_FISIER = documentScanat["DENUMIRE_FISIER"].ToString(); }
            catch { }
            try { this.EXTENSIE_FISIER = documentScanat["EXTENSIE_FISIER"].ToString(); }
            catch { }
            try { this.DATA_INCARCARE = Convert.ToDateTime(documentScanat["DATA_INCARCARE"]); }
            catch { }
            try { this.DIMENSIUNE_FISIER = Convert.ToInt32(documentScanat["DIMENSIUNE_FISIER"]); }
            catch { }
            try { this.ID_TIP_DOCUMENT = Convert.ToInt32(documentScanat["ID_TIP_DOCUMENT"]); }
            catch { }
            try { this.ID_DOSAR = Convert.ToInt32(documentScanat["ID_DOSAR"]); }
            catch { }
            try { this.VIZA_CASCO = Convert.ToBoolean(documentScanat["VIZA_CASCO"]); }
            catch { }
            try { this.DETALII = documentScanat["DETALII"].ToString(); }
            catch { }
            try{ this.FILE_CONTENT = (byte[])documentScanat["FILE_CONTENT"]; }
            catch { }
            try { this.SMALL_ICON = (byte[])documentScanat["SMALL_ICON"]; }
            catch { }
            try { this.MEDIUM_ICON = (byte[])documentScanat["MEDIUM_ICON"]; }
            catch { }
            try { this.CALE_FISIER = documentScanat["CALE_FISIER"].ToString(); }
            catch { }
        }

        /// <summary>
        /// Metoda pentru inserarea Documentului scanat curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Insert()
        {
            //try { if (this.DATA_INCARCARE == new DateTime() || this.DATA_INCARCARE == null) this.DATA_INCARCARE = GetFileCreationDate(); }
            try { if (this.DATA_INCARCARE == new DateTime() || this.DATA_INCARCARE == null) this.DATA_INCARCARE = DateTime.Now; }
            catch { }
            try
            {
                if (this.FILE_CONTENT == null && this.CALE_FISIER != null)
                {
                    //this.FILE_CONTENT = FileManager.GetFileContentFromFile(this.CALE_FISIER);
                    this.FILE_CONTENT = FileManager.UploadFile(this.CALE_FISIER);
                    this.DIMENSIUNE_FISIER = this.FILE_CONTENT.Length;
                    //File.Delete(this.CALE_FISIER); // nu mai stergem, ca ne trebuie si in File Storage !
                }
            }
            catch { }
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "documente_scanate");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOCUMENTE_SCANATEsp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status)
            {
                this.ID = toReturn.InsertedId;
                /*
                try
                {
                    if (System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"] != null && Convert.ToBoolean(System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"]))
                        Mesaje.GenerateAndSendMessage(this.ID, DateTime.Now, "Document nou", Convert.ToInt32(System.Web.HttpContext.Current.Session["AUTHENTICATED_ID"]), (int)Mesaje.Importanta.Low);
                }
                catch { }
                */
            }

            return toReturn;
        }

        public response Insert(ThumbNailSizes[] tSizes)
        {
            response toReturn = this.Insert();
            if(toReturn.Status && toReturn.InsertedId != null)
                toReturn = this.GenerateImgThumbNails(tSizes);
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru modificarea Documentului scanat curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Update()
        {
            //try { if (this.DATA_INCARCARE == new DateTime() || this.DATA_INCARCARE == null) this.DATA_INCARCARE = GetFileCreationDate(); }
            try { if (this.DATA_INCARCARE == new DateTime() || this.DATA_INCARCARE == null) this.DATA_INCARCARE = DateTime.Now; }
            catch { }
            try
            {
                if (this.FILE_CONTENT == null && this.CALE_FISIER != null)
                {
                    //this.FILE_CONTENT = FileManager.GetFileContentFromFile(this.CALE_FISIER);
                    this.FILE_CONTENT = FileManager.UploadFile(this.CALE_FISIER);
                    this.DIMENSIUNE_FISIER = this.FILE_CONTENT.Length;
                    //File.Delete(this.CALE_FISIER);
                }
            }
            catch { }
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "documente_scanate");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOCUMENTE_SCANATEsp_update", _parameters.ToArray());
            toReturn = da.ExecuteUpdateQuery();
            /*
            try
            {
                if (System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"] != null && Convert.ToBoolean(System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"]))
                    Mesaje.GenerateAndSendMessage(this.ID, DateTime.Now, "Document modificat", Convert.ToInt32(System.Web.HttpContext.Current.Session["AUTHENTICATED_ID"]), (int)Mesaje.Importanta.Low);
            }
            catch { }
            */
            return toReturn;
        }

        public response Update(ThumbNailSizes[] tSizes)
        {
            response toReturn = this.Update();
            if (toReturn.Status && toReturn.InsertedId != null)
                toReturn = this.GenerateImgThumbNails(tSizes);
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

        /// <summary>
        /// Metoda pentru stergerea Documentului scanat curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Delete()
        {
            response toReturn = new response(false, "", null, null, new List<Error>());;
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOCUMENTE_SCANATEsp_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            /*
            if (toReturn.Status && _deleteFile)
                FileManager.DeleteFile(this.CALE_FISIER, this.DENUMIRE_FISIER, this.EXTENSIE_FISIER);
            */
            if (toReturn.Status)
            {
                /*
                try
                {
                    if (System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"] != null && Convert.ToBoolean(System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"]))
                        Mesaje.GenerateAndSendMessage(this.ID, DateTime.Now, "Document sters", Convert.ToInt32(System.Web.HttpContext.Current.Session["AUTHENTICATED_ID"]), (int)Mesaje.Importanta.Low);
                }
                catch { }
                */
            }
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru validarea Documentului scanat curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Validare()
        {
            response toReturn = new response(true, "", null, null, new List<Error>());;
            Error err = new Error();
            if (this.DENUMIRE_FISIER == null || this.DENUMIRE_FISIER.Trim() == "")
            {
                toReturn.Status = false;
                err = ErrorParser.ErrorMessage("emptyDenumireFisier");
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

        /// <summary>
        /// Metoda pentru generarea filtrului de cautare/filtrare pe baza coloanelor si a valorilor acestora.
        /// Folosita la cautarea cu TypeAhead
        /// </summary>
        /// <returns>string cu filtrul ce va fi trimis ca parametru in procedura stocata din BD pentru filtrare</returns>
        public string GenerateFilterFromJsonObject()
        {
            return Filtering.GenerateFilterFromJsonObject(this);
        }

        public DateTime GetFileCreationDate(string fileName)
        {
            DateTime toReturn = new DateTime();
            FileInfo fi = new FileInfo(fileName);
            toReturn = fi.CreationTime;
            File.Delete(fileName);
            return toReturn;
        }

        public DateTime GetFileCreationDate()
        {
            FileStream fs = new FileStream(this.DENUMIRE_FISIER, FileMode.Create, FileAccess.ReadWrite);
            fs.Write(this.FILE_CONTENT, 0, this.FILE_CONTENT.Length);
            return GetFileCreationDate(this.DENUMIRE_FISIER);
        }

        public response HasChildrens(string tableName)
        {
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "actions", tableName);
        }

        public response HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "actions", tableName, childrenId);
        }

        public response GetChildrens(string tableName)
        {
            return CommonFunctions.GetChildrens(this, tableName);
        }

        public response GetChildren(string tableName, int childrenId)
        {
            return CommonFunctions.GetChildren(this, tableName, childrenId);
        }

        public response GenerateImgThumbNails(ThumbNailSizes[] tSizes)
        {
            response toReturn = new response(true, "", null, null, new List<Error>());
            foreach(ThumbNailSizes tSize in tSizes)
            {
                if (tSize.thumbNailType == ThumbNailType.Small)
                    this.SMALL_ICON = ThumbNails.GenerateImgThumbNail(this, tSize);
                if (tSize.thumbNailType == ThumbNailType.Medium)
                    this.MEDIUM_ICON = ThumbNails.GenerateImgThumbNail(this, tSize);
            }
            toReturn = this.Update();
            return toReturn;
        }
    }
}