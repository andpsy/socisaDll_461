using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;

namespace SOCISA.Models
{
    public interface IDocumenteScanateRepository
    {
        response GetAll();
        response GetFiltered(string _sort, string _order, string _filter, string _limit);
        response Find(int _id);
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
        response Update(string fieldValueCollection);
        response Update(int id, string fieldValueCollection, object file);
        response Update(string fieldValueCollection, object file);
        response Update(int id, string fieldValueCollection, object file, ThumbNailSizes[] tSizes);
        response Update(string fieldValueCollection, object file, ThumbNailSizes[] tSizes);
        response Delete(DocumentScanat item);
        response HasChildrens(DocumentScanat item, string tableName);
        response HasChildren(DocumentScanat item, string tableName, int childrenId);
        response GetChildrens(DocumentScanat item, string tableName);
        response GetChildren(DocumentScanat item, string tableName, int childrenId);
        response Delete(int _id);
        response HasChildrens(int _id, string tableName);
        response HasChildren(int _id, string tableName, int childrenId);
        response GetChildrens(int _id, string tableName);
        response GetChildren(int _id, string tableName, int childrenId);
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

        public response GetAll()
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
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response GetFiltered(string _sort, string _order, string _filter, string _limit)
        {
            try
            {
                try
                {
                    string newFilter = Filtering.GenerateFilterFromJsonObject(typeof(DocumentScanat), _filter, authenticatedUserId, connectionString);
                    _filter = newFilter == null ? _filter : newFilter;
                }
                catch { }
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
                return new response(true, JsonConvert.SerializeObject(toReturn), toReturn, null, null); 
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
        }

        public response Find(int _id)
        {
            try
            {
                DocumentScanat item = new DocumentScanat(authenticatedUserId, connectionString, _id);
                return new response(true, JsonConvert.SerializeObject(item), item, null, null); ;
            }
            catch (Exception exp) { LogWriter.Log(exp); return new response(false, exp.ToString(), null, null, new System.Collections.Generic.List<Error>() { new Error(exp) }); }
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
            DocumentScanat item = JsonConvert.DeserializeObject<DocumentScanat>(Find(id).Message);
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection)
        {
            DocumentScanat tmpItem = JsonConvert.DeserializeObject<DocumentScanat>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            return JsonConvert.DeserializeObject<DocumentScanat>(Find(Convert.ToInt32(tmpItem.ID)).Message).Update(fieldValueCollection);
        }
        public response Update(int id, string fieldValueCollection, object file)
        {
            DocumentScanat item = JsonConvert.DeserializeObject<DocumentScanat>(Find(id).Message);
            byte[] f = GetBytesFromParameter(file);
            item.FILE_CONTENT = f;
            return item.Update(fieldValueCollection);
        }
        public response Update(string fieldValueCollection, object file)
        {
            DocumentScanat tmpItem = JsonConvert.DeserializeObject<DocumentScanat>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            DocumentScanat item = JsonConvert.DeserializeObject<DocumentScanat>(Find(Convert.ToInt32(tmpItem.ID)).Message);
            byte[] f = GetBytesFromParameter(file);
            item.FILE_CONTENT = f;
            return item.Update(fieldValueCollection);
        }
        public response Update(int id, string fieldValueCollection, object file, ThumbNailSizes[] tSizes)
        {
            DocumentScanat item = JsonConvert.DeserializeObject<DocumentScanat>(Find(id).Message);
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
        public response Update(string fieldValueCollection, object file, ThumbNailSizes[] tSizes)
        {
            DocumentScanat tmpItem = JsonConvert.DeserializeObject<DocumentScanat>(fieldValueCollection); // sa vedem daca merge asa sau trebuie cu JObject
            DocumentScanat item = JsonConvert.DeserializeObject<DocumentScanat>(Find(Convert.ToInt32(tmpItem.ID)).Message);
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
            DocumentScanat item = JsonConvert.DeserializeObject<DocumentScanat>(Find(_id).Message);
            return item.Delete();
        }

        public response HasChildrens(DocumentScanat item, string tableName)
        {
            return item.HasChildrens(tableName);
        }

        public response HasChildren(DocumentScanat item, string tableName, int childrenId)
        {
            return item.HasChildren(tableName, childrenId);
        }

        public response GetChildrens(DocumentScanat item, string tableName)
        {
            return item.GetChildrens(tableName);
        }

        public response GetChildren(DocumentScanat item, string tableName, int childrenId)
        {
            return item.GetChildren(tableName, childrenId);
        }

        public response HasChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<DocumentScanat>(obj.Message).HasChildrens(tableName);
        }
        public response HasChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<DocumentScanat>(obj.Message).HasChildren(tableName, childrenId);
        }
        public response GetChildrens(int _id, string tableName)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<DocumentScanat>(obj.Message).GetChildrens(tableName);
        }
        public response GetChildren(int _id, string tableName, int childrenId)
        {
            var obj = Find(_id);
            return JsonConvert.DeserializeObject<DocumentScanat>(obj.Message).GetChildren(tableName, childrenId);
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
