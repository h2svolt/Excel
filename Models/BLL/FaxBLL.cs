using ExcelTranscript.Models.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using static ExcelTranscript.Controllers.WordDocumentController;

namespace ExcelTranscript.Models.BLL
{
    public class FaxBLL
    {

        private HttpClient Client { get; set; }
        public string UploadFile(string FileName)
        {
            LogHelper.PrintGenericInfo("Upload File API CAlLLED!!!!!!!!!!!!!!!!!");
            try
            {
                // string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                //                var form = new MultipartFormDataContent(boundary);
                var form = new MultipartFormDataContent();
                #region MyRegion
                // File name and path 
                //D:\\Mangotech\\Projects\\ExcelTrans\\From saqib\\exceltranscript\\ExcelTranscript\\Content\\Transcripts
                string sFileName = "";
                //                FileName = "D:\\Mangotech\\Projects\\ExcelTrans\\From saqib\\exceltranscript\\ExcelTranscript\\Content\\Transcripts\\" + FileName; // local
                // FileName = @"D:\Inetpub\vhosts\mangotech-apps.com\excel.mangotech-apps.com\Content\Transcripts\" + FileName; // live path
                sFileName = FileName;
                LogHelper.PrintGenericInfo("Upload File Name =" + FileName);
                var fileContent = new ByteArrayContent(File.ReadAllBytes(sFileName));


                fileContent.Headers.Add("Content-Type", "application/pdf");
                form.Headers.ContentType.MediaType = "multipart/form-data";
                form.Add(fileContent, "file", Path.GetFileName(sFileName));

                #endregion

                // var result = PostFile2("https://api.cocofax.com/v1/file/upload", form);
                Client = new HttpClient();
                Client.DefaultRequestHeaders.Add("x-api-key", "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0");
                var response = Client.PostAsync("https://api.cocofax.com/v1/file/upload", form).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    object data = response.Content.ReadAsStringAsync().Result;
                    LogHelper.PrintError("Upload Doc Api Result=" + data);
                    var a = JObject.Parse(data.ToString());

                    if (a["data"]["fileId"] != null)
                    {
                        var fileID = a["data"]["fileId"].ToString();
                        return fileID;
                    }
                    return null;
                }
                return null;

            }
            catch (Exception ex)
            {
                LogHelper.PrintError("Upload File Error =" + ex);
                return null;
            }
        }

        //        public void SendFax(string File,string ToFax)
        //        {
        //            try
        //            {
        //                var url = "https://api.cocofax.com/v1/fax/send";

        //                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
        //                httpRequest.Method = "POST";

        //                httpRequest.Accept = "application/json";
        //                httpRequest.ContentType = "application/json";
        //                httpRequest.Headers.Add("x-api-key", "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0");

        //                var body = @"{
        //" + "\n" +
        //@"  ""files"": [
        //" + "\n" +
        //@"    ""2022-01-19/a80781ac-f44b-4324-b373-242060d3177a.pdf""  // put uploaded file link here 
        //" + "\n" +
        //@"  ],
        //" + "\n" +
        //@"  ""fromPhone"": ""+13322349040"",
        //" + "\n" +
        //@"  ""texts"": [
        //" + "\n" +
        //@"    ""test""
        //" + "\n" +
        //@"  ],
        //" + "\n" +
        //@"  ""toPhones"": [
        //" + "\n" +
        //@"    ""+17182283603"" // put fax number here 
        //" + "\n" +
        //@"  ]
        //" + "\n" +
        //@"}";

        //                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
        //                {
        //                    streamWriter.Write(body);
        //                }

        //                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
        //                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //                {
        //                    var result = streamReader.ReadToEnd();
        //                }

        //                Console.WriteLine(httpResponse.StatusCode);

        //            }
        //            catch (Exception ex)
        //            {
        //                LogHelper.PrintError(ex);
        //            }
        //        }
        db_ExcelTransEntities db = new db_ExcelTransEntities();
        public string CheckStatus(string FaxID)
        {
            try
            {
                var url = "https://api.cocofax.com/v1/fax/" + FaxID;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";
                httpRequest.Headers.Add("x-api-key", "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0");

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var a = JObject.Parse(result.ToString());

                    if (a["message"] != null)
                    {
                        var status = a["data"].ToString();
                        // return fileID;
                        dynamic stuff = JsonConvert.DeserializeObject<StatusJson>(status);


                        string id = stuff.id;
                        string status_ = stuff.status;


                        //try
                        {
                            var faxST = db.FileFaxStatus.Where(_a => _a.FaxID == FaxID && _a.IsActive == true).FirstOrDefault();
                            if (faxST != null)
                            {
                                faxST.IsActive = false;

                                var fileFaxStatu = new FileFaxStatu
                                {
                                    AddedOn = DateTime.UtcNow,
                                    DateCreated = stuff.dateCreated,
                                    FaxID = id,
                                    FileID = faxST.FileID,
                                    PageCount = stuff.pageCount,
                                    ToNumber = stuff.to,
                                    UpDocID = faxST.UpDocID,
                                    IsActive = true,
                                    Status = status_
                                };
                                db.FileFaxStatus.Add(fileFaxStatu);
                                db.SaveChanges();
                            }
                        }
                        return status_;
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                LogHelper.PrintError(ex);
                return "";
            }
        }

        public async Task RefreshStatusAsync(string FileID)
        {
            try
            {
                var url = "https://api.cocofax.com/v1/fax/" + FileID;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";
                httpRequest.Headers.Add("x-api-key", "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0");

                var httpResponse = await httpRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = await streamReader.ReadToEndAsync();
                    var a = JObject.Parse(result.ToString());

                    if (a["message"] != null)
                    {
                        var status = a["data"].ToString();
                        // return fileID;
                        dynamic stuff = JsonConvert.DeserializeObject<StatusJson>(status);

                        string id = stuff.id;
                        string status_ = stuff.status;

                        var faxST = await db.FileFaxStatus.Where(_a => _a.FaxID == FileID && _a.IsActive == true).FirstOrDefaultAsync();
                        if (faxST != null)
                        {
                            faxST.IsActive = false;

                            var fileFaxStatu = new FileFaxStatu
                            {
                                AddedOn = DateTime.UtcNow,
                                DateCreated = stuff.dateCreated,
                                FaxID = id,
                                FileID = faxST.FileID,
                                PageCount = stuff.pageCount,
                                ToNumber = stuff.to,
                                UpDocID = faxST.UpDocID,
                                IsActive = true,
                                Status = status_
                            };
                            db.FileFaxStatus.Add(fileFaxStatu);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.PrintError(ex);
                // return null;
            }
        }

    }
}