using System;
using System.IO;

namespace SOCISA
{
    public static class LogWriter
    {
        public static void Log(Exception exp)
        {
            try
            {
                using (StreamWriter w = File.AppendText(Path.Combine(CommonFunctions.GetLogsFolder(), "ErrorLog.txt")))
                {
                    w.Write(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "\r\n" + exp.ToString() + "\r\n=====================================================\r\n");
                }
            }
            catch { }
        }

        public static void Log(string exp, string file)
        {
            try
            {
                using (StreamWriter w = File.AppendText(Path.Combine(CommonFunctions.GetLogsFolder(), file)))
                {
                    w.Write(exp);
                }
            }
            catch { }
        }
    }
}