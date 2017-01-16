using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace SOCISA
{
    public interface IFlowJsRepo
    {
        FlowJsPostChunkResponse PostChunk(HttpRequestMessage request, string folder);
        FlowJsPostChunkResponse PostChunk(HttpRequestMessage request, string folder, FlowValidationRules validationRules);
        bool ChunkExists(string folder, HttpRequestMessage request);
    }
    public class FlowJsRepo : IFlowJsRepo
    {
        public FlowJsPostChunkResponse PostChunk(HttpRequestMessage request, string folder)
        {
            return PostChunkBase(request, folder, null);
        }

        public FlowJsPostChunkResponse PostChunk(HttpRequestMessage request, string folder, FlowValidationRules validationRules)
        {
            return PostChunkBase(request, folder, validationRules);
        }

        public bool ChunkExists(string folder, HttpRequestMessage request)
        {
            Uri uri = request.RequestUri;
            //var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            var queryDictionary = new Dictionary<string, object>();

            //var identifier = request.Properties["flowIdentifier"].ToString();
            var identifier = queryDictionary["flowIdentifier"].ToString();
            //var chunkNumber = int.Parse(request.Properties["flowChunkNumber"].ToString());
            var chunkNumber = int.Parse(queryDictionary["flowChunkNumber"].ToString());
            var chunkFullPathName = GetChunkFilename(chunkNumber, identifier, folder);
            return File.Exists(Path.Combine(folder, chunkFullPathName));
        }

        private FlowJsPostChunkResponse PostChunkBase(HttpRequestMessage request, string folder, FlowValidationRules validationRules)
        {
            var chunk = new FlowChunk();
            //var requestIsSane = chunk.ParseForm(request.Form);
            var requestIsSane = chunk.ParseForm((Dictionary<string, object>)request.Properties);
            if (!requestIsSane)
            {
                var errResponse = new FlowJsPostChunkResponse();
                errResponse.Status = PostChunkStatus.Error;
                errResponse.ErrorMessages.Add("damaged");
            }

            List<string> errorMessages = null;
            //var file = request.Files[0];
            var file = File.Create("");

            //var response = new FlowJsPostChunkResponse { FileName = chunk.FileName, Size = chunk.TotalSize };
            var fileExtension = chunk.FileName.Substring(chunk.FileName.LastIndexOf('.'));
            var newFileName = Guid.NewGuid().ToString();
            var newFullFileName = String.Format("{0}{1}", newFileName, fileExtension);
            var response = new FlowJsPostChunkResponse { Path = folder, OriginalFileName = chunk.FileName, FileName = newFileName, FileExtension = fileExtension, Size = chunk.TotalSize, CreationDate = DateTime.Now.ToString() };

            var chunkIsValid = true;
            if (validationRules != null)
                chunkIsValid = chunk.ValidateBusinessRules(validationRules, out errorMessages);

            if (!chunkIsValid)
            {
                response.Status = PostChunkStatus.Error;
                response.ErrorMessages = errorMessages;
                return response;
            }

            var chunkFullPathName = GetChunkFilename(chunk.Number, chunk.Identifier, folder);
            try
            {
                // create folder if it does not exist
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                // save file
                //file.SaveAs(chunkFullPathName);
            }
            catch (Exception)
            {
                throw;
            }

            // see if we have more chunks to upload. If so, return here
            for (int i = 1, l = chunk.TotalChunks; i <= l; i++)
            {
                var chunkNameToTest = GetChunkFilename(i, chunk.Identifier, folder);
                var exists = File.Exists(chunkNameToTest);
                if (!exists)
                {
                    response.Status = PostChunkStatus.PartlyDone;
                    return response;
                }
            }

            // if we are here, all chunks are uploaded
            var fileAry = new List<string>();
            for (int i = 1, l = chunk.TotalChunks; i <= l; i++)
            {
                fileAry.Add("flow-" + chunk.Identifier + "." + i);
            }

            //MultipleFilesToSingleFile(folder, fileAry, chunk.FileName);
            MultipleFilesToSingleFile(folder, fileAry, newFullFileName);

            for (int i = 0, l = fileAry.Count; i < l; i++)
            {
                try
                {
                    File.Delete(Path.Combine(folder, fileAry[i]));
                }
                catch (Exception)
                {
                }
            }

            response.Status = PostChunkStatus.Done;
            return response;

        }



        private static void MultipleFilesToSingleFile(string dirPath, IEnumerable<string> fileAry, string destFile)
        {
            using (var destStream = File.Create(Path.Combine(dirPath, destFile)))
            {
                foreach (string filePath in fileAry)
                {
                    using (var sourceStream = File.OpenRead(Path.Combine(dirPath, filePath)))
                        sourceStream.CopyTo(destStream); // You can pass the buffer size as second argument.
                }
            }
        }

        private string GetChunkFilename(int chunkNumber, string identifier, string folder)
        {
            return Path.Combine(folder, "flow-" + identifier + "." + chunkNumber);
        }
    }
}
