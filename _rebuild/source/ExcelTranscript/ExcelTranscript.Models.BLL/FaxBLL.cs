using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ExcelTranscript.Controllers;
using ExcelTranscript.Models.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExcelTranscript.Models.BLL
{
	public class FaxBLL
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		private HttpClient Client { get; set; }

		public string UploadFile(string FileName)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			LogHelper.PrintGenericInfo("Upload File API CAlLLED!!!!!!!!!!!!!!!!!");
			try
			{
				MultipartFormDataContent val = new MultipartFormDataContent();
				string text = "";
				text = FileName;
				LogHelper.PrintGenericInfo("Upload File Name =" + FileName);
				ByteArrayContent val2 = new ByteArrayContent(File.ReadAllBytes(text));
				((HttpHeaders)((HttpContent)val2).Headers).Add("Content-Type", "application/pdf");
				((HttpContent)val).Headers.ContentType.MediaType = "multipart/form-data";
				val.Add((HttpContent)(object)val2, "file", Path.GetFileName(text));
				Client = new HttpClient();
				((HttpHeaders)Client.DefaultRequestHeaders).Add("x-api-key", "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0");
				HttpResponseMessage result = Client.PostAsync("https://api.cocofax.com/v1/file/upload", (HttpContent)(object)val).Result;
				if (result.IsSuccessStatusCode)
				{
					result.EnsureSuccessStatusCode();
					object result2 = result.Content.ReadAsStringAsync().Result;
					LogHelper.PrintError("Upload Doc Api Result=" + result2);
					JObject jObject = JObject.Parse(result2.ToString());
					if (jObject["data"]["fileId"] != null)
					{
						return jObject["data"]["fileId"].ToString();
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

		public string CheckStatus(string FaxID)
		{
			try
			{
				string requestUriString = "https://api.cocofax.com/v1/fax/" + FaxID;
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
				httpWebRequest.Method = "GET";
				httpWebRequest.Accept = "application/json";
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Headers.Add("x-api-key", "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0");
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
				{
					string text = streamReader.ReadToEnd();
					JObject jObject = JObject.Parse(text.ToString());
					if (jObject["message"] != null)
					{
						string value = jObject["data"].ToString();
						dynamic val = JsonConvert.DeserializeObject<WordDocumentController.StatusJson>(value);
						string faxID = val.id;
						string text2 = val.status;
						FileFaxStatu fileFaxStatu = db.FileFaxStatus.Where((FileFaxStatu _a) => _a.FaxID == FaxID && _a.IsActive == true).FirstOrDefault();
						if (fileFaxStatu != null)
						{
							fileFaxStatu.IsActive = false;
							FileFaxStatu fileFaxStatu2 = new FileFaxStatu();
							fileFaxStatu2.AddedOn = DateTime.UtcNow;
							fileFaxStatu2.DateCreated = val.dateCreated;
							fileFaxStatu2.FaxID = faxID;
							fileFaxStatu2.FileID = fileFaxStatu.FileID;
							fileFaxStatu2.PageCount = val.pageCount;
							fileFaxStatu2.ToNumber = val.to;
							fileFaxStatu2.UpDocID = fileFaxStatu.UpDocID;
							fileFaxStatu2.IsActive = true;
							fileFaxStatu2.Status = text2;
							FileFaxStatu entity = fileFaxStatu2;
							db.FileFaxStatus.Add(entity);
							db.SaveChanges();
						}
						return text2;
					}
				}
				return "";
			}
			catch (Exception error)
			{
				LogHelper.PrintError(error);
				return "";
			}
		}

		public async Task RefreshStatusAsync(string FileID)
		{
			try
			{
				string url = "https://api.cocofax.com/v1/fax/" + FileID;
				HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
				httpRequest.Method = "GET";
				httpRequest.Accept = "application/json";
				httpRequest.ContentType = "application/json";
				httpRequest.Headers.Add("x-api-key", "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0");
				using (StreamReader streamReader = new StreamReader((await httpRequest.GetResponseAsync()).GetResponseStream()))
				{
					JObject a = JObject.Parse((await streamReader.ReadToEndAsync()).ToString());
					if (a["message"] != null)
					{
						string status = a["data"].ToString();
						dynamic stuff = JsonConvert.DeserializeObject<WordDocumentController.StatusJson>(status);
						string id = stuff.id;
						string status_ = stuff.status;
						FileFaxStatu faxST = await db.FileFaxStatus.Where((FileFaxStatu _a) => _a.FaxID == FileID && _a.IsActive == true).FirstOrDefaultAsync();
						if (faxST != null)
						{
							faxST.IsActive = false;
							FileFaxStatu fileFaxStatu = new FileFaxStatu
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
				Exception ex2 = ex;
				LogHelper.PrintError(ex2);
			}
		}
	}
}
