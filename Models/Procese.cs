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
    /// Clasa care contine definitia obiectului ce mapeaza tabela cu Procese din baza de date
    /// </summary>
    public class Proces
    {
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }

        public int? ID { get; set; }
        public string NR_INTERN { get; set; }
        public string NR_DOSAR_INSTANTA { get; set; }
        public DateTime? DATA_DEPUNERE { get; set; }
        public string OBSERVATII { get; set; }
        public double? SUMA_SOLICITATA { get; set; }
        public double? PENALITATI { get; set; }
        public double? TAXA_TIMBRU { get; set; }
        public double? TIMBRU_JUDICIAR { get; set; }
        public double? ONORARIU_EXPERT { get; set; }
        public double? ONORARIU_AVOCAT { get; set; }
        public int? ID_INSTANTA { get; set; }
        /* public NomenclatorJson Instanta { get; set; } */
        public int? ID_COMPLET { get; set; }
        /* public NomenclatorJson Complet { get; set; } */
        public int? ID_CONTRACT { get; set; }
        /* public ContracteJson Contract { get; set; } */
        public string STADIU { get; set; }
        public int? ZILE_PENALIZARI { get; set; }
        public double? CHELTUIELI_MICA_PUBLICITATE { get; set; }
        public double? ONORARIU_CURATOR { get; set; }
        public double? ALTE_CHELTUIELI_JUDECATA { get; set; }
        public double? TAXA_TIMBRU_REEXAMINARE { get; set; }
        public string NR_DOSAR_EXECUTARE { get; set; }
        public DateTime? DATA_EXECUTARE { get; set; }
        public double? ONORARIU_AVOCAT_EXECUTARE { get; set; }
        public double? CHELTUIELI_EXECUTARE { get; set; }
        public double? DESPAGUBIRE_ACORDATA { get; set; }
        public double? CHELTUIELI_JUDECATA_ACORDATE { get; set; }
        public bool MONITORIZARE { get; set; }
        public int? ID_TIP_PROCES { get; set; }
        /*
        public NomenclatorJson TipProces { get; set; }
        public ProcesePlatiTaxaTimbruJson[] ProcesePlatiTaxaTimbru { get; set; }
        public PlatiTaxaTimbruJson[] PlatiTaxaTimbru { get; set; }
        */

        /// <summary>
        /// Constructorul default
        /// </summary>
        public Proces() { }

        public Proces(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public Proces(int _authenticatedUserId, string _connectionString, int _ID)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "ACTIONSsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
            DbDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                ProcesConstructor(item);
                break;
            }
        }

        public Proces(int _authenticatedUserId, string _connectionString, IDataRecord item)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            ProcesConstructor(item);
        }

        /// <summary>
        /// Functie pentru popularea obiectului Proces, folosta de diferiti constructori
        /// </summary>
        /// <param name="dosarInstanta">Inregistrare din DB cu informatiile procesului de instanta curent</param>
        public void ProcesConstructor(IDataRecord dosarInstanta)
        {
            try { this.ID = Convert.ToInt32(dosarInstanta["ID"]); }
            catch { }
            try { this.NR_INTERN = dosarInstanta["NR_INTERN"].ToString(); }
            catch { }
            try { this.NR_DOSAR_INSTANTA = dosarInstanta["NR_DOSAR_INSTANTA"].ToString(); }
            catch { }
            try { this.DATA_DEPUNERE = Convert.ToDateTime(dosarInstanta["DATA_DEPUNERE"]); }
            catch { }
            try { this.OBSERVATII = dosarInstanta["OBSERVATII"].ToString(); }
            catch { }
            try { this.SUMA_SOLICITATA = Convert.ToDouble(dosarInstanta["SUMA_SOLICITATA"]); }
            catch { }
            try { this.PENALITATI = Convert.ToDouble(dosarInstanta["PENALITATI"]); }
            catch { }
            try { this.TAXA_TIMBRU = Convert.ToDouble(dosarInstanta["TAXA_TIMBRU"]); }
            catch { }
            try { this.TIMBRU_JUDICIAR = Convert.ToDouble(dosarInstanta["TIMBRU_JUDICIAR"]); }
            catch { }
            try { this.ONORARIU_EXPERT = Convert.ToDouble(dosarInstanta["ONORARIU_EXPERT"]); }
            catch { }
            try { this.ONORARIU_AVOCAT = Convert.ToDouble(dosarInstanta["ONORARIU_AVOCAT"]); }
            catch { }
            try { this.ID_INSTANTA = Convert.ToInt32(dosarInstanta["ID_INSTANTA"]); }
            catch { }
            try { this.ID_COMPLET = Convert.ToInt32(dosarInstanta["ID_COMPLET"]); }
            catch { }
            try { this.ID_CONTRACT = Convert.ToInt32(dosarInstanta["ID_CONTRACT"]); }
            catch { }
            try { this.STADIU = dosarInstanta["STADIU"].ToString(); }
            catch { }
            try { this.ZILE_PENALIZARI = Convert.ToInt32(dosarInstanta["ZILE_PENALIZARI"]); }
            catch { }
            try { this.CHELTUIELI_MICA_PUBLICITATE = Convert.ToDouble(dosarInstanta["CHELTUIELI_MICA_PUBLICITATE"]); }
            catch { }
            try { this.ONORARIU_CURATOR = Convert.ToDouble(dosarInstanta["ONORARIU_CURATOR"]); }
            catch { }
            try { this.ALTE_CHELTUIELI_JUDECATA = Convert.ToDouble(dosarInstanta["ALTE_CHELTUIELI_JUDECATA"]); }
            catch { }
            try { this.TAXA_TIMBRU_REEXAMINARE = Convert.ToDouble(dosarInstanta["TAXA_TIMBRU_REEXAMINARE"]); }
            catch { }
            try { this.NR_DOSAR_EXECUTARE = dosarInstanta["NR_DOSAR_EXECUTARE"].ToString(); }
            catch { }
            try { this.DATA_EXECUTARE = Convert.ToDateTime(dosarInstanta["DATA_EXECUTARE"]); }
            catch { }
            try { this.ONORARIU_AVOCAT_EXECUTARE = Convert.ToDouble(dosarInstanta["ONORARIU_AVOCAT_EXECUTARE"]); }
            catch { }
            try { this.CHELTUIELI_EXECUTARE = Convert.ToDouble(dosarInstanta["CHELTUIELI_EXECUTARE"]); }
            catch { }
            try { this.DESPAGUBIRE_ACORDATA = Convert.ToDouble(dosarInstanta["DESPAGUBIRE_ACORDATA"]); }
            catch { }
            try { this.CHELTUIELI_JUDECATA_ACORDATE = Convert.ToDouble(dosarInstanta["CHELTUIELI_JUDECATA_ACORDATE"]); }
            catch { }
            try { this.MONITORIZARE = Convert.ToBoolean(dosarInstanta["MONITORIZARE"]); }
            catch { }
            try { this.ID_TIP_PROCES = Convert.ToInt32(dosarInstanta["ID_TIP_PROCES"]); }
            catch { }
            /*
            try { this.Instanta = GetInstanta(); }
            catch { }
            try { this.Complet = GetComplet(); }
            catch { }
            try { this.Contract = GetContract(); }
            catch { }
            try { this.TipProces = GetTipProces(); }
            catch { }
            try { this.PlatiTaxaTimbru = GetPlatiTaxaTimbru(); }
            catch { }
            */
        }

        /// <summary>
        /// Metoda pt. popularea Instantei din dosar
        /// </summary>
        /// <returns>SOCISA.NomenclatorJson</returns>
        public Nomenclator GetInstanta()
        {
            try
            {
                Nomenclator toReturn = new Nomenclator(authenticatedUserId, connectionString, "instante", Convert.ToInt32(this.ID_INSTANTA));
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea Completului din dosar
        /// </summary>
        /// <returns>SOCISA.NomenclatorJson</returns>
        public Nomenclator GetComplet()
        {
            try
            {
                Nomenclator toReturn = new Nomenclator(authenticatedUserId, connectionString, "complete", Convert.ToInt32(this.ID_COMPLET));
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea Tipului de proces
        /// </summary>
        /// <returns>SOCISA.NomenclatorJson</returns>
        public Nomenclator GetTipProces()
        {
            try
            {
                Nomenclator toReturn = new Nomenclator(authenticatedUserId, connectionString, "tip_procese", Convert.ToInt32(this.ID_TIP_PROCES));
                return toReturn;
            }
            catch { return null; }
        }

        /*
        /// <summary>
        /// Metoda pt. popularea Contractului de asistenta juridica atasat procesului curent
        /// </summary>
        /// <returns>SOCISA.ContracteJson</returns>
        public ContracteJson GetContract()
        {
            try
            {
                ContracteJson toReturn = new ContracteJson(Convert.ToInt32(this.ID_CONTRACT));
                return toReturn;
            }
            catch { return null; }
        }

        /// <summary>
        /// Metoda pt. popularea platilor pentru taxa de timbru
        /// </summary>
        /// <returns>SOCISA.PlatiTaxaTimbruJson</returns>
        public PlatiTaxaTimbruJson[] GetPlatiTaxaTimbru()
        {
            try
            {
                DataAccess da = new DataAccess(CommandType.StoredProcedure, "PROCESE_PLATI_TAXA_TIMBRUsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_PROCES", this.ID) });
                DataTable procesePlatiTaxaTimbru = da.ExecuteSelectQuery().Tables[0];
                // this.ProcesePlatiTaxaTimbru = SOCISA.ProcesePlatiTaxaTimbru.GetProcesePlatiTaxaTimbru(procesePlatiTaxaTimbru); 

                PlatiTaxaTimbruJson[] toReturn = new PlatiTaxaTimbruJson[procesePlatiTaxaTimbru.Rows.Count];
                for (int i = 0; i < procesePlatiTaxaTimbru.Rows.Count; i++)
                {
                    toReturn[i] = new PlatiTaxaTimbruJson(Convert.ToInt32(procesePlatiTaxaTimbru.Rows[i]["ID_PLATA_TAXA_TIMBRU"]));
                }
                return toReturn;
            }
            catch { return null; }
        }
        */

        /// <summary>
        /// Metoda pentru inserarea procesului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Insert()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }

            #region -- old --
            /* -- insert informatii externe (instanta, complet, contract etc.) -- */
            /*
            if (this.Instanta != null)
            {
                response toReturnPi = this.Instanta.Insert();
                if (toReturnPi.Status && toReturnPi.InsertedId != null)
                    this.ID_INSTANTA = toReturnPi.InsertedId;
            }
            if (this.Complet != null)
            {
                response toReturnPc = this.Complet.Insert();
                if (toReturnPc.Status && toReturnPc.InsertedId != null)
                    this.ID_COMPLET = toReturnPc.InsertedId;
            }
            if (this.Contract != null)
            {
                response toReturnPct = this.Contract.Insert();
                if (toReturnPct.Status && toReturnPct.InsertedId != null)
                    this.ID_CONTRACT = toReturnPct.InsertedId;
            }
            if (this.TipProces != null)
            {
                response toReturnPtp = this.TipProces.Insert();
                if (toReturnPtp.Status && toReturnPtp.InsertedId != null)
                    this.ID_TIP_PROCES = toReturnPtp.InsertedId;
            }
            */
            /* -- end insert informatii externe (instanta, complet, contract etc.) -- */
            #endregion 

            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "procese");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "PROCESEsp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status)
            {
                this.ID = toReturn.InsertedId;
            }
            /*
            if (this.PlatiTaxaTimbru != null && this.PlatiTaxaTimbru.Length > 0)
            {
                foreach (PlatiTaxaTimbruJson pttj in this.PlatiTaxaTimbru)
                {
                    response toReturnPttj = pttj.Insert();
                    if (toReturnPttj.Status)
                    {
                        ProcesePlatiTaxaTimbruJson ppttj = new ProcesePlatiTaxaTimbruJson();
                        ppttj.ID_PROCES = Convert.ToInt32(this.ID);
                        ppttj.ID_PLATA_TAXA_TIMBRU = Convert.ToInt32(toReturnPttj.InsertedId);
                        response toReturnPpttj = ppttj.Insert();
                    }
                }
            }
            */
            return toReturn;
        }

        public response Insert(int _ID_DOSAR)
        {
            response toReturn = Insert();
            if (toReturn.Status)
            {
                this.ID = toReturn.InsertedId;
                try
                {
                        DosarProces dpj = new DosarProces() { ID = null, ID_DOSAR = _ID_DOSAR, ID_PROCES = Convert.ToInt32(this.ID) };
                        response r = dpj.Insert();
                }
                catch { }
            }
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru modificarea procesului curent
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
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "procese");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "PROCESEsp_update", _parameters.ToArray());
            toReturn = da.ExecuteUpdateQuery();
            return toReturn;
        }

        public response Update(int _ID_DOSAR)
        {
            response toReturn = Update();
            if (toReturn.Status)
            {
                try
                {
                    DosarProces dpj = new DosarProces() { ID = null, ID_DOSAR = _ID_DOSAR, ID_PROCES = Convert.ToInt32(this.ID) };
                    response r = dpj.Insert();
                }
                catch { }
            }
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
        /// Metoda pentru stergerea procesului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Delete()
        {
            response toReturn = new response(false, "", null, new List<Error>()); ;
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "PROCESEsp_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }

        public response Delete(int _ID_DOSAR)
        {
            response toReturn = new response(false, "", null, new List<Error>());
            DosarProces dp = new DosarProces(authenticatedUserId, connectionString);
            dp.ID_DOSAR = _ID_DOSAR;
            dp.ID_PROCES = Convert.ToInt32(this.ID);
            toReturn = dp.Delete();
            if (toReturn.Status)
            {
                toReturn = Delete();
            }
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru validarea procesului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Validare()
        {
            response toReturn = new response(true, "", null, new List<Error>()); ;


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

        public bool HasChildrens(string tableName)
        {
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "procese", tableName);
        }

        public bool HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "procese", tableName, childrenId);
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