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
                using (StreamWriter w = File.AppendText("ErrorLog.txt"))
                {
                    w.Write(exp.ToString() + "\r\n=====================================================\r\n");
                }
            }
            catch { }
        }
    }
}