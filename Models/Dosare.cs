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
    /// Clasa care contine definitia obiectului ce mapeaza tabela cu Dosare din baza de date
    /// </summary>
    public class Dosar
    {
        const string _TABLE_NAME = "dosare";
        private int authenticatedUserId { get; set; }
        private string connectionString { get; set; }

        public int? ID { get; set; }
        public string NR_SCA { get; set; }
        public DateTime? DATA_SCA { get; set; }
        public int? ID_ASIGURAT_CASCO { get; set; }
        //public string ASIGURAT_CASCO { get; set; }
        /* public AsiguratiJson AsiguratCasco { get; set; } */
        public string NR_POLITA_CASCO { get; set; }
        public int? ID_AUTO_CASCO { get; set; }
        /* public AutoJson AutoCasco { get; set; } */
        //public string NR_AUTO_CASCO { get; set; }
        public int? ID_SOCIETATE_CASCO { get; set; }
        /* public SocietatiAsigurareJson SocietateCasco { get; set; } */
        public string NR_POLITA_RCA { get; set; }
        public int? ID_AUTO_RCA { get; set; }
        //public string NR_AUTO_RCA { get; set; }
        /* public AutoJson AutoRca { get; set; } */
        public double? VALOARE_DAUNA { get; set; }
        public double? VALOARE_REGRES { get; set; }
        public int? ID_INTERVENIENT { get; set; }
        //public string INTERVENIENT { get; set; }
        /* public IntervenientiJson Intervenient { get; set; } */
        public string NR_DOSAR_CASCO { get; set; }
        public double? VMD { get; set; }
        public string OBSERVATII { get; set; }
        public int? ID_SOCIETATE_RCA { get; set; }
        /* public SocietatiAsigurareJson SocietateRca { get; set; } */
        public DateTime? DATA_EVENIMENT { get; set; }
        public double? REZERVA_DAUNA { get; set; }
        public DateTime? DATA_INTRARE_RCA { get; set; }
        public DateTime? DATA_IESIRE_CASCO { get; set; }
        public string NR_INTRARE_RCA { get; set; }
        public string NR_IESIRE_CASCO { get; set; }
        public int? ID_ASIGURAT_RCA { get; set; }
        //public string ASIGURAT_RCA { get; set; }
        /* public AsiguratiJson AsiguratRca { get; set; } */
        public int? ID_TIP_DOSAR { get; set; }
        /* public NomenclatorJson TipDosar { get; set; } */
        public double? SUMA_IBNR { get; set; }
        public DateTime? DATA_AVIZARE { get; set; }
        public DateTime? DATA_NOTIFICARE { get; set; }
        public DateTime? DATA_ULTIMEI_MODIFICARI { get; set; }
        public bool? AVIZAT { get; set; }
        /*
        public DosareProceseJson[] DosareProcese { get; set; }
        public ProceseJson[] Procese { get; set; }
        public DosareStadiiJson[] DosareStadii { get; set; }
        public StadiiJson[] Stadii { get; set; }
        public DosarePlatiJson[] DosarePlati { get; set; }
        public PlatiJson[] Plati { get; set; }
        public DosarePlatiContracteJson[] DosarePlatiContracte { get; set; }
        public PlatiContracteJson[] PlatiContract { get; set; }
        public UtilizatoriDosareJson[] UtilizatoriDosare { get; set; }
        public UtilizatoriJson[] Utilizatori { get; set; }
        public MesajeJson[] Mesaje { get; set; }
        */

        /// <summary>
        /// Constructorul default
        /// </summary>
        public Dosar() { }

        public Dosar(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        /// <summary>
        /// Constructor pentru crearea unui obiect pe baza ID-ului unic
        /// </summary>
        /// <param name="_ID">ID-ul unic din baza de date</param>
        public Dosar(int _auhenticatedUserId, string _connectionString, int _ID) {
            authenticatedUserId = _auhenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
            MySqlDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                DosarConstructor(item);
                break;
            }
            r.Close(); r.Dispose();
        }

        /// <summary>
        /// Constructor pentru crearea unui Dosar cu erori din tabela temporara de import pe baza ID-ului unic
        /// </summary>
        /// <param name="_ID">ID-ul unic din baza de date</param>
        /// <param name="_hasErrors">selector pt. tabela - true - din pending, false - din dosare</param>
        public Dosar(int _auhenticatedUserId, string _connectionString, int _ID, bool _hasErrors)
        {
            authenticatedUserId = _auhenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, _hasErrors ? "PENDING_IMPORT_ERRORSsp_GetById" : "DOSAREsp_GetById", new object[] { new MySqlParameter("_ID", _ID) });
            MySqlDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                DosarConstructor(item);
                break;
            }
            r.Close(); r.Dispose();
        }

        /// <summary>
        /// Constructor pentru crearea unui obiect pe baza numarului Casco
        /// </summary>
        /// <param name="_NR_CASCO">Nr. Casco din baza de date</param>
        public Dosar(int _auhenticatedUserId, string _connectionString, string _NR_CASCO)
        {
            authenticatedUserId = _auhenticatedUserId;
            connectionString = _connectionString;
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_GetByNrCasco", new object[] { new MySqlParameter("_NR_CASCO", _NR_CASCO) });
            MySqlDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                IDataRecord item = (IDataRecord)r;
                DosarConstructor(item);
                break;
            }
            r.Close(); r.Dispose();
        }

        /// <summary>
        /// Constructor pentru crearea unui obiect pe baza unei inregistrari din baza de date
        /// </summary>
        /// <param name="_dosar">Inregistrare din baza de date</param>
        public Dosar(int _authenticatedUserId, string _connectionString, IDataRecord item)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
            DosarConstructor(item);
        }

        /// <summary>
        /// Functie pt. popularea Dosarului curent, folosita de mai multi constructori
        /// </summary>
        /// <param name="_dosar">Inregistrare din DB cu informatiile dosarului curent</param>
        public void DosarConstructor(IDataRecord _dosar)
        {
            try { this.ID = Convert.ToInt32(_dosar["ID"]); }
            catch { }
            try { this.NR_SCA = _dosar["NR_SCA"].ToString(); }
            catch { }
            try { this.DATA_SCA = Convert.ToDateTime(_dosar["DATA_SCA"]); }
            catch { }
            try { this.ID_ASIGURAT_CASCO = Convert.ToInt32(_dosar["ID_ASIGURAT_CASCO"]); }
            catch { }
            //try{this.ASIGURAT_CASCO = _dosar["ASIGURAT_CASCO"].ToString();}catch{}
            /* try { this.AsiguratCasco = GetAsiguratCasco(); } catch { } */
            try { this.NR_POLITA_CASCO = _dosar["NR_POLITA_CASCO"].ToString(); }
            catch { }
            try { this.ID_AUTO_CASCO = Convert.ToInt32(_dosar["ID_AUTO_CASCO"]); }
            catch { }
            //try{this.NR_AUTO_CASCO = _dosar["NR_AUTO_CASCO"].ToString();}catch{}
            try { this.ID_SOCIETATE_CASCO = Convert.ToInt32(_dosar["ID_SOCIETATE_CASCO"]); }
            catch { }
            /* try { this.SocietateCasco = GetSocietateCasco(); } catch { } */
            try { this.NR_POLITA_RCA = _dosar["NR_POLITA_RCA"].ToString(); }
            catch { }
            try { this.ID_AUTO_RCA = Convert.ToInt32(_dosar["ID_AUTO_RCA"]); }
            catch { }
            //try{this.NR_AUTO_RCA = _dosar["NR_AUTO_RCA"].ToString();}catch{}
            try { this.VALOARE_DAUNA = Convert.ToDouble(_dosar["VALOARE_DAUNA"]); }
            catch { }
            try { this.VALOARE_REGRES = Convert.ToDouble(_dosar["VALOARE_REGRES"]); }
            catch { }
            try { this.ID_INTERVENIENT = Convert.ToInt32(_dosar["ID_INTERVENIENT"]); }
            catch { }
            //try{this.INTERVENIENT = _dosar["INTERVENIENT"].ToString();}catch{}
            /* try { this.Intervenient = GetIntervenient(); } catch { } */
            try { this.NR_DOSAR_CASCO = _dosar["NR_DOSAR_CASCO"].ToString(); }
            catch { }
            try { this.VMD = Convert.ToDouble(_dosar["VMD"]); }
            catch { }
            try { this.OBSERVATII = _dosar["OBSERVATII"].ToString(); }
            catch { }
            try { this.ID_SOCIETATE_RCA = Convert.ToInt32(_dosar["ID_SOCIETATE_RCA"]); }
            catch { }
            /* try { this.SocietateRca = GetSocietateRca(); } catch { } */
            try { this.DATA_EVENIMENT = Convert.ToDateTime(_dosar["DATA_EVENIMENT"]); }
            catch { }
            try { this.REZERVA_DAUNA = Convert.ToDouble(_dosar["REZERVA_DAUNA"]); }
            catch { }
            try { this.DATA_INTRARE_RCA = Convert.ToDateTime(_dosar["DATA_INTRARE_RCA"]); }
            catch { }
            try { this.DATA_IESIRE_CASCO = Convert.ToDateTime(_dosar["DATA_IESIRE_CASCO"]); }
            catch { }
            try { this.NR_INTRARE_RCA = _dosar["NR_INTRARE_RCA"].ToString(); }
            catch { }
            try { this.NR_IESIRE_CASCO = _dosar["NR_IESIRE_CASCO"].ToString(); }
            catch { }
            try { this.ID_ASIGURAT_RCA = Convert.ToInt32(_dosar["ID_ASIGURAT_RCA"]); }
            catch { }
            //try{this.ASIGURAT_RCA = _dosar["ASIGURAT_RCA"].ToString();}catch{}
            /* try { this.AsiguratRca = GetAsiguratRca(); } catch { } */
            try { this.ID_TIP_DOSAR = Convert.ToInt32(_dosar["ID_TIP_DOSAR"]); }
            catch { }
            /* try { this.TipDosar = GetTipDosar(); } catch { } */
            try { this.SUMA_IBNR = Convert.ToDouble(_dosar["SUMA_IBNR"]); }
            catch { }
            try { this.DATA_AVIZARE = Convert.ToDateTime(_dosar["DATA_AVIZARE"]); }
            catch { }
            try { this.DATA_NOTIFICARE = Convert.ToDateTime(_dosar["DATA_NOTIFICARE"]); }
            catch { }
            try { this.DATA_ULTIMEI_MODIFICARI = Convert.ToDateTime(_dosar["DATA_ULTIMEI_MODIFICARI"]); }
            catch { }
            try { this.AVIZAT= Convert.ToBoolean(_dosar["AVIZAT"]); }
            catch { }
            /*
            try { this.Procese = GetProcese(); }
            catch { }
            try { this.Stadii = GetStadii(); }
            catch { }
            try { this.AutoCasco = GetAutoCasco(); }
            catch { }
            try { this.AutoRca = GetAutoRca(); }
            catch { }
            try { this.Plati = GetPlati(); }
            catch { }
            try { this.PlatiContract = GetPlatiContracte(); }
            catch { }
            try { this.Utilizatori = GetUtilizatori(); }
            catch { }
            try { this.Mesaje = GetMesaje(); }
            catch { }
            */
        }

        /// <summary>
        /// Metoda pt. popularea Stadiilor Dosarului
        /// </summary>
        /// <returns>vector de SOCISA.StadiiJson</returns>
        public response GetStadii()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID) });
                MySqlDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Stadiu a = new Stadiu(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_STADIU"]));
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                Stadiu[] toReturn = new Stadiu[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Stadiu)aList[i];
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Stadiilor Dosarului
        /// </summary>
        /// <returns>vector de SOCISA.DosareStadiiJson</returns>
        public response GetDosareStadii()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_STADIIsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID) });
                MySqlDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    DosarStadiu a = new DosarStadiu(authenticatedUserId, connectionString, Convert.ToInt32(r["ID"]));
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                DosarStadiu[] toReturn = new DosarStadiu[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (DosarStadiu)aList[i];
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Proceselor asociate Dosarului
        /// </summary>
        /// <returns>vector de SOCISA.Procese</returns>
        public response GetProcese()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSARE_PROCESEsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID) });
                MySqlDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Proces p = new Proces(authenticatedUserId, connectionString, Convert.ToInt32(r["ID"]));
                    aList.Add(p);
                }
                r.Close(); r.Dispose();
                Proces[] toReturn = new Proces[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Proces)aList[i];
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /*
        /// <summary>
        /// Metoda pt. popularea Platilor asociate Dosarului
        /// </summary>
        /// <returns>vector de SOCISA.PlatiJson</returns>
        public response GetPlati()
        {
            try
            {
                DataAccess da = new DataAccess(CommandType.StoredProcedure, "DOSARE_PLATIsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID) });
                DataTable dosarePlati = da.ExecuteSelectQuery().Tables[0];
                // this.DosarePlati = SOCISA.DosarePlati.GetDosarePlati(dosarePlati);

                PlatiJson[] toReturn = new PlatiJson[dosarePlati.Rows.Count];
                for (int i = 0; i < dosarePlati.Rows.Count; i++)
                {
                    toReturn[i] = new PlatiJson(Convert.ToInt32(dosarePlati.Rows[i]["ID_PLATA"]));
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Platilor pe contract asociate Dosarului
        /// </summary>
        /// <returns>vector de SOCISA.PlatiContracteJson</returns>
        public response GetPlatiContracte()
        {
            try
            {
                DataAccess da = new DataAccess(CommandType.StoredProcedure, "DOSARE_PLATI_CONTRACTEsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID) });
                DataTable dosarePlatiContracte = da.ExecuteSelectQuery().Tables[0];
                // this.DosarePlatiContracte = SOCISA.DosarePlatiContracte.GetDosarePlatiContracte(dosarePlatiContracte);

                PlatiContracteJson[] toReturn = new PlatiContracteJson[dosarePlatiContracte.Rows.Count];
                for (int i = 0; i < dosarePlatiContracte.Rows.Count; i++)
                {
                    toReturn[i] = new PlatiContracteJson(Convert.ToInt32(dosarePlatiContracte.Rows[i]["ID_PLATA_CONTRACT"]));
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }
        
        /// <summary>
        /// Metoda pt. popularea Mesajelor carora le este asociat Dosarul
        /// </summary>
        /// <returns>vector de SOCISA.MesajeJson</returns>
        public response GetMesaje()
        {
            try
            {
                DataAccess da = new DataAccess(CommandType.StoredProcedure, "MESAJEsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID) });
                DataTable dosareMesaje = da.ExecuteSelectQuery().Tables[0];

                MesajeJson[] toReturn = new MesajeJson[dosareMesaje.Rows.Count];
                for (int i = 0; i < dosareMesaje.Rows.Count; i++)
                {
                    toReturn[i] = new MesajeJson(Convert.ToInt32(dosareMesaje.Rows[i]["ID_MESAJ"]));
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }
        */

        /// <summary>
        /// Metoda pt. popularea Autoturismului CASCO din Dosar
        /// </summary>
        /// <returns>SOCISA.AutoJson</returns>
        public response GetAutoCasco()
        {
            try
            {
                Auto toReturn = new Auto(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_AUTO_CASCO));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Autoturismului RCA din Dosar
        /// </summary>
        /// <returns>SOCISA.AutoJson</returns>
        public response GetAutoRca()
        {
            try
            {
                Auto toReturn = new Auto(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_AUTO_RCA));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Asiguratului CASCO din Dosar
        /// </summary>
        /// <returns>SOCISA.AsiguratiJson</returns>
        public response GetAsiguratCasco()
        {
            try
            {
                Asigurat toReturn = new Asigurat(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_ASIGURAT_CASCO));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Asiguratului RCA din Dosar
        /// </summary>
        /// <returns>SOCISA.AsiguratiJson</returns>
        public response GetAsiguratRca()
        {
            try
            {
                Asigurat toReturn = new Asigurat(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_ASIGURAT_RCA));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Societatii CASCO din Dosar
        /// </summary>
        /// <returns>SOCISA.SocietatiAsigurareJson</returns>
        public response GetSocietateCasco()
        {
            try
            {
                SocietateAsigurare toReturn = new SocietateAsigurare(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_SOCIETATE_CASCO));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Societatii RCA din Dosar
        /// </summary>
        /// <returns>SOCISA.SocietatiAsigurareJson</returns>
        public response GetSocietateRca()
        {
            try
            {
                SocietateAsigurare toReturn = new SocietateAsigurare(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_SOCIETATE_RCA));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Intervenientului din Dosar
        /// </summary>
        /// <returns>SOCISA.IntervenientiJson</returns>
        public response GetIntervenient()
        {
            try
            {
                Intervenient toReturn = new Intervenient(authenticatedUserId, connectionString, Convert.ToInt32(this.ID_INTERVENIENT));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Tipului Dosarului
        /// </summary>
        /// <returns>SOCISA.NomenclatorJson</returns>
        public response GetTipDosar()
        {
            try
            {
                Nomenclator toReturn = new Nomenclator(authenticatedUserId, connectionString, "tip_dosare", (Convert.ToInt32(this.ID_TIP_DOSAR)));
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pt. popularea Utilizatorilor carora le este assignat Dosarul
        /// </summary>
        /// <returns>vector de SOCISA.UtilizatoriJson</returns>
        public response GetUtilizatori()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "UTILIZATORI_DOSAREsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID) });
                MySqlDataReader r = da.ExecuteSelectQuery();
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
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public response GetDocumente()
        {
            try
            {
                DataAccess da = new DataAccess( authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOCUMENTE_SCANATEsp_GetByIdDosar", new object[] { new MySqlParameter("_ID_DOSAR", this.ID) });
                MySqlDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    DocumentScanat a = new DocumentScanat(authenticatedUserId, connectionString, Convert.ToInt32(r["ID"]));
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                DocumentScanat[] toReturn = new DocumentScanat[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (DocumentScanat)aList[i];
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        /// <summary>
        /// Metoda pentru inserarea Dosarului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Insert()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }
            /* -- insert informatii externe (auto, asigurati, intervenient, tipdosar, societati) -- */
            /*
            if (this.AsiguratCasco != null)
            {
                response toReturnDac = this.AsiguratCasco.Insert();
                if (toReturnDac.Status && toReturnDac.InsertedId != null)
                    this.ID_ASIGURAT_CASCO = toReturnDac.InsertedId;
            }
            if (this.AsiguratRca != null)
            {
                response toReturnDar = this.AsiguratRca.Insert();
                if (toReturnDar.Status && toReturnDar.InsertedId != null)
                    this.ID_ASIGURAT_RCA = toReturnDar.InsertedId;
            }
            if (this.AutoCasco != null)
            {
                response toReturnDac = this.AutoCasco.Insert();
                if (toReturnDac.Status && toReturnDac.InsertedId != null)
                    this.ID_AUTO_CASCO = toReturnDac.InsertedId;
            }
            if (this.AutoRca != null)
            {
                response toReturnDAr = this.AutoRca.Insert();
                if (toReturnDAr.Status && toReturnDAr.InsertedId != null)
                    this.ID_AUTO_RCA = toReturnDAr.InsertedId;
            }
            if (this.SocietateCasco != null)
            {
                response toReturnDsc = this.SocietateCasco.Insert();
                if (toReturnDsc.Status && toReturnDsc.InsertedId != null)
                    this.ID_SOCIETATE_CASCO = toReturnDsc.InsertedId;
            }
            if (this.SocietateRca != null)
            {
                response toReturnDsr = this.SocietateRca.Insert();
                if (toReturnDsr.Status && toReturnDsr.InsertedId != null)
                    this.ID_SOCIETATE_RCA = toReturnDsr.InsertedId;
            }
            if (this.Intervenient != null)
            {
                response toReturnDi = this.Intervenient.Insert();
                if (toReturnDi.Status && toReturnDi.InsertedId != null)
                    this.ID_INTERVENIENT = toReturnDi.InsertedId;
            }
            if (this.TipDosar != null)
            {
                response toReturnDtd = this.TipDosar.Insert();
                if (toReturnDtd.Status && toReturnDtd.InsertedId != null)
                    this.ID_TIP_DOSAR = toReturnDtd.InsertedId;
            }
            */
            /* -- end insert informatii externe (auto, asigurati, intervenient, tipdosar, societati) -- */

            this.DATA_ULTIMEI_MODIFICARI = DateTime.Now;

            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "dosare");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status)
            {
                this.ID = toReturn.InsertedId;
                /*
                try
                {
                    if (System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"] != null && Convert.ToBoolean(System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"]))
                        Mesaje.GenerateAndSendMessage(this.ID, DateTime.Now, "Dosar nou", Convert.ToInt32(System.Web.HttpContext.Current.Session["AUTHENTICATED_ID"]), (int)Mesaje.Importanta.Low);
                }
                catch { }
                */
            }

            #region -- older --
            /* 
            if (this.Procese != null && this.Procese.Length > 0)
            {
                foreach (ProceseJson pj in this.Procese)
                {
                    response toReturnPj = pj.Insert();
                    if (toReturnPj.Status)
                    {
                        DosareProceseJson dpj = new DosareProceseJson();
                        dpj.ID_DOSAR = Convert.ToInt32(this.ID);
                        dpj.ID_PROCES = Convert.ToInt32(toReturnPj.InsertedId);
                        response toReturnDpj = dpj.Insert();
                    }
                }
            }

            if (this.DosareStadii != null && this.DosareStadii.Length > 0)
            {
                foreach (DosareStadiiJson dsj in this.DosareStadii)
                {
                    response toReturnSj = dsj.Stadiu.Insert();
                    if (toReturnSj.Status)
                    {
                        dsj.ID_STADIU = Convert.ToInt32(toReturnSj.InsertedId);
                        dsj.ID_DOSAR = Convert.ToInt32(this.ID);
                        response toReturnDsj = dsj.Insert();
                    }
                }
            }

            if (this.Plati != null && this.Plati.Length > 0)
            {
                foreach (PlatiJson pj in this.Plati)
                {
                    response toReturnPj = pj.Insert();
                    if (toReturnPj.Status)
                    {
                        DosarePlatiJson dpj = new DosarePlatiJson(Convert.ToInt32(toReturnPj.InsertedId));
                        dpj.ID_DOSAR = Convert.ToInt32(this.ID);
                        dpj.ID_PLATA = Convert.ToInt32(toReturnPj.InsertedId);
                        response toReturnDsj = dpj.Insert();
                    }
                }
            }

            if (this.PlatiContract != null && this.PlatiContract.Length > 0)
            {
                foreach (PlatiContracteJson pcj in this.PlatiContract)
                {
                    response toReturnPcj = pcj.Insert();
                    if (toReturnPcj.Status)
                    {
                        DosarePlatiContracteJson dpcj = new DosarePlatiContracteJson();
                        dpcj.ID_DOSAR = Convert.ToInt32(this.ID);
                        dpcj.ID_PLATA_CONTRACT = Convert.ToInt32(toReturnPcj.InsertedId);
                        response toReturnDpcj = dpcj.Insert();
                    }
                }
            }

            if (this.Utilizatori != null && this.Utilizatori.Length > 0)
            {
                foreach (UtilizatoriJson uj in this.Utilizatori)
                {
                    response toReturnUj = uj.Insert();
                    if (toReturnUj.Status)
                    {
                        UtilizatoriDosareJson udj = new UtilizatoriDosareJson();
                        udj.ID_DOSAR = Convert.ToInt32(this.ID);
                        udj.ID_UTILIZATOR = Convert.ToInt32(toReturnUj.InsertedId);
                        response toReturnUdj = udj.Insert();
                    }
                }
            }

            if (this.Mesaje != null && this.Mesaje.Length > 0)
            {
                foreach (MesajeJson mj in this.Mesaje)
                {
                    mj.ID_DOSAR = this.ID;
                    response toReturnMj = mj.Insert();
                }
            }
            */
            #endregion

            return toReturn;
        }

        /// <summary>
        /// Metoda pentru inserarea unui Dosar cu erori, rezultat in urma importului, in tabela temporara
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response InsertWithErrors()
        {
            response toReturn = new response(true, "", null, null, new List<Error>());
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "dosare");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "PENDING_IMPORT_ERRORSsp_insert", _parameters.ToArray());
            toReturn = da.ExecuteInsertQuery();
            if (toReturn.Status)
            {
                this.ID = toReturn.InsertedId;
            }
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru modificarea Dosarului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Update()
        {
            response toReturn = Validare();
            if (!toReturn.Status)
            {
                return toReturn;
            }

            #region -- older --
            /* -- insert informatii externe (auto, asigurati, intervenient, tipdosar, societati) -- */
            /*
            if (this.AsiguratCasco != null)
            {
                response toReturnDac = this.AsiguratCasco.Insert();
                if (toReturnDac.Status && toReturnDac.InsertedId != null)
                    this.ID_ASIGURAT_CASCO = toReturnDac.InsertedId;
            }
            if (this.AsiguratRca != null)
            {
                response toReturnDar = this.AsiguratRca.Insert();
                if (toReturnDar.Status && toReturnDar.InsertedId != null)
                    this.ID_ASIGURAT_RCA = toReturnDar.InsertedId;
            }
            if (this.AutoCasco != null)
            {
                response toReturnDac = this.AutoCasco.Insert();
                if (toReturnDac.Status && toReturnDac.InsertedId != null)
                    this.ID_AUTO_CASCO = toReturnDac.InsertedId;
            }
            if (this.AutoRca != null)
            {
                response toReturnDAr = this.AutoRca.Insert();
                if (toReturnDAr.Status && toReturnDAr.InsertedId != null)
                    this.ID_AUTO_RCA = toReturnDAr.InsertedId;
            }
            if (this.SocietateCasco != null)
            {
                response toReturnDsc = this.SocietateCasco.Insert();
                if (toReturnDsc.Status && toReturnDsc.InsertedId != null)
                    this.ID_SOCIETATE_CASCO = toReturnDsc.InsertedId;
            }
            if (this.SocietateRca != null)
            {
                response toReturnDsr = this.SocietateRca.Insert();
                if (toReturnDsr.Status && toReturnDsr.InsertedId != null)
                    this.ID_SOCIETATE_RCA = toReturnDsr.InsertedId;
            }
            if (this.Intervenient != null)
            {
                response toReturnDi = this.Intervenient.Insert();
                if (toReturnDi.Status && toReturnDi.InsertedId != null)
                    this.ID_INTERVENIENT = toReturnDi.InsertedId;
            }
            */
            /* -- end insert/update external columns -- */
            #endregion 

            this.DATA_ULTIMEI_MODIFICARI = DateTime.Now;

            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "dosare");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_update", _parameters.ToArray());
            toReturn = da.ExecuteUpdateQuery();
            /*
            if (toReturn.Status)
            {
                try
                {
                    if (System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"] != null && Convert.ToBoolean(System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"]))
                        Mesaje.GenerateAndSendMessage(this.ID, DateTime.Now, "Dosar modificat", Convert.ToInt32(System.Web.HttpContext.Current.Session["AUTHENTICATED_ID"]), (int)Mesaje.Importanta.Low);
                }
                catch { }
            }
            */
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
                        if (fieldName.ToUpper() == prop.Name.ToUpper() && fieldName.ToUpper() != "ID")
                        {
                            var tmpVal = prop.PropertyType.FullName.IndexOf("System.String") > -1 ? changes[fieldName] : prop.PropertyType.FullName.IndexOf("System.DateTime") > -1 ? Convert.ToDateTime(changes[fieldName]) : Newtonsoft.Json.JsonConvert.DeserializeObject(changes[fieldName], prop.PropertyType);
                            //var tmpVal = CommonFunctions.ConvertValue(changes[fieldName], prop.PropertyType);
                            prop.SetValue(this, tmpVal);
                            break;
                        }
                    }

                }
                return this.Update();
            }
        }

        /// <summary>
        /// Metoda pentru modificarea unui Dosar cu erori, rezultat in urma importului, in tabela temporara
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response UpdateWithErrors()
        {
            response toReturn = new response(true, "", null, null, new List<Error>());
            PropertyInfo[] props = this.GetType().GetProperties();
            ArrayList _parameters = new ArrayList();
            var col = CommonFunctions.table_columns(authenticatedUserId, connectionString, "dosare");
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
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "PENDING_IMPORT_ERRORSsp_update", _parameters.ToArray());
            toReturn = da.ExecuteUpdateQuery();
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru stergerea Dosarului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Delete()
        {
            //lasam nomenclatoarele asociate
            response toReturn = new response(false, "", null, null, new List<Error>());;
            /*
            foreach (UtilizatoriDosareJson udj in this.UtilizatoriDosare)
            {
                udj.Delete();
            }
            foreach (DosareStadiiJson dsj in this.DosareStadii)
            {
                dsj.Delete();
            }
            foreach (DosareProceseJson dpj in this.DosareProcese)
            {
                dpj.Delete();
            }
            //DELETE PROCESE ???
            foreach (DosarePlatiJson dpj in this.DosarePlati)
            {
                dpj.Delete();
            }
            //DELETE PLATI ???
            foreach (DosarePlatiContracteJson dpcj in this.DosarePlatiContracte)
            {
                dpcj.Delete();
            }
            */
            //DELETE PLATI_CONTRACTE ???

            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            /*
            if (toReturn.Status)
            {
                try
                {
                    if (System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"] != null && Convert.ToBoolean(System.Web.HttpContext.Current.Request.Params["SEND_MESSAGE"]))
                        Mesaje.GenerateAndSendMessage(this.ID, DateTime.Now, "Document nou", Convert.ToInt32(System.Web.HttpContext.Current.Session["AUTHENTICATED_ID"]), (int)Mesaje.Importanta.Low);
                }
                catch { }
            }
            */
            return toReturn;
        }

        /// <summary>
        /// Metoda pentru stergerea unui Dosar cu erori, rezultat in urma importului, in tabela temporara
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response DeleteWithErrors()
        {
            //lasam nomenclatoarele asociate
            response toReturn = new response(true, "", null, null, new List<Error>()); ;
            ArrayList _parameters = new ArrayList();
            _parameters.Add(new MySqlParameter("_ID", this.ID));
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "PENDING_IMPORT_ERRORSsp_delete", _parameters.ToArray());
            toReturn = da.ExecuteDeleteQuery();
            return toReturn;
        }


        /// <summary>
        /// Metoda pentru preluare (import) dosar de la societate externa
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public void Import()
        {
            response r = Insert();
            Log(r);
        }

        /// <summary>
        /// Metoda pentru logarea importului Dosarului curent
        /// </summary>
        public void Log(response r)
        {
            Log(r, DateTime.Now.Date);
        }

        /// <summary>
        /// Metoda pentru logarea importului Dosarului curent
        /// </summary>
        /// <param name="_data_import">Data importului</param>
        public void Log(response r, DateTime _data_import)
        {
            DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_import_log", new object[] { 
                new MySqlParameter("_STATUS", r.Status), 
                new MySqlParameter("_MESSAGE", r.Message), 
                new MySqlParameter("_INSERTED_ID", r.InsertedId), 
                new MySqlParameter("_ERRORS", Newtonsoft.Json.JsonConvert.SerializeObject(r.Error)), 
                new MySqlParameter("_DATA_IMPORT", _data_import) });
            da.ExecuteInsertQuery();
        }

        /// <summary>
        /// Metoda pentru validarea Dosarului curent
        /// </summary>
        /// <returns>SOCISA.response = new object(bool = status, string = error message, int = id-ul cheie returnat)</returns>
        public response Validare()
        {
            /*
            response toReturn = new response(true, "", null, null, new List<Error>());
            Error err = new Error();
            Validation[] validations = Validator.GetTableValidations(_TABLE_NAME);
            if (validations != null && validations.Length > 0) // daca s-au citit validarile din fisier mergem pe fisier
            {
                PropertyInfo[] pis = this.GetType().GetProperties();
                foreach (Validation v in validations)
                {
                    if (v.Active)
                    {
                        foreach (PropertyInfo pi in pis)
                        {
                            if (v.FieldName.ToUpper() == pi.Name.ToUpper())
                            {
                                switch (v.ValidationType)
                                {
                                    case "Mandatory":
                                        if (pi.GetValue(this) == null || pi.GetValue(this).ToString().Trim() == "")
                                        {
                                            toReturn.Status = false;
                                            err = ErrorParser.ErrorMessage(v.ErrorCode);
                                            toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                                            toReturn.InsertedId = null;
                                            toReturn.Error.Add(err);
                                        }
                                        break;
                                    case "Confirmation":
                                        // ... TO DO ...
                                        break;
                                    case "Duplicate":
                                        try
                                        {
                                            Type typeOfThis = this.GetType();
                                            Type propertyType = pi.GetValue(this).GetType();
                                            //ConstructorInfo[] cis = typeOfThis.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                            ConstructorInfo ci = typeOfThis.GetConstructor(new Type[] {Type.GetType("System.Int32"), Type.GetType("System.String"), propertyType });

                                            if (ci != null && ID == null) // doar la insert verificam dublura
                                            {
                                                //Dosar dj = new Dosar(authenticatedUserId, connectionString, pi.GetValue(this).ToString()); // trebuie sa existe constructorul pt. campul trimis ca parametru !!!
                                                dynamic dj = Activator.CreateInstance(typeOfThis, new object[] { authenticatedUserId, connectionString, pi.GetValue(this) });
                                                if (dj != null && dj.ID != null)
                                                {
                                                    toReturn.Status = false;
                                                    err = ErrorParser.ErrorMessage(v.ErrorCode);
                                                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                                                    toReturn.InsertedId = null;
                                                    toReturn.Error.Add(err);
                                                }
                                            }
                                        }
                                        catch { }
                                        break;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            */
            bool succes;
            response toReturn = Validator.Validate(authenticatedUserId, connectionString, this, _TABLE_NAME, out succes);
            if(!succes) // daca nu s-au putut citi validarile din fisier, sau nu sunt definite in fisier, mergem pe varianta hardcodata
            {
                toReturn = new response(true, "", null, null, new List<Error>());
                Error err = new Error();

                if (this.ID_ASIGURAT_CASCO == null || this.ID_ASIGURAT_CASCO <= 0)
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyAsiguratCasco");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }

                if (this.ID_ASIGURAT_RCA == null || this.ID_ASIGURAT_RCA <= 0)
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyAsiguratRca");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }

                if (this.ID_SOCIETATE_CASCO == null || this.ID_SOCIETATE_CASCO <= 0)
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyAsiguratorCasco");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                if (this.ID_SOCIETATE_RCA == null || this.ID_SOCIETATE_RCA <= 0)
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyAsiguratorRca");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                if (this.NR_DOSAR_CASCO == null || this.NR_DOSAR_CASCO.Trim() == "")
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyNrDosarCasco");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                if (this.NR_POLITA_CASCO == null || this.NR_POLITA_CASCO.Trim() == "")
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyNrPolitaCasco");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                if (this.NR_POLITA_RCA == null || this.NR_POLITA_RCA.Trim() == "")
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyNrPolitaRca");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                if (this.ID_AUTO_CASCO == null || this.ID_AUTO_CASCO <= 0)
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyAutoCasco");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                if (this.ID_AUTO_RCA == null || this.ID_AUTO_RCA <= 0)
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyAutoRca");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                if (this.VALOARE_DAUNA == null || this.VALOARE_DAUNA.ToString().Trim() == "")
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyValoareDauna");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                if (this.VALOARE_REGRES == null || this.VALOARE_REGRES.ToString().Trim() == "")
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyValoareRegres");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                if (this.DATA_EVENIMENT == null || this.DATA_EVENIMENT.ToString().Trim() == "")
                {
                    toReturn.Status = false;
                    err = ErrorParser.ErrorMessage("emptyDataEveniment");
                    toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                    toReturn.InsertedId = null;
                    toReturn.Error.Add(err);
                }
                try
                {
                    if (ID == null) // doar la insert verificam dublura
                    {
                        Dosar dj = new Dosar(authenticatedUserId, connectionString, this.NR_DOSAR_CASCO);
                        if (dj != null && dj.ID != null)
                        {
                            toReturn.Status = false;
                            err = ErrorParser.ErrorMessage("dosarExistent");
                            toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                            toReturn.InsertedId = null;
                            toReturn.Error.Add(err);
                        }
                    }
                }
                catch { }
            }
            return toReturn;
        }

        public response ValidareColoane(string fieldValueCollection)
        {
            return CommonFunctions.ValidareColoane(this, fieldValueCollection);
        }

        public response SetDataUltimeiModificari(DateTime _DATA_ULTIMEI_MODIFICARI)
        {
            response toReturn = new response(false, "", null, null, new List<Error>()); ;
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_SetDataUltimeiModificari", new object[] { new MySqlParameter("_ID_DOSAR", ID), new MySqlParameter("_DATA_ULTIMEI_MODIFICARI", _DATA_ULTIMEI_MODIFICARI) });
                toReturn = da.ExecuteUpdateQuery();
            }
            catch { }
            return toReturn;
        }

        public response GetDataUltimeiModificari()
        {
            DateTime? toReturn = null;
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_GetDataUltimeiModificari", new object[] { new MySqlParameter("_ID_DOSAR", ID) });
                toReturn = Convert.ToDateTime(da.ExecuteScalarQuery().Result);
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
        }

        public response GetInvolvedParties()
        {
            Utilizator[] toReturn = null;
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOSAREsp_GetInvolvedParties", new object[] { new MySqlParameter("_ID_DOSAR", ID) });
                MySqlDataReader r = da.ExecuteSelectQuery();
                ArrayList aList = new ArrayList();
                while (r.Read())
                {
                    Utilizator a = new Utilizator(authenticatedUserId, connectionString, Convert.ToInt32(r["ID_UTILIZATOR"]));
                    aList.Add(a);
                }
                r.Close(); r.Dispose();
                toReturn = new Utilizator[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                {
                    toReturn[i] = (Utilizator)aList[i];
                }
                return new response(true, Newtonsoft.Json.JsonConvert.SerializeObject(toReturn), toReturn, null, null);
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new List<Error>() { new Error(exp) }); }
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

        public response HasChildrens(string tableName)
        {
            return CommonFunctions.HasChildrens(authenticatedUserId, connectionString, this, "dosare", tableName);
        }

        public response HasChildren(string tableName, int childrenId)
        {
            return CommonFunctions.HasChildren(authenticatedUserId, connectionString, this, "dosare", tableName, childrenId);
        }

        public response GetChildrens(string tableName)
        {
            return CommonFunctions.GetChildrens(this, tableName);
        }

        public response GetChildren(string tableName, int childrenId)
        {
            return CommonFunctions.GetChildren(this, tableName, childrenId);
        }

        public response ExportDocumenteDosarToPdf()
        {
            return PdfGenerator.ExportDocumenteDosarToPdf(this);
        }
        public response ExportDosarToPdf(string templateFileName)
        {
            return PdfGenerator.ExportDosarToPdf(authenticatedUserId, connectionString, templateFileName, this);
        }
        public response ExportDosarCompletToPdf(string templateFileName)
        {
            return PdfGenerator.ExportDosarCompletToPdf(authenticatedUserId, connectionString, templateFileName, this);
        }
    }
}