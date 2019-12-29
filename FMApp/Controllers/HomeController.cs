using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using System.Net.Http;
using System.Web.Http;
using FMApp.Models;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Azure.Storage.Blobs;
using Models;
using Newtonsoft.Json;
using System.IO;

namespace FMApp.Controllers
{
    public class HomeController : ApiController
    {
        public void Index()
        {
        }

        [HttpPost]
        [Route("posttoazure")]
        public async Task<IHttpActionResult> postToAzureStorage(HttpRequestMessage reqData)
        {
            var req = reqData.Content.ReadAsStringAsync().Result;
            var spendDetails = JsonConvert.DeserializeObject<SpendDetails>(req);
            var blobName = "AccountDetails_" + spendDetails.AccountNo.ToString() + "_" + Guid.NewGuid().ToString() + ".json";
            var azureStorageAccName = ConfigurationManager.AppSettings["storage:account:name"];
            var azureStorageAccKey = ConfigurationManager.AppSettings["storage:account:key1"];
            var azureStorageContainer = ConfigurationManager.AppSettings["storage:account:container"];
            var azureStorageConnString = ConfigurationManager.AppSettings["storage:account:key1:connection"];
            //var blobName = reqData.AccountNo + "_" + DateTime.Now.ToString() + "_" + Guid.NewGuid().ToString();
            //var blobName = DateTime.Now.ToString() + "_" + Guid.NewGuid().ToString();
            var localPath = HttpContext.Current.Server.MapPath("~/App_Data/");
            var localFilePath = Path.Combine(localPath, blobName);
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(localFilePath))
                {
                    await streamWriter.WriteAsync(spendDetails.AccountDetails.ToString());
                }          
                BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(azureStorageContainer);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                //using (StreamReader streamReader = new StreamReader(localFilePath))
                //{
                //    await blobClient.UploadAsync("D:\\Azure\\FMApp\\FMApp\\App_Data\\AccountDetails1234567890.json");
                //}                    
                await blobClient.UploadAsync(localFilePath);
            }
            catch (Exception ex)
            {
                throw ex; 
            }            
            return Ok($"File {blobName} uploaded in cloud");
        }
    }
}
