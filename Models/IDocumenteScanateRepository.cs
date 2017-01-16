using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SOCISA.Models
{
    public interface IDocumenteScanateRepository
    {
        DocumentScanat[] GetAll();
        DocumentScanat[] GetFiltered(string _sort, string _order, string _filter, string _limit);
        DocumentScanat Find(int _id);
        response Insert(DocumentScanat item);
        response Insert(DocumentScanat item, object file);
        response Insert(DocumentScanat item, ThumbNailSizes[] tSizes);
        response Insert(DocumentScanat item, object file, ThumbNailSizes[] tSizes);
        response GenerateThumbNails(DocumentScanat item, ThumbNailSizes[] tSizes);
        response Update(DocumentScanat item);
        response Update(DocumentScanat item, object file);
        response Update(DocumentScanat item, ThumbNailSizes[] tSizes);
        response Update(DocumentScanat item, object file, ThumbNailSizes[] tSizes);
        response Update(int id, string fieldValueCollection);
        response Update(int id, string fieldValueCollection, object file);
        response Update(int id, string fieldValueCollection, object file, ThumbNailSizes[] tSizes);
        response Delete(DocumentScanat item);
        bool HasChildrens(DocumentScanat item, string tableName);
        bool HasChildren(DocumentScanat item, string tableName, int childrenId);
        object[] GetChildrens(DocumentScanat item, string tableName);
        object GetChildren(DocumentScanat item, string tableName, int childrenId);
        response Delete(int _id);
        bool HasChildrens(int _id, string tableName);
        bool HasChildren(int _id, string tableName, int childrenId);
        object[] GetChildrens(int _id, string tableName);
        object GetChildren(int _id, string tableName, int childrenId);
    }

    public class DocumenteScanateRepository : IDocumenteScanateRepository
    {
        private string connectionString;
        private int authenticatedUserId;

        public DocumenteScanateRepository(int _authenticatedUserId, string _connectionString)
        {
            authenticatedUserId = _authenticatedUserId;
            connectionString = _connectionString;
        }

        public DocumentScanat[] GetAll()
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOCUMENTE_SCANATEsp_select", new object[] {
                new MySqlParameter("_SORT", null),
                new MySqlParameter("_ORDER", null),
                new MySqlParameter("_FILTER", null),
                new MySqlParameter("_LIMIT", null) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DocumentScanat a = new DocumentScanat(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DocumentScanat[] toReturn = new DocumentScanat[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DocumentScanat)aList[i];
                return toReturn;
            }
            catch (Exception exp) { LogWriter.Log(exp); return null; }
        }

        public DocumentScanat[] GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                DataAccess da = new DataAccess(authenticatedUserId, connectionString, CommandType.StoredProcedure, "DOCUMENTE_SCANATEsp_select", new object[] {
                new MySqlParameter("_SORT", _sort),
                new MySqlParameter("_ORDER", _order),
                new MySqlParameter("_FILTER", _filter),
                new MySqlParameter("_LIMIT", _limit) });
                ArrayList aList = new ArrayList();
                DbDataReader r = da.ExecuteSelectQuery();
                while (r.Read())
                {
                    DocumentScanat a = new DocumentScanat(authenticatedUserId, connectionString, (IDataRecord)r);
                    aList.Add(a);
                }
                DocumentScanat[] toReturn = new DocumentScanat[aList.Count];
                for (int i = 0; i < aList.Count; i++)
                    toReturn[i] = (DocumentScanat)aList[i];
                return toReturn;
            }
            catch { return null; }
        }

        public DocumentScanat Find(int _id)
        {
            DocumentScanat item = new DocumentScanat(authenticatedUserId, connectionString, _id);
            return item;
        }

        public response Insert(DocumentScanat item)
        {
            return item.Insert();
        }
        public response Insert(DocumentScanat item, ThumbNailSizes[] tSizes)
        {
            return item.Insert(tSizes);
        }
        public response Insert(DocumentScanat item, object file)
        {
            item.FILE_CONTENT = GetBytesFromParameter(file);
            return item.Insert();
        }
        public response Insert(DocumentScanat item, object file, ThumbNailSizes[] tSizes)
        {
            item.FILE_CONTENT = GetBytesFromParameter(file);
            return item.Insert(tSizes);
        }

        public response Update(DocumentScanat item)
        {
            return item.Update();
        }
        public response Update(DocumentScanat item, ThumbNailSizes[] tSizes)
        {
            return item.Update(tSizes);
        }
        public response Update(DocumentScanat item, object file)
        {
            item.FILE_CONTENT = GetBytesFromParameter(file);
            return item.Update();
        }
        public response Update(DocumentScanat item, object file, ThumbNailSizes[] tSizes)
        {
            item.FILE_CONTENT = GetBytesFromParameter(file);
            return item.Update(tSizes);
        }

        public response Update(int id, string fieldValueCollection)
        {
            DocumentScanat item = Find(id);
            return item.Update(fieldValueCollection);
        }
        public response Update(int id, string fieldValueCollection, object file)
        {
            DocumentScanat item = Find(id);
            byte[] f = GetBytesFromParameter(file);
            item.FILE_CONTENT = f;
            return item.Update(fieldValueCollection);
        }
        public response Update(int id, string fieldValueCollection, object file, ThumbNailSizes[] tSizes)
        {
            DocumentScanat item = Find(id);
            byte[] f = GetBytesFromParameter(file);
            item.FILE_CONTENT = f;
            response r = item.Update(fieldValueCollection);
            if (r.Status)
            {
                r = item.Update(tSizes);
                return r;
            }
            else return r;
        }

        public response Delete(DocumentScanat item)
        {
            return item.Delete();
        }
        public response Delete(int _id)
        {
            var obj = Find(_id);
            return obj.Delete();
        }

        public bool HasChildrens(DocumentScanat item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public bool HasChildren(DocumentScanat item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public object[] GetChildrens(DocumentScanat item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public object GetChildren(DocumentScanat item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }

        public bool HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return obj.HasChildrens(tableName);
        }
        public bool HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return obj.HasChildren(tableName, childrenId);
        }
        public object[] GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return obj.GetChildrens(tableName);
        }
        public object GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return obj.GetChildren(tableName, childrenId);
        }
        public response GenerateThumbNails(DocumentScanat item, ThumbNailSizes[] tSizes)
        {
            return item.GenerateImgThumbNails(tSizes);
        }
        public byte[] GetBytesFromParameter(object file)
        {
            byte[] f = null;
            if (file is byte[]) f = (byte[])file;
            if (file is string) f = FileManager.UploadFile(Convert.ToString(file));
            if (file is Microsoft.AspNetCore.Http.IFormFile) f = FileManager.UploadFile((Microsoft.AspNetCore.Http.IFormFile)file);
            return f;
        }
    }
}
