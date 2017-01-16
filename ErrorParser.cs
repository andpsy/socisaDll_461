using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace SOCISA
{
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
    }
}
