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
            BinaryReader reader = new BinaryReader(file.OpenReadStream());
            byte[]  toReturn = reader.ReadBytes((int)file.Length);
            return toReturn;
        }

        public static byte[] UploadFile(string filePath)
        {
            string newFilePath = File.Exists(filePath) ? filePath : Path.Combine(CommonFunctions.GetScansFolder(), filePath);
            FileStream fs = File.Open(newFilePath, FileMode.Open, FileAccess.Read);
            byte[] toReturn = new byte[fs.Length];
            fs.Read(toReturn, 0, (int)fs.Length);
            fs.Flush();
            fs.Dispose();
            response r = ThumbNails.GenerateImgThumbNail(filePath);
            return toReturn;
        }

        public static byte[] UploadFile(string filePath, string fileName)
        {
            FileStream fs = File.Open(Path.Combine(filePath, fileName), FileMode.Open, FileAccess.Read);
            byte[] toReturn = new byte[fs.Length];
            fs.Read(toReturn, 0, (int)fs.Length);
            fs.Flush();
            fs.Dispose();
            response r = ThumbNails.GenerateImgThumbNail(filePath, fileName);
            return toReturn;
        }

        public static bool LoadTemplateFileIntoDb(int _authenticatedUserId, string _connectionString, string filePath, string _DETALII)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                int FileSize;
                byte[] rawData;
                FileStream fs;
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                FileSize = (int)fs.Length;

                rawData = new byte[FileSize];
                fs.Read(rawData, 0, FileSize);
                DataAccess da = new DataAccess(_authenticatedUserId, _connectionString, CommandType.StoredProcedure, "TEMPLATESsp_insert", new object[]
                {
                    new MySqlParameter("_DENUMIRE_FISIER", fi.Name),
                    new MySqlParameter("_EXTENSIE_FISIER", fi.Extension),
                    new MySqlParameter("_DIMENSIUNE_FISIER", FileSize),
                    new MySqlParameter("_FILE_CONTENT", rawData),
                    new MySqlParameter("_DETALII", _DETALII)
                });
                response r = da.ExecuteInsertQuery();
                return r.Status;
            }
            catch { return false; }
        }

        public static byte[] GetTemplateFileFromDb(int _authenticatedUserId, string _connectionString, string fileName)
        {
            try
            {
                byte[] rawData;
                DataAccess da = new DataAccess(_authenticatedUserId, _connectionString, CommandType.StoredProcedure, "TEMPLATESsp_GetByName", new object[]
                {
                    new MySqlParameter("_DENUMIRE_FISIER", fileName)
                });
                IDataReader r = da.ExecuteSelectQuery();
                r.Read();
                rawData = (byte[])r["FILE_CONTENT"];
                r.Close(); r.Dispose();
                return rawData;
            }
            catch { return null; }
        }

        public static byte[] GetTemplateFileFromDb(int _authenticatedUserId, string _connectionString, int templateId)
        {
            try
            {
                byte[] rawData;
                DataAccess da = new DataAccess(_authenticatedUserId, _connectionString, CommandType.StoredProcedure, "TEMPLATESsp_GetById", new object[]
                {
                    new MySqlParameter("_ID", templateId)
                });
                IDataReader r = da.ExecuteSelectQuery();
                r.Read();
                rawData = (byte[])r["FILE_CONTENT"];
                r.Close(); r.Dispose();
                return rawData;
            }
            catch { return null; }
        }

        public static byte[] GetFileContentFromFile(string fileName)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                byte[] toReturn = new byte[fs.Length];
                fs.Read(toReturn, 0, (int)fs.Length);
                return toReturn;
            }
            catch { return null; }
        }

        public static byte[] GetFileContentFromDb(int _authenticatedUserId, string _connectionString, int documentScanatId)
        {
            try
            {
                byte[] rawData;
                DataAccess da = new DataAccess(_authenticatedUserId, _connectionString, CommandType.StoredProcedure, "DOCUMENTE_SCANATEsp_GetById", new object[]
                {
                    new MySqlParameter("_ID", documentScanatId)
                });
                IDataReader r = da.ExecuteSelectQuery();
                r.Read();
                rawData = (byte[])r["FILE_CONTENT"];
                r.Close(); r.Dispose();
                return rawData;
            }
            catch { return null; }
        }
    }
}
