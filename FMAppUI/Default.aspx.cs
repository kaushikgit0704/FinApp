using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Models;

namespace FMAppUI
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!fileUpload.HasFile)
            //{
            //    btnUpload.Enabled = false;
            //}
            //else
            //{
            //    btnUpload.Enabled = true;
            //}
        }

        protected void btnUpload_click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
               var inputFilePath = HttpContext.Current.Server.MapPath("~/App_Data/input/" + fileUpload.FileName);
               var outputFilePath = HttpContext.Current.Server.MapPath("~/App_Data/output/");
               fileUpload.SaveAs(inputFilePath);
               var jsonString = ConvertExcelToJson(inputFilePath, outputFilePath);
               sendDataToAPI(jsonString); 
            }
        }

        private void sendDataToAPI(string spendData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["serviceURI"].ToString());                     
                var spendDetails = new SpendDetails { AccountNo = txtAcc.Text, AccountDetails = spendData };
                var postTask = client.PostAsJsonAsync<Object>(ConfigurationManager.AppSettings["apiName"].ToString(), spendDetails);
                postTask.Wait();
                if (postTask.Result.IsSuccessStatusCode)
                {
                }
                else
                {
                }
            }
        }

        private string ConvertExcelToJson(string inputFilePath, string outputFilePath)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            List<string> columns = new List<string>();
            Dictionary<string, object> row;

            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException(inputFilePath);
            if (File.Exists(outputFilePath))
                throw new ArgumentException("File exists: " + outputFilePath);

            var cnnStr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;IMEX=1;HDR=NO\"", inputFilePath);
            var cnn = new OleDbConnection(cnnStr);

            var dt = new DataTable();
            try
            {
                cnn.Open();
                var schemaTable = cnn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);                
                string worksheet = schemaTable.Rows[0]["table_name"].ToString().Replace("'", "");
                string sql = String.Format("select * from [{0}]", worksheet);
                var da = new OleDbDataAdapter(sql, cnn);
                da.Fill(dt);

                //Datatable to JSON                    
                for (int count = 0; count < dt.Rows[0].ItemArray.Length; count++)
                {
                    columns.Add(dt.Rows[0].ItemArray[count].ToString());
                }
                dt.Rows.RemoveAt(0);
                foreach (DataRow dr in dt.Rows)
                {                                       
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(columns[col.Ordinal], dr[col]);
                    }
                    rows.Add(row);
                }                
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(outputFilePath, fileUpload.FileName + "_" + Guid.NewGuid() + ".json")))
                {
                    outputFile.WriteLine(JsonConvert.SerializeObject(rows));
                }
            }
            catch (Exception e)
            {
                // ???
                throw e;
            }
            finally
            {
                // free resources
                cnn.Close();
            }            
            return JsonConvert.SerializeObject(rows);
        }
    }
}