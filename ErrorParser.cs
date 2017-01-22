﻿using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Xml;

namespace SOCISA
{
    /// <summary>
    /// Clasa pentru definirea unei erori
    /// </summary>
    public class Error
    {
        /// <summary>
        /// ID-ul uni pentru identificarea erorii
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Cod unic pentru identificarea erorii
        /// </summary>
        public string ERROR_CODE { get; set; }
        /// <summary>
        /// Mesajul de eroare
        /// </summary>
        public string ERROR_MESSAGE { get; set; }
        /// <summary>
        /// Denumirea obiectului care a generat eroarea
        /// </summary>
        public string ERROR_OBJECT { get; set; }
        /// <summary>
        /// Tipul erorii
        /// </summary>
        public string ERROR_TYPE { get; set; } // Critical, Warning, Information

        /// <summary>
        /// Constructorul default
        /// </summary>
        public Error() { }
    }
    public static class ErrorParser
    {
        private static Dictionary<int, string> definedErrors = new Dictionary<int, string>();

        public static Dictionary<int, string> DefinedErrors{
            get{
                try
                {
                    definedErrors.Add(1451, "Inregistrarea selectata are referinte in alte tabele si nu poate fi stearsa!");
                }catch{}
                return definedErrors;
    
               }
        }

        public static string ParseError(MySqlException mySqlException){
            try
            {
                return DefinedErrors[mySqlException.Number] != null ? DefinedErrors[mySqlException.Number] : mySqlException.Message;
            }
            catch { return mySqlException.Message; }
        }

        public static string MySqlErrorParser(MySqlException mySqlException)
        {
            return ErrorParser.ParseError(mySqlException);
        }

        /// <summary>
        /// Proprietate care mapeaza tabela din fisierul .xml cu erori predefinite
        /// </summary>
        public static Dictionary<string, Error> ErrorMessages
        {
            get
            {
                Dictionary<string, Error> errorMessages = new Dictionary<string, Error>();
                XmlReader r = XmlReader.Create("ErrorMessages.xml");

                XmlDocument xdoc = new XmlDocument();//xml doc used for xml parsing

                xdoc.Load(r);//loading XML in xml doc

                XmlNodeList xNodelst = xdoc.DocumentElement.SelectNodes("ErrorMessages");//reading node so that we can traverse thorugh the XML

                foreach (XmlNode xNode in xNodelst)//traversing XML 
                {
                    if (xNode.Name == "ErrorMessage" && xNode.HasChildNodes)
                    {
                        string errCode = "";
                        Error err = new Error();
                        foreach (XmlNode child in xNode.ChildNodes)
                        {
                            switch (child.Name)
                            {
                                case "ID":
                                    err.ID = Convert.ToInt32(child.Value);
                                    break;
                                case "ERROR_CODE":
                                    errCode = err.ERROR_CODE = child.Value.ToString();
                                    break;
                                case "ERROR_MESSAGE":
                                    err.ERROR_MESSAGE = child.Value.ToString();
                                    break;
                                case "ERROR_OBJECT":
                                    err.ERROR_OBJECT = child.Value.ToString();
                                    break;
                                case "ERROR_TYPE":
                                    err.ERROR_TYPE = child.Value.ToString();
                                    break;
                            }
                        }
                        errorMessages.Add(errCode, err);
                    }
                }
                return errorMessages;
            }
        }

        public static Error ErrorMessage(string errorCode)
        {
            try
            {
                Error error = new Error();
                ErrorMessages.TryGetValue(errorCode, out error);
                return error;
            }
            catch { return null; }
        }
    }
}
