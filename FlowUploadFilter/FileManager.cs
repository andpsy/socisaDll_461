using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;

namespace SOCISA
{
    public static class FileManager
    {
        /*
        public static Dictionary<string, byte[]> UploadFile(ICollection<IFormFile> files)
        {
            Dictionary<string, byte[]> toReturn = new Dictionary<string, byte[]>();

            foreach (IFormFile file in files)
            {
                if (file.Length > 0)
                {
                    toReturn.Add(file.FileName, UploadFile(file));
                }
            }
            return toReturn;
        }
        */

        public static byte[] UploadFile(IFormFile file)
        {
            byte[] toReturn = null;
            BinaryReader reader = new BinaryReader(file.OpenReadStream());
            toReturn = reader.ReadBytes((int)file.Length);
            return toReturn;
        }

        public static byte[] UploadFile(string filePath)
        {
            byte[] toReturn = null;
            FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read);
            fs.Read(toReturn, 0, (int)fs.Length);
            fs.Flush();
            fs.Dispose();
            return toReturn;
        }
    }
}
