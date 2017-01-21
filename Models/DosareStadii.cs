using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace SOCISA.Models
{
    /// <summary>
    /// Clasa care contine definitia obiectului ce mapeaza o inregistrare din tabela cu relatiile dintre Dosare si Stadiile aferente din baza de date
    /// </summary>
    public class DosarStadiu
    {
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }

        public int? ID {get;set;}
        public int ID_DOSAR {get;set;}
        public int ID_STADIU {get;set;}
        public DateTime TERMEN {get; set;}
        public string OBSERVATII { get; set; }
        public DateTime DATA {get; set;}
        public DateTime SCADENTA {get; set;}
        public string ORA { get; set; }
        public DateTime TERMEN_ADMINISTRATIV { get; set; }
        /*
        public StadiiJson Stadiu { get; set; }
        public DosareStadiiSentinteJson[] DosareStadiiSentinte {get; set; }
        public SentinteJson[] Sentinte { get; set; }
        */

        /// <summary>
        /// Constructorul default
        /// </summary>
        public DosarStadiu() { }

        public DosarStadiu(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public DosarStadiu(int _authenticatedUserId, string _connectionString, int _ID)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
            DbDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                DosarStadiuConstructor(item);
                break;
            }
        }

        public DosarStadiu(int _authenticatedUserId, string _connectionString, IDataRecord item)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DosarStadiuConstructor(item);
        }

        /// <summary>
        /// Functie pentru popularea obiectului Dosar-Stadiu, folosita de diferiti constructori
        /// </summary>
        /// <param name="dosarStadiu">Inregistrare din DB cu informatiile obiectului curent</param>
        public void DosarStadiuConstructor(IDataRecord dosarStadiu)
        {
            try { this.ID = Convert.ToInt32(dosarStadiu["ID"]); }
            catch { }
            try { this.ID_DOSAR = Convert.ToInt32(dosarStadiu["ID_DOSAR"]); }
            catch { }
            try { this.ID_STADIU = Convert.ToInt32(dosarStadiu["ID_STADIU"]); }
            catch { }
            try { this.TERMEN = Convert.ToDateTime(dosarStadiu["TERMEN"]); }
            catch { }
            try { this.OBSERVATII = dosarStadiu["OBSERVATII"].ToString(); }
            catch { }
            try { this.DATA = Convert.ToDateTime(dosarStadiu["DATA"]); }
            catch { }
            try { this.SCADENTA = Convert.ToDateTime(dosarStadiu["SCADENTA"]); }
            catch { }
            try { this.ORA = dosarStadiu["ORA"].ToString(); }
            catch { }
            try { this.TERMEN_ADMINISTRATIV = Convert.ToDateTime(dosarStadiu["TERMEN_ADMINISTRATIV"]); }
            catch { }
            /*
            try { this.Stadiu = GetStadiu(); }
            catch { }
            try { this.Sentinte = GetSentinte(); }
            catch { }
            */
        }

        /// <summary>
        /// Metoda pt. popularea Stadiului curent
        /// </summary>
        /// <returns>SOCISA.StadiiJson</returns>
        public Stadiu GetStadiu()
        {
            try
            {
                Stadiu toReturn = new Stadiu(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_STADIU));
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea Sentintelor dosarului
        /// </summary>
        /// <returns>vector de SOCISA.SentinteJson</returns>
        public Sentinta[] GetSentinte()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADII_SENTINTEsp_GetByIdDosarStadiu", new object[] { new MySqlParameter("_ID_DOSAR_STADIU", this.ID) });
                DbDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Sentinta a = new Sentinta(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_SENTINTA"]));
                    aList.Add(a);
                }
                Sentinta[] toReturn = new Sentinta[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Sentinta)aList[i];
                }
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pentru inserarea relatiei Dosar-stadiu curenta
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Insert()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            /*
            if (this.Stadiu != null)
            {
                response toReturnS = this.Stadiu.Insert();
                if (toReturnS.Status && toReturnS.InsertedId != null)
                    this.ID_STADIU = Convert.ToInt32(toReturnS.InsertedId);
            }
            */

            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "dosare_stadii");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status)
            {
                this.ID = toReturn.InsertedId;
                try
                {
                    Dosar d = new Dosar(authenticatedUserId, connectionString, this.ID_DOSAR);
                    d.SetDataUltimeiModificari(DateTime.Now);
                }
                catch { }
            }
            /*
            if (Sentinte != null && Sentinte.Length > 0)
            {
                foreach (SentinteJson sj in Sentinte)
                {
                    response toReturnSj = sj.Insert();
                    if (toReturnSj.Status && toReturnSj.InsertedId != null)
                    {
                        DosareStadiiSentinteJson dssj = new DosareStadiiSentinteJson();
                        dssj.ID_DOSAR_STADIU = Convert.ToInt32(this.ID);
                        dssj.ID_SENTINTA = Convert.ToInt32(toReturnSj.InsertedId);
                        response toReturnDssj = dssj.Insert();
                    }
                }
            }
            */
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru modificarea relatiei Dosar-stadiu curenta
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Update()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "dosare_stadii");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_update", _parameters.ToArray());
            toReturn = da.ExecuteUpdateQuery();
            try
            {
                Dosar d = new Dosar(authenticatedUserId, connectionString, this.ID_DOSAR);
                d.SetDataUltimeiModificari(DateTime.Now);
            }
            catch { }

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
        /// Metoda pentru stergerea relatiei Dosare-stadii curente
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Delete()
        {
            response toReturn = new response(false, "", null, new List<Error>());;
            Sentinta[] sentinte = this.GetSentinte();
            try
            {
                foreach (Sentinta sj in sentinte)
                {
                    DosarStadiuSentinta dssj = new DosarStadiuSentinta();
                    dssj.ID_DOSAR_STADIU = Convert.ToInt32(this.ID);
                    dssj.ID_SENTINTA = Convert.ToInt32(sj.ID);
                    dssj.Delete();
                    sj.Delete();
                }
            }
            catch { }
            
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru validarea relatiei Dosar-stadiu curenta
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Validare()
        {
            response toReturn = new response(true, "", null, new List<Error>());;


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
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "dosare_stadii", tableName);
        }

        public bool HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "dosare_stadii", tableName, childrenId);
        }

        public object[] GetChildrens(string tableName)
        {
            return null;
        }

        public object GetChildren(string tableName, int childrenId)
        {
            return null;
        }
    }
}