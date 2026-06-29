using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using ExcelTranscript.Models;
using ExcelTranscript.Models.BLL;
using ExcelTranscript.Models.Helper;
using HtmlAgilityPack;
using InterFAX.Api;
using Ionic.Zip;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spire.Doc;
using Spire.Doc.Documents;

namespace ExcelTranscript.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private FaxBLL faxBLL = new FaxBLL();

		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public static int CurUserId { get; set; }

		public static string CurUserName { get; set; }

		private string GetCurrentUser()
		{
			IIdentity identity = base.User.Identity;
			string currentUserId = base.User.Identity.GetUserId();
			AspNetUser aspNetUser = db.AspNetUsers.Where((AspNetUser x) => x.Id == currentUserId).FirstOrDefault();
			if (aspNetUser == null)
			{
				return "";
			}
			CurUserName = aspNetUser.UserName;
			CurUserId = aspNetUser.UserId;
			return aspNetUser.UserName;
		}

		public ActionResult Index(bool? btn, string DictatorID, string ClinicID, int? StatusId, DateTime? DOTfromDate, DateTime? DOTtoDate, DateTime? DOSfromDate, DateTime? DOStoDate, DateTime? DOBfromDate, DateTime? DOBtoDate)
		{
			try
			{
				GetCurrentUser();
				if (CurUserId <= 0 || string.IsNullOrEmpty(CurUserName))
				{
					return RedirectToAction("Login", "Account");
				}
				List<int> managerProviders = new List<int>();
				List<int> managerClinics = new List<int>();
				base.ViewBag.StatusId = from s in db.Status.Select((Status sa) => new
					{
						value = sa.Id,
						text = sa.Name
					}).ToList()
					orderby s.value == 4, s.value
					select s;
				if (base.User.IsInRole(UserRoles.Provider))
				{
					base.ViewBag.DictatorID = (from sa in db.Dictators
						where sa.LoginID == CurUserName
						select new
						{
							value = sa.DictatorID,
							text = sa.LoginID
						} into s
						orderby s.text
						select s).ToList();
					int Dictator = (from a in db.Dictators
						where a.LoginID == CurUserName
						select a into s
						select s.AccountID).FirstOrDefault();
					base.ViewBag.GetTypist = (from x in db.stp_GetTypist()
						select new
						{
							value = x.UserId,
							text = x.UserName
						}).ToList();
					base.ViewBag.ClinicID = (from sa in db.Clinics
						where sa.ClinicID == Dictator
						select new
						{
							value = sa.ClinicID,
							text = sa.Name
						} into s
						orderby s.text
						select s).ToList();
				}
				if (base.User.IsInRole(UserRoles.Manager))
				{
					int managerID = db.Managers.Where((Manager m) => m.Username == CurUserName).FirstOrDefault().ManagerID;
					managerClinics = (from m in db.ManagerClinics
						where m.ManagerID == managerID
						select m into x
						select x.ClinicID).ToList();
					managerProviders = (from m in db.ManagerClinics
						where m.ManagerID == managerID
						select m into x
						select x.DictatorID).ToList();
					base.ViewBag.DictatorID = (from sa in db.Dictators
						where managerProviders.Contains(sa.DictatorID)
						select new
						{
							value = sa.DictatorID,
							text = sa.LoginID
						} into s
						orderby s.text
						select s).ToList();
					base.ViewBag.GetTypist = (from x in db.stp_GetTypist()
						select new
						{
							value = x.UserId,
							text = x.UserName
						}).ToList();
					base.ViewBag.ClinicID = (from sa in db.Clinics
						where managerClinics.Contains(sa.ClinicID)
						select new
						{
							value = sa.ClinicID,
							text = sa.Name
						} into s
						orderby s.text
						select s).ToList();
				}
				else if (!base.User.IsInRole(UserRoles.Typist))
				{
					base.ViewBag.DictatorID = (from sa in db.Dictators
						select new
						{
							value = sa.DictatorID,
							text = sa.LoginID
						} into s
						orderby s.text
						select s).ToList();
					base.ViewBag.ClinicID = (from sa in db.Clinics
						select new
						{
							value = sa.ClinicID,
							text = sa.Name
						} into s
						orderby s.text
						select s).ToList();
					base.ViewBag.GetTypist = (from x in db.stp_GetTypist()
						select new
						{
							value = x.UserId,
							text = x.UserName
						}).ToList();
				}
				else
				{
					base.ViewBag.DictatorID = (from sa in db.Dictators
						select new
						{
							value = sa.DictatorID,
							text = sa.LoginID
						} into s
						orderby s.text
						select s).ToList();
					base.ViewBag.ClinicID = (from sa in db.Clinics
						select new
						{
							value = sa.ClinicID,
							text = sa.Name
						} into s
						orderby s.text
						select s).ToList();
					base.ViewBag.GetTypist = (from s in db.stp_GetTypist()
						where s.UserId == CurUserId
						select s into x
						select new
						{
							value = x.UserId,
							text = x.UserName
						}).ToList();
				}
				if (btn.HasValue)
				{
					List<stp_GetDocumentsDataNew_Result> list = new List<stp_GetDocumentsDataNew_Result>();
					DateTime? dateTime = DOTfromDate;
					DateTime? dateTime2 = DOTtoDate;
					if (!dateTime.HasValue)
					{
						dateTime = CustomTimeZone.Get_US_UTC_Time().AddHours(-48.0);
						DOTtoDate = CustomTimeZone.Get_US_UTC_Time();
						DOTfromDate = dateTime;
					}
					if (DOSfromDate.HasValue && !DOStoDate.HasValue)
					{
						DOStoDate = CustomTimeZone.Get_US_UTC_Time();
					}
					if (DOBfromDate.HasValue && !DOBtoDate.HasValue)
					{
						DOStoDate = CustomTimeZone.Get_US_UTC_Time();
					}
					db.Database.CommandTimeout = 60;
					string dIds = "";
					string cIds = "";
					managerProviders.ForEach(delegate(int c)
					{
						dIds = dIds + c + ",";
					});
					managerClinics.ForEach(delegate(int c)
					{
						cIds = cIds + c + ",";
					});
					list = (base.User.IsInRole(UserRoles.Typist) ? (from sa in db.stp_GetDocumentsDataNew(DictatorID, ClinicID, null, StatusId, DOTfromDate, DOTtoDate, DOSfromDate, DOStoDate, DOBfromDate, DOBtoDate)
						where sa.TypistAssignID == CurUserId
						orderby sa.StackMark descending, sa.DOT
						select sa).ToList() : ((!base.User.IsInRole(UserRoles.Manager)) ? (from sa in db.stp_GetDocumentsDataNew(DictatorID, ClinicID, null, StatusId, DOTfromDate, DOTtoDate, DOSfromDate, DOStoDate, DOBfromDate, DOBtoDate)
						orderby sa.StackMark descending, sa.DOT
						select sa).ToList() : (from sa in db.stp_GetDocumentsDataNew(DictatorID, ClinicID, null, StatusId, DOTfromDate, DOTtoDate, DOSfromDate, DOStoDate, DOBfromDate, DOBtoDate)
						orderby sa.StackMark descending, sa.DOT
						select sa).ToList()));
					return PartialView("_Index", list);
				}
				return View();
			}
			catch (Exception ex)
			{
				HelperEmail.SendEmail("Home Index: " + ex.StackTrace + " \n\n\n|||||||||Inner exception: " + ex.InnerException);
				throw;
			}
		}

		[HttpPost]
		public JsonResult UpdateAllStatus(IEnumerable<int?> STRMSelected)
		{
			return Json(1, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult GetStatusValue(int audID, int? DocId, int cond)
		{
			GetCurrentUser();
			string data = "";
			UploadedFile uploadedFile = db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == audID).FirstOrDefault();
			UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.FileID == (int?)audID && (int?)sa.Id == DocId).FirstOrDefault();
			if (uploadedFile != null && uploadedDocument != null)
			{
				switch (cond)
				{
				case 1:
					if (!SaveDocument(uploadedDocument.Id))
					{
						break;
					}
					uploadedFile.StatusId = 5;
					db.Entry(uploadedFile).State = EntityState.Modified;
					db.SaveChanges();
					if (uploadedDocument.IsApproved != true || uploadedDocument.StatusId != 5)
					{
						string fileName = uploadedDocument.FileName;
						string extension = uploadedDocument.Extension;
						DirectoryInfo directoryInfo = new DirectoryInfo(base.Server.MapPath("~\\Content\\Transcripts\\"));
						string text = "";
						FaxBLL faxBLL = new FaxBLL();
						string text2 = faxBLL.UploadFile(string.Concat(directoryInfo, fileName, ".", extension));
						if (text2 != null)
						{
							text = SendFax(text2, uploadedDocument.FaxNumber, uploadedDocument.MRN, uploadedDocument.FileID ?? 0, DocId ?? 0);
						}
						if (text != null && text != "")
						{
							LogHelper.PrintGenericInfo("Fax Sent status=" + text);
							uploadedDocument.Decription = text;
						}
						uploadedDocument.StatusId = 5;
						uploadedDocument.IsApproved = true;
						db.Entry(uploadedDocument).State = EntityState.Modified;
						db.SaveChanges();
						List<UserLog> entities = new List<UserLog>
						{
							new UserLog
							{
								UserLogId = CurUserId,
								UserLogName = CurUserName,
								AudioID = uploadedDocument.FileID,
								FileName = uploadedDocument.FileName,
								FileType = uploadedDocument.Extension,
								DocID = DocId,
								Action = 5,
								Date = CustomTimeZone.Get_US_UTC_Time(),
								StatusId = 5,
								ProviderId = uploadedFile.DictatorID
							},
							new UserLog
							{
								UserLogId = CurUserId,
								UserLogName = CurUserName,
								AudioID = uploadedDocument.FileID,
								FileName = uploadedDocument.FileName,
								FileType = uploadedDocument.Extension,
								DocID = DocId,
								Action = 7,
								Date = CustomTimeZone.Get_US_UTC_Time(),
								StatusId = 5,
								ProviderId = uploadedFile.DictatorID
							}
						};
						db.UserLogs.AddRange(entities);
						db.SaveChanges();
					}
					data = "Green";
					break;
				case 4:
				{
					uploadedFile.StatusId = 4;
					db.Entry(uploadedFile).State = EntityState.Modified;
					db.SaveChanges();
					uploadedDocument.StatusId = 4;
					db.Entry(uploadedDocument).State = EntityState.Modified;
					db.SaveChanges();
					UserLog entity = new UserLog
					{
						UserLogId = CurUserId,
						UserLogName = CurUserName,
						AudioID = uploadedDocument.FileID,
						FileName = uploadedDocument.FileName,
						FileType = uploadedDocument.Extension,
						DocID = uploadedDocument.Id,
						Action = 4,
						Date = CustomTimeZone.Get_US_UTC_Time(),
						StatusId = 4,
						ProviderId = uploadedFile.DictatorID
					};
					db.UserLogs.Add(entity);
					db.SaveChanges();
					data = "Green";
					break;
				}
				default:
					data = "Red";
					break;
				}
			}
			return Json(data);
		}

		public bool OpenFile(string filename)
		{
			try
			{
				if (filename == null)
				{
					return false;
				}
				string fileName = Path.GetFileName(filename);
				string text = fileName.Split('.')[0];
				string text2 = Path.GetExtension(filename.ToLower()).Substring(1);
				if (text2 == "docx" || text2 == "doc")
				{
					bool flag = false;
					object obj = 8;
					string text3 = DateTime.Now.Ticks.ToString();
					FileInfo fileInfo = null;
					FileInfo[] array = null;
					object obj2 = null;
					object obj3 = null;
					string text4 = null;
					string text5 = null;
					DirectoryInfo directoryInfo = new DirectoryInfo(base.Server.MapPath("~\\Content\\TempDownload"));
					if (directoryInfo != null)
					{
						obj2 = base.Server.MapPath("~/Content/TempDownload/") + fileName;
						fileInfo = new FileInfo(obj2.ToString());
						if (fileInfo.Exists)
						{
							flag = true;
							text5 = filename.Split('.')[0];
							obj3 = base.Server.MapPath("~/Content/TempDownload/") + text5 + ".htm";
							text4 = base.Server.MapPath("~/Content/TempDownload/") + text5 + "_files";
							array = directoryInfo.GetFiles("*" + filename + "*.*");
						}
						else
						{
							text5 = filename.Split('.')[0];
							obj3 = base.Server.MapPath("~/Content/Temp/") + text5 + ".htm";
							text4 = base.Server.MapPath("~/Content/Temp/") + text5 + "_files";
							if (!Directory.Exists(base.Server.MapPath("~/Content/Temp/")))
							{
								Directory.CreateDirectory(base.Server.MapPath("~/Content/Temp/"));
							}
							DirectoryInfo directoryInfo2 = new DirectoryInfo(base.Server.MapPath("~\\Content\\Transcripts"));
							array = directoryInfo2.GetFiles("*" + filename + "*.*");
						}
					}
					else
					{
						obj3 = base.Server.MapPath("~/Content/Temp/") + text5 + ".htm";
						text4 = base.Server.MapPath("~/Content/Temp/") + text5 + "_files";
						if (!Directory.Exists(base.Server.MapPath("~/Content/Temp/")))
						{
							Directory.CreateDirectory(base.Server.MapPath("~/Content/Temp/"));
						}
						DirectoryInfo directoryInfo3 = new DirectoryInfo(base.Server.MapPath("~\\Content\\Transcripts"));
						array = directoryInfo3.GetFiles("*" + filename + "*.*");
					}
					array?.FirstOrDefault();
					foreach (FileInfo fileInfo2 in array)
					{
						if (flag)
						{
							obj2 = base.Server.MapPath("~/Content/TempDownload/") + Path.GetFileName(array[0].Name);
							continue;
						}
						obj2 = base.Server.MapPath("~/Content/Temp/") + Path.GetFileName(array[0].Name);
						FileInfo fileInfo3 = new FileInfo(obj2.ToString());
						if (fileInfo3.Exists)
						{
							fileInfo3.Refresh();
							fileInfo3.Delete();
							fileInfo2.CopyTo(obj2.ToString());
						}
						else
						{
							fileInfo2.CopyTo(obj2.ToString());
						}
					}
					Document document = new Document();
					document.LoadFromFile(obj2.ToString());
					foreach (Section section in document.Sections)
					{
						if (section.Body.ChildObjects[0].DocumentObjectType == DocumentObjectType.Paragraph)
						{
							if (string.IsNullOrEmpty((section.Body.ChildObjects[0] as Paragraph).Text.Trim()))
							{
								section.Body.ChildObjects.Remove(section.Body.ChildObjects[0]);
							}
							if (string.IsNullOrEmpty((section.Body.ChildObjects[section.Body.ChildObjects.Count - 1] as Paragraph).Text.Trim()))
							{
								section.Body.ChildObjects.Remove(section.Body.ChildObjects[section.Body.ChildObjects.Count - 1]);
							}
						}
						int num = 0;
						foreach (Paragraph paragraph in document.Sections[0].Paragraphs)
						{
							if (section.Body.ChildObjects[num].DocumentObjectType == DocumentObjectType.Paragraph)
							{
								paragraph.Format.BeforeAutoSpacing = false;
								paragraph.Format.AfterAutoSpacing = false;
								num++;
							}
							else
							{
								num++;
							}
						}
					}
					text = filename.Split('.')[0];
					string text6 = obj2.ToString().Split('.')[0];
					string text7 = "C:\\inetpub\\vhosts\\excelonwork.com\\httpdocs\\Content\\Temp\\";
					string text8 = text7 + text + ".htm";
					string text9 = text7 + text + ".docx";
					HtmlDocument htmlDocument = new HtmlDocument();
					if (ConfigurationManager.AppSettings["appEnv"].ToString() == "Dev")
					{
						document.SaveToFile(obj2.ToString(), FileFormat.Docx2013);
						document.SaveToFile(text6 + ".htm", FileFormat.Html);
						htmlDocument.Load(text6 + ".htm");
					}
					else
					{
						document.SaveToFile(text9, FileFormat.Docx2013);
						document.SaveToFile(text8, FileFormat.Html);
						htmlDocument.Load(text8);
					}
					string text10 = "";
					if (ConfigurationManager.AppSettings["appEnv"].ToString() == "Dev")
					{
						text10 = obj2.ToString().Remove(0, obj2.ToString().IndexOf("Content"));
					}
					else
					{
						int num2 = text9.ToString().IndexOf("Content");
						text10 = text9.ToString().Remove(0, text9.ToString().IndexOf("Content"));
					}
					string imgPath = "/" + text10.Remove(text10.LastIndexOf("\\")).Replace("\\", "/");
					List<HtmlNode> list = new List<HtmlNode>();
					List<HtmlNode> list2 = new List<HtmlNode>();
					if (htmlDocument.DocumentNode.SelectNodes("//img") != null)
					{
						list = htmlDocument.DocumentNode.SelectNodes("//img").ToList();
					}
					if (htmlDocument.DocumentNode.SelectNodes("//link") != null)
					{
						list2 = htmlDocument.DocumentNode.SelectNodes("//link").ToList();
					}
					list2.ForEach(delegate(HtmlNode link)
					{
						string value = link.Attributes["href"].Value;
						link.SetAttributeValue("href", imgPath + "/" + value);
					});
					list.ForEach(delegate(HtmlNode img)
					{
						string value2 = img.Attributes["src"].Value;
						img.SetAttributeValue("src", imgPath + "/" + value2);
					});
					StringWriter stringWriter = new StringWriter();
					HtmlTextWriter writer = new HtmlTextWriter(stringWriter);
					htmlDocument.Save((TextWriter)writer);
					document.Close();
					string fileText = stringWriter.ToString();
					if (AddSign(fileText, filename))
					{
						return true;
					}
				}
			}
			catch (Exception innerException)
			{
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				LogHelper.PrintError("File Error Dated: " + DateTime.Now.ToString() + "->ExMessage->" + innerException.StackTrace);
			}
			return false;
		}

		public bool AddSign(string FileText, string CurFileName)
		{
			try
			{
				string html = FileText;
				HtmlDocument htmlDocument = new HtmlDocument();
				FileText = "<html xmlns:v='urn:schemas-microsoft-com:vml'\r\n                        xmlns: o = 'urn:schemas-microsoft-com:office:office'\r\n                        xmlns: w = 'urn:schemas-microsoft-com:office:word'\r\n                        xmlns: m = 'http://schemas.microsoft.com/office/2007/12/omml'\r\n                        xmlns = 'http://www.w3.org/TR/REC-html40' >\r\n                        <head> " + FileText + "</body> </html>";
				htmlDocument.LoadHtml(html);
				if (!Directory.Exists(base.Server.MapPath("~/Content/TempDownload/")))
				{
					Directory.CreateDirectory(base.Server.MapPath("~/Content/TempDownload/"));
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(base.Server.MapPath("~\\Content\\TempDownload\\"));
				string text = base.Server.MapPath("~/Content/TempDownload/");
				string fileName = string.Concat(directoryInfo, CurFileName);
				string FileName = CurFileName.Split('.')[0];
				string text2 = CurFileName.Split('.')[1];
				FileInfo fileInfo = new FileInfo(fileName);
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
				}
				DirectoryInfo directoryInfo2 = new DirectoryInfo(base.Server.MapPath("~\\Content\\Transcripts\\"));
				fileName = string.Concat(directoryInfo, FileName, ".", text2);
				string text3 = fileName.ToString().Remove(0, fileName.ToString().IndexOf("Content"));
				string text4 = "/" + text3.Remove(text3.LastIndexOf("\\")).Replace("\\", "/");
				List<HtmlNode> list = new List<HtmlNode>();
				List<HtmlNode> list2 = new List<HtmlNode>();
				if (htmlDocument.DocumentNode.SelectNodes("//img") != null)
				{
					list = htmlDocument.DocumentNode.SelectNodes("//img").ToList();
				}
				if (htmlDocument.DocumentNode.SelectNodes("//link") != null)
				{
					list2 = htmlDocument.DocumentNode.SelectNodes("//link").ToList();
				}
				list2.ForEach(delegate(HtmlNode link)
				{
					string value = link.Attributes["href"].Value;
					link.SetAttributeValue("href", value);
				});
				list.ForEach(delegate(HtmlNode img)
				{
					string value2 = img.Attributes["src"].Value;
					img.SetAttributeValue("src", value2);
				});
				Document document = new Document();
				Section section = document.AddSection();
				string fileName2 = string.Concat(directoryInfo2, FileName, ".", text2);
				Document document2 = new Document();
				document2.LoadFromFile(fileName2);
				HeaderFooter header = document2.Sections[0].HeadersFooters.Header;
				HeaderFooter footer = document2.Sections[0].HeadersFooters.Footer;
				Document document3 = new Document();
				Section section2 = document3.AddSection();
				document3.Sections.Add(section2);
				foreach (Section section5 in document.Sections)
				{
					foreach (DocumentObject childObject in header.ChildObjects)
					{
						section5.HeadersFooters.Header.ChildObjects.Add(childObject.Clone());
					}
					foreach (DocumentObject childObject2 in footer.ChildObjects)
					{
						section5.HeadersFooters.Footer.ChildObjects.Add(childObject2.Clone());
						document3.Sections[0].HeadersFooters.Footer.ChildObjects.Add(childObject2.Clone());
					}
				}
				document2.Close();
				document2.Dispose();
				string text5 = "";
				HtmlDocument htmlDocument2 = new HtmlDocument();
				try
				{
					document3.SaveToFile(text + "testttt_footer.htm", FileFormat.Html);
					htmlDocument2.Load(text + "testttt_footer.htm");
				}
				catch (Exception ex)
				{
					string text6 = ex.Message + " | " + ex.StackTrace;
				}
				HtmlDocument htmlDocument3 = new HtmlDocument();
				htmlDocument3 = htmlDocument2;
				HtmlNode htmlNode = htmlDocument3.DocumentNode.SelectSingleNode("//div[contains(@class,'Section0')]");
				foreach (HtmlNode item in (IEnumerable<HtmlNode>)htmlNode.ChildNodes)
				{
					if (htmlDocument.Text.Contains(item.OuterHtml))
					{
						htmlDocument.Text = htmlDocument.Text.Replace(item.OuterHtml, "");
					}
				}
				string text7 = htmlDocument.Text.Replace(htmlNode.InnerHtml, "");
				Paragraph paragraph = section.AddParagraph();
				int lineNum = 0;
				string text8 = "";
				bool flag = true;
				try
				{
					HtmlNode htmlNode2 = htmlDocument.DocumentNode.SelectNodes("//p").Last();
					HtmlNode htmlNode3 = htmlDocument.DocumentNode.SelectSingleNode("//p[contains(@class,'Header')]");
					lineNum = htmlNode3.Line;
					text8 = text7.Replace(htmlNode3.OuterHtml, "");
					IEnumerable<HtmlNode> enumerable = from x in htmlDocument.DocumentNode.SelectNodes("//p")
						where x.Line < lineNum
						select x;
					foreach (HtmlNode item2 in enumerable)
					{
						HtmlNode htmlNode4 = item2;
						text8 = text8.Replace(item2.OuterHtml, "");
					}
					HtmlNode previousSibling = htmlDocument.DocumentNode.SelectSingleNode("//p[contains(@class,'Header')]").PreviousSibling;
					flag = true;
				}
				catch (Exception)
				{
				}
				bool flag2 = true;
				string text9 = "";
				UploadedDocument upldDoc = db.UploadedDocuments.Where((UploadedDocument x) => x.FileName == FileName).FirstOrDefault();
				if (upldDoc != null)
				{
					Dictator dictator = db.Dictators.Where((Dictator x) => (int?)x.DictatorID == upldDoc.UploadedFile.DictatorID).FirstOrDefault();
					if (dictator != null)
					{
						text9 = ((dictator.MiddleName == null) ? (dictator.Prefix + ". " + dictator.FirstName + " " + dictator.LastName) : (dictator.Prefix + ". " + dictator.FirstName + " " + dictator.MiddleName + " " + dictator.LastName));
					}
				}
				string text10 = CustomTimeZone.Get_US_UTC_Time().ToString("MM/dd/yyyy hh:mm:ss tt");
				string text11 = "<p class=\"Normal\" style=\"text-align:left;\"><span style=\"font-size:11pt;font-family:'Times New Roman';mso-fareast-font-family:'Times New Roman';mso-bidi-font-family:'Times New Roman';lang:EN-US;mso-fareast-language:EN-US;mso-ansi-language:AR-SA;\">Electronically Signed By " + text9 + " on " + text10 + " EST.</span></p>";
				string[] array = text8.Split(new string[1] { "<p class=\"Footer\"" }, StringSplitOptions.None);
				text8 = array[0] + text11;
				paragraph.AppendHTML(text8);
				foreach (Section section6 in document.Sections)
				{
					if (section6.Body.ChildObjects[0].DocumentObjectType == DocumentObjectType.Paragraph)
					{
						if (string.IsNullOrEmpty((section6.Body.ChildObjects[0] as Paragraph).Text.Trim()))
						{
							section6.Body.ChildObjects.Remove(section6.Body.ChildObjects[0]);
						}
						if (string.IsNullOrEmpty((section6.Body.ChildObjects[section6.Body.ChildObjects.Count - 1] as Paragraph).Text.Trim()))
						{
							section6.Body.ChildObjects.Remove(section6.Body.ChildObjects[section6.Body.ChildObjects.Count - 1]);
						}
					}
					int num = 0;
					foreach (Paragraph paragraph2 in document.Sections[0].Paragraphs)
					{
						if (section6.Body.ChildObjects[num].DocumentObjectType == DocumentObjectType.Paragraph)
						{
							paragraph2.Format.BeforeAutoSpacing = false;
							paragraph2.Format.AfterAutoSpacing = false;
							document.Sections[0].Paragraphs.Insert(num, paragraph2);
							num++;
						}
						else
						{
							num++;
						}
					}
				}
				document.Styles.ApplyDocDefaultsToNormalStyle();
				document.SaveToFile(text + CurFileName, FileFormat.Docx2013);
				document.Close();
				return true;
			}
			catch (Exception innerException)
			{
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				LogHelper.PrintError("File Error Dated: " + DateTime.Now.ToString() + "->ExMessage->" + innerException.StackTrace);
			}
			return false;
		}

		[HttpPost]
		public JsonResult SetDocumentStatus(int DocID, int cond)
		{
			GetCurrentUser();
			string data = "";
			UploadedDocument document = db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == DocID).FirstOrDefault();
			UploadedFile uploadedFile = db.UploadedFiles.Where((UploadedFile a) => (int?)a.FileID == document.FileID).FirstOrDefault();
			if (document != null)
			{
				switch (cond)
				{
				case 1:
				{
					string fileName = document.FileName;
					int id = document.Id;
					string extension = document.Extension;
					DirectoryInfo directoryInfo = new DirectoryInfo(base.Server.MapPath("~\\Content\\Transcripts\\"));
					if (SaveDocument(id))
					{
						string text = "";
						FaxBLL faxBLL = new FaxBLL();
						string text2 = faxBLL.UploadFile(string.Concat(directoryInfo, fileName, ".", extension));
						if (text2 != null)
						{
							text = SendFax(text2, document.FaxNumber, document.MRN, document.FileID ?? 0, id);
						}
						if (text != null && text != "")
						{
							LogHelper.PrintGenericInfo("Fax Sent status=" + text);
							document.Decription = text;
						}
						document.IsApproved = true;
						document.StatusId = 5;
						db.Entry(document).State = EntityState.Modified;
						db.SaveChanges();
						List<UserLog> entities = new List<UserLog>
						{
							new UserLog
							{
								UserLogId = CurUserId,
								UserLogName = CurUserName,
								AudioID = document.FileID,
								FileName = document.FileName,
								FileType = document.Extension,
								DocID = id,
								Action = 5,
								Date = CustomTimeZone.Get_US_UTC_Time(),
								StatusId = 5,
								ProviderId = uploadedFile.DictatorID
							},
							new UserLog
							{
								UserLogId = CurUserId,
								UserLogName = CurUserName,
								AudioID = document.FileID,
								FileName = document.FileName,
								FileType = document.Extension,
								DocID = id,
								Action = 7,
								Date = CustomTimeZone.Get_US_UTC_Time(),
								StatusId = 5,
								ProviderId = uploadedFile.DictatorID
							}
						};
						db.UserLogs.AddRange(entities);
						db.SaveChanges();
						List<UploadedDocument> list = db.UploadedDocuments.Where((UploadedDocument d) => d.FileID == document.FileID && d.PatientName == document.PatientName).ToList();
						if (list.TrueForAll((UploadedDocument doc) => doc.IsApproved == true || doc.StatusId == 5))
						{
							uploadedFile.StatusId = 5;
							db.Entry(uploadedFile).State = EntityState.Modified;
							db.SaveChanges();
						}
						return Json(data = "Green");
					}
					break;
				}
				case 4:
				{
					document.StatusId = 4;
					db.Entry(document).State = EntityState.Modified;
					db.SaveChanges();
					UserLog entity = new UserLog
					{
						UserLogId = CurUserId,
						UserLogName = CurUserName,
						AudioID = document.FileID,
						FileName = document.FileName,
						FileType = document.Extension,
						DocID = document.Id,
						Action = 4,
						Date = CustomTimeZone.Get_US_UTC_Time(),
						StatusId = 4,
						ProviderId = uploadedFile.DictatorID
					};
					db.UserLogs.Add(entity);
					db.SaveChanges();
					return Json(data = "Green");
				}
				default:
					data = "Red";
					break;
				}
			}
			return Json(data);
		}

		public bool SaveDocument(int docId)
		{
			try
			{
				GetCurrentUser();
				HtmlDocument htmlDocument = new HtmlDocument();
				string text = "";
				UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == docId).FirstOrDefault();
				if (uploadedDocument != null)
				{
					string text2 = uploadedDocument.FileName + "." + uploadedDocument.Extension;
					string text3 = Path.GetExtension(text2.ToLower()).Substring(1);
					if (text3 == "docx" || text3 == "doc")
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(base.Server.MapPath("~\\Content\\TempDownload"));
						if (directoryInfo != null)
						{
							string text4 = base.Server.MapPath("~/Content/TempDownload/") + text2;
							FileInfo fileInfo = new FileInfo(text4.ToString());
							text = ((!fileInfo.Exists) ? uploadedDocument.FilePath : ("/Content/TempDownload/" + text2));
						}
					}
				}
				if (!Directory.Exists(base.Server.MapPath("~/Content/TempDownload/")))
				{
					Directory.CreateDirectory(base.Server.MapPath("~/Content/TempDownload/"));
				}
				DirectoryInfo directoryInfo2 = new DirectoryInfo(base.Server.MapPath("~\\Content\\TempDownload\\"));
				string text5 = base.Server.MapPath("~/Content/TempDownload/");
				string text6 = uploadedDocument.FileName + "." + uploadedDocument.Extension;
				string text7 = string.Concat(directoryInfo2, text6);
				string FileName = uploadedDocument.FileName;
				string extension = uploadedDocument.Extension;
				Document document = new Document();
				Section section = document.AddSection();
				string path = text;
				Document document2 = new Document();
				document2.LoadFromFile(base.Server.MapPath(path));
				foreach (Section section6 in document2.Sections)
				{
					foreach (DocumentObject childObject in section6.Body.ChildObjects)
					{
						document.Sections[0].Body.ChildObjects.Add(childObject.Clone());
					}
				}
				HeaderFooter header = document2.Sections[0].HeadersFooters.Header;
				HeaderFooter footer = document2.Sections[0].HeadersFooters.Footer;
				Document document3 = new Document();
				Section section3 = document3.AddSection();
				document3.Sections.Add(section3);
				foreach (Section section7 in document.Sections)
				{
					foreach (DocumentObject childObject2 in header.ChildObjects)
					{
						section7.HeadersFooters.Header.ChildObjects.Add(childObject2.Clone());
					}
					foreach (DocumentObject childObject3 in footer.ChildObjects)
					{
						section7.HeadersFooters.Footer.ChildObjects.Add(childObject3.Clone());
						document3.Sections[0].HeadersFooters.Footer.ChildObjects.Add(childObject3.Clone());
					}
				}
				document2.Close();
				document2.Dispose();
				Paragraph paragraph = section.AddParagraph();
				string text8 = "";
				UploadedDocument upldDoc = db.UploadedDocuments.Where((UploadedDocument x) => x.FileName == FileName).FirstOrDefault();
				if (upldDoc != null && upldDoc.IsApproved != true)
				{
					Dictator dictator = db.Dictators.Where((Dictator x) => (int?)x.DictatorID == upldDoc.UploadedFile.DictatorID).FirstOrDefault();
					if (dictator != null)
					{
						text8 = ((dictator.MiddleName == null) ? (dictator.Prefix + ". " + dictator.FirstName + " " + dictator.LastName) : (dictator.Prefix + ". " + dictator.FirstName + " " + dictator.MiddleName + " " + dictator.LastName));
						string text9 = CustomTimeZone.Get_US_UTC_Time().ToString("MM/dd/yyyy hh:mm:ss tt");
						string text10 = "Electronically Signed By " + text8 + " on " + text9 + " EST.";
						paragraph.AppendText(text10);
					}
				}
				Style style = document.Styles.FindByName("FontStyle");
				if (style != null)
				{
					paragraph.ApplyStyle(style.Name);
				}
				else
				{
					ParagraphStyle paragraphStyle = new ParagraphStyle(document)
					{
						Name = "FontStyle"
					};
					paragraphStyle.CharacterFormat.FontName = "Times New Roman";
					paragraphStyle.CharacterFormat.FontSize = 11f;
					document.Styles.Add(paragraphStyle);
					paragraph.ApplyStyle(paragraphStyle.Name);
				}
				foreach (Section section8 in document.Sections)
				{
					DocumentObjectType documentObjectType = section8.Body.ChildObjects[0].DocumentObjectType;
					if (documentObjectType == DocumentObjectType.Paragraph)
					{
						if (string.IsNullOrEmpty((section8.Body.ChildObjects[0] as Paragraph).Text.Trim()))
						{
							section8.Body.ChildObjects.Remove(section8.Body.ChildObjects[0]);
						}
						if (string.IsNullOrEmpty((section8.Body.ChildObjects[section8.Body.ChildObjects.Count - 1] as Paragraph).Text.Trim()))
						{
							section8.Body.ChildObjects.Remove(section8.Body.ChildObjects[section8.Body.ChildObjects.Count - 1]);
						}
					}
					else if (string.IsNullOrEmpty((section8.Body.ChildObjects[section8.Body.ChildObjects.Count - 1] as Paragraph).Text.Trim()))
					{
						section8.Body.ChildObjects.Remove(section8.Body.ChildObjects[section8.Body.ChildObjects.Count - 1]);
					}
					int num = 0;
					foreach (Paragraph paragraph4 in document.Sections[0].Paragraphs)
					{
						if (section8.Body.ChildObjects[num].DocumentObjectType == DocumentObjectType.Paragraph)
						{
							Paragraph paragraph3 = section8.Body.ChildObjects[num] as Paragraph;
							paragraph3.Format.BeforeAutoSpacing = false;
							paragraph3.Format.AfterAutoSpacing = false;
							if (paragraph3.CharCount <= 0)
							{
								paragraph3.Text = "";
							}
							if (Regex.IsMatch(paragraph3.Text, "((\\d).[\\s]{2,})"))
							{
								string text11 = Regex.Replace(paragraph3.Text, "((\\d).[\\s]{2,})", "");
								paragraph3.Text = text11;
								paragraph3.ListFormat.ApplyNumberedStyle();
								paragraph3.ApplyStyle("FontStyle");
							}
							document.Sections[0].Paragraphs.Insert(num, paragraph3);
							num++;
						}
						else
						{
							num++;
						}
					}
				}
				document.Styles.ApplyDocDefaultsToNormalStyle();
				document.SaveToFile(text5 + text6, FileFormat.Docx2013);
				document.Close();
				return true;
			}
			catch (Exception ex)
			{
				LogHelper.PrintError("Save Document Errorrrrrrr=  " + ex.StackTrace + "Message =>" + ex.Message);
				return false;
			}
		}

		[HttpPost]
		public async Task<JsonResult> StackMark(int AudioID)
		{
			UploadedFile UpdateStatus = await db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == AudioID).FirstOrDefaultAsync();
			bool IsCheck = false;
			if (UpdateStatus != null)
			{
				if (UpdateStatus.StackMark.GetValueOrDefault() == 0)
				{
					UpdateStatus.StackMark = 1;
					IsCheck = true;
				}
				else
				{
					UpdateStatus.StackMark = 0;
					IsCheck = false;
				}
				await db.SaveChangesAsync();
			}
			return Json(IsCheck);
		}

		[HttpPost]
		public async Task<JsonResult> GetPatientDetails(int AudioID, string PatientName)
		{
			try
			{
				PatientName = PatientName.Trim();
				object details = new object();
				List<object> arr = new List<object>();
				DateTimeFormatInfo customFormatInfo = new DateTimeFormatInfo
				{
					DateSeparator = "/"
				};
				UploadedDocument document = await db.UploadedDocuments.Where((UploadedDocument sa) => sa.FileID == (int?)AudioID && sa.PatientName.Contains(PatientName)).FirstOrDefaultAsync();
				var data = await (from sa in db.UploadedDocuments
					where sa.FileID == (int?)AudioID && sa.FileName == document.FileName
					select sa into a
					select new
					{
						DocId = a.Id,
						FileName = a.FileName,
						UploadOn = a.UploadOn,
						FilePath = a.FilePath,
						ext = a.Extension,
						DocChildID = a.DocChildID
					} into sa
					orderby sa.FileName
					select sa).ToListAsync();
				Modality modality = await db.Modalities.Where((Modality m) => (int?)m.Id == document.ModalityId).FirstOrDefaultAsync();
				if (document != null)
				{
					if (DateTime.TryParse(document.DOB, customFormatInfo, DateTimeStyles.None, out var sampleDate))
					{
						document.DOB = sampleDate.ToString("MM/dd/yyyy", customFormatInfo);
					}
					else
					{
						document.DOB = "";
					}
					if (DateTime.TryParse(document.DOS, customFormatInfo, DateTimeStyles.None, out var dosDate))
					{
						document.DOS = dosDate.ToString("MM/dd/yyyy", customFormatInfo);
					}
					else
					{
						document.DOS = "";
					}
					details = new
					{
						Modality = ((modality == null) ? "N/A" : modality.ModalityName),
						FaxNumber = (string.IsNullOrWhiteSpace(document.FaxNumber) ? "N/A" : document.FaxNumber),
						RefDoctor = (string.IsNullOrWhiteSpace(document.RefDoctor) ? "N/A" : document.RefDoctor),
						DOS = (string.IsNullOrWhiteSpace(document.DOS) ? "N/A" : document.DOS),
						DOB = (string.IsNullOrWhiteSpace(document.DOB) ? "N/A" : document.DOB),
						MRN = (string.IsNullOrWhiteSpace(document.MRN) ? "N/A" : document.MRN),
						PatientName = (string.IsNullOrWhiteSpace(document.PatientName) ? "N/A" : document.PatientName)
					};
				}
				if (data != null)
				{
					foreach (var i in data)
					{
						string[] fileArray = i.FileName.Split('.');
						arr.Add(new
						{
							DocId = i.DocId,
							FileName = fileArray[0].ToString(),
							Uploaded = $"{i.UploadOn:MM/dd/yyyy}",
							FilePath = i.FilePath,
							ext = i.ext,
							DocChildID = i.DocChildID
						});
					}
				}
				if (details == null && document == null)
				{
					return Json(new
					{
						documents = "Error",
						details = "Error"
					}, JsonRequestBehavior.AllowGet);
				}
				if (document == null && details != null)
				{
					return Json(new
					{
						documents = "Error",
						details = details
					}, JsonRequestBehavior.AllowGet);
				}
				if (details == null && document != null)
				{
					return Json(new
					{
						documents = arr,
						details = "Error"
					}, JsonRequestBehavior.AllowGet);
				}
				return Json(new
				{
					documents = arr,
					details = details
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				return Json(new
				{
					documents = "Error",
					details = "Error"
				}, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public async Task<JsonResult> GetDocumentDetails(int DocId)
		{
			try
			{
				GetCurrentUser();
				object details = new object();
				List<object> documents = new List<object>();
				DateTimeFormatInfo customFormatInfo = new DateTimeFormatInfo
				{
					DateSeparator = "/"
				};
				UploadedDocument document = await db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == DocId).FirstOrDefaultAsync();
				Modality modality = await db.Modalities.Where((Modality m) => (int?)m.Id == document.ModalityId).FirstOrDefaultAsync();
				if (document != null)
				{
					if (DateTime.TryParse(document.DOB, customFormatInfo, DateTimeStyles.None, out var sampleDate))
					{
						document.DOB = sampleDate.ToString("MM/dd/yyyy", customFormatInfo);
					}
					else
					{
						document.DOB = "";
					}
					if (DateTime.TryParse(document.DOS, customFormatInfo, DateTimeStyles.None, out var dosDate))
					{
						document.DOS = dosDate.ToString("MM/dd/yyyy", customFormatInfo);
					}
					else
					{
						document.DOS = "";
					}
					details = new
					{
						Modality = ((modality == null) ? "N/A" : modality.ModalityName),
						FaxNumber = (string.IsNullOrWhiteSpace(document.FaxNumber) ? "N/A" : document.FaxNumber),
						RefDoctor = (string.IsNullOrWhiteSpace(document.RefDoctor) ? "N/A" : document.RefDoctor),
						DOS = (string.IsNullOrWhiteSpace(document.DOS) ? "N/A" : document.DOS),
						DOB = (string.IsNullOrWhiteSpace(document.DOB) ? "N/A" : document.DOB),
						MRN = (string.IsNullOrWhiteSpace(document.MRN) ? "N/A" : document.MRN),
						PatientName = (string.IsNullOrWhiteSpace(document.PatientName) ? "N/A" : document.PatientName)
					};
				}
				if (document != null)
				{
					UploadedFile audio = db.UploadedFiles.Where((UploadedFile ad) => (int?)ad.FileID == document.FileID).FirstOrDefault();
					Status status = db.Status.Where((Status s) => (int?)s.Id == document.StatusId).FirstOrDefault();
					string statusColor;
					string statusName;
					if (status == null)
					{
						statusColor = db.Status.Where((Status s) => (int?)s.Id == audio.StatusId).First().Color;
						statusName = db.Status.Where((Status s) => (int?)s.Id == audio.StatusId).First().Name;
					}
					else
					{
						statusColor = db.Status.Where((Status s) => (int?)s.Id == document.StatusId).First().Color;
						statusName = db.Status.Where((Status s) => (int?)s.Id == document.StatusId).First().Name;
					}
					documents.Add(new
					{
						bgColor = statusColor,
						Status = statusName,
						DocId = document.Id,
						FileName = document.FileName.ToString(),
						Uploaded = $"{document.UploadOn:MM/dd/yyyy}",
						FilePath = document.FilePath,
						ext = document.Extension,
						DocChildID = document.DocChildID,
						isFax = db.Dictators.Where((Dictator d) => (int?)d.DictatorID == document.UploadedFile.DictatorID).FirstOrDefault().IsFax
					});
				}
				string userRole = (from x in UserList()
					where x.UserName == CurUserName
					select x).FirstOrDefault().RoleName;
				if (details == null && document == null)
				{
					return Json(new
					{
						documents = "Error",
						details = "Error",
						role = userRole
					}, JsonRequestBehavior.AllowGet);
				}
				if (document == null && details != null)
				{
					return Json(new
					{
						documents = "Error",
						details = details,
						role = userRole
					}, JsonRequestBehavior.AllowGet);
				}
				if (details == null && document != null)
				{
					return Json(new
					{
						documents = documents,
						details = "Error",
						role = userRole
					}, JsonRequestBehavior.AllowGet);
				}
				return Json(new
				{
					documents = documents,
					details = details,
					role = userRole
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				return Json(new
				{
					documents = "Error",
					details = "Error"
				}, JsonRequestBehavior.AllowGet);
			}
		}

		public async Task FaxAsync()
		{
			string User = base.Session["UserName"].ToString();
			string Pass = base.Session["Password"].ToString();
			FaxClient interfax = new FaxClient(User, Pass);
			SendOptions options = new SendOptions
			{
				FaxNumber = "+17182283603"
			};
			IFaxDocument fileDocument = interfax.Documents.BuildFaxDocument("D:\\Plesk.docx");
			await interfax.Outbound.SendFax(fileDocument, options);
			using (FileStream fileStream = System.IO.File.OpenRead("D:\\Plesk.docx"))
			{
				IFaxDocument fileDoc = interfax.Documents.BuildFaxDocument("D:\\Plesk.docx", fileStream);
				await interfax.Outbound.SendFax(fileDoc, options);
			}
		}

		[HttpGet]
		public ActionResult DownloadDocuments(int AudioId)
		{
			GetCurrentUser();
			try
			{
				using (ZipFile zipFile = new ZipFile())
				{
					List<UploadedDocument> list = new List<UploadedDocument>();
					if (AudioId <= 0)
					{
						list = db.UploadedDocuments.Where((UploadedDocument sa) => sa.FileID == (int?)AudioId).ToList();
					}
					zipFile.AlternateEncodingUsage = ZipOption.AsNecessary;
					string text = "";
					string text2 = base.Server.MapPath("~\\Content\\Transcripts\\");
					foreach (UploadedDocument item in list)
					{
						text = item.UploadedFile.Dictator.Clinic.Name;
						zipFile.AddFile(text2 + item.FileName + "." + item.Extension, text);
					}
					string fileDownloadName = string.Format("FilesZip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
					using (MemoryStream memoryStream = new MemoryStream())
					{
						zipFile.Save(memoryStream);
						return File(memoryStream.ToArray(), "application/zip", fileDownloadName);
					}
				}
			}
			catch (Exception)
			{
				return HttpNotFound("The file has been removed, renamed or permanantly moved to a new location.");
			}
		}

		public ActionResult About()
		{
			base.ViewBag.Message = "Your application description page.";
			return View();
		}

		public ActionResult Contact()
		{
			base.ViewBag.Message = "Your contact page.";
			return View();
		}

		public ActionResult Record()
		{
			GetCurrentUser();
			List<int> managerProviders = new List<int>();
			List<int> managerClinics = new List<int>();
			if (base.User.IsInRole(UserRoles.Provider))
			{
				base.ViewBag.ClinicID = new SelectList(db.Clinics.Where((Clinic sa) => sa.AddedBy == CurUserId), "ClinicID", "Name");
				base.ViewBag.GetTypist = from s in GetTypist()
					orderby s.Text
					select s;
				base.ViewBag.DictatorID = (from sa in db.Dictators
					where sa.LoginID == CurUserName && sa.IsActive != true
					select new
					{
						value = sa.DictatorID,
						Text = sa.LoginID
					} into s
					orderby s.Text
					select s).ToList();
			}
			else if (base.User.IsInRole(UserRoles.Typist))
			{
				base.ViewBag.ClinicID = new SelectList(db.Clinics.Where((Clinic sa) => sa.AddedBy == CurUserId), "ClinicID", "Name");
				base.ViewBag.GetTypist = from s in GetTypist()
					orderby s.Text
					select s;
				base.ViewBag.DictatorID = (from sa in db.Dictators
					where sa.AddedBy == CurUserId && sa.IsActive != true
					select new
					{
						value = sa.DictatorID,
						Text = sa.LoginID
					} into s
					orderby s.Text
					select s).ToList();
			}
			else if (!base.User.IsInRole(UserRoles.Manager))
			{
				base.ViewBag.ClinicID = new SelectList(db.Clinics, "ClinicID", "Name");
				base.ViewBag.GetTypist = from s in GetTypist()
					orderby s.Text
					select s;
				base.ViewBag.DictatorID = (from sa in db.Dictators
					where sa.IsActive != true
					select new
					{
						value = sa.DictatorID,
						Text = sa.LoginID
					} into s
					orderby s.Text
					select s).ToList();
			}
			else
			{
				int managerID = db.Managers.Where((Manager m) => m.Username == CurUserName).FirstOrDefault().ManagerID;
				managerClinics = (from m in db.ManagerClinics
					where m.ManagerID == managerID
					select m into x
					select x.ClinicID).ToList();
				managerProviders = (from m in db.ManagerClinics
					where m.ManagerID == managerID
					select m into x
					select x.DictatorID).ToList();
				base.ViewBag.DictatorID = (from sa in db.Dictators
					where managerProviders.Contains(sa.DictatorID)
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToList();
				base.ViewBag.GetTypist = (from x in db.stp_GetTypist()
					select new
					{
						value = x.UserId,
						text = x.UserName
					}).ToList();
				base.ViewBag.ClinicID = (from sa in db.Clinics
					where managerClinics.Contains(sa.ClinicID)
					select new
					{
						value = sa.ClinicID,
						text = sa.Name
					} into s
					orderby s.text
					select s).ToList();
			}
			return View();
		}

		public SelectList GetTypist()
		{
			var list = (from a in db.AspNetUsers
				join r in db.AspNetUserRoles on a.Id equals r.UserId
				where r.RoleId == "4"
				select new { a.UserId, a.UserName }).ToList();
			List<SelectListItem> list2 = new List<SelectListItem>();
			foreach (var item2 in list)
			{
				SelectListItem item = new SelectListItem
				{
					Value = item2.UserId.ToString(),
					Text = item2.UserName
				};
				list2.Add(item);
			}
			return new SelectList(list2, "Value", "Text");
		}

		[HttpPost]
		public string SendFax(dynamic FileID, string faxNumber, string Description, int FileID_, int UpDocID)
		{
			LogHelper.PrintGenericInfo("Send Fax CAlled = " + FileID + ", FaxNumber =" + faxNumber);
			try
			{
				string requestUriString = "https://api.cocofax.com/v1/fax/send";
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
				httpWebRequest.Method = "POST";
				httpWebRequest.Headers["x-api-key"] = "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0";
				httpWebRequest.ContentType = "application/json";
				faxNumber = (string.IsNullOrEmpty(faxNumber) ? "" : faxNumber.Replace("-", "").Trim());
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("{");
				stringBuilder.Append("\"files\": [");
				stringBuilder.Append("\"" + FileID + "\"],");
				stringBuilder.Append("\"fromPhone\": \"+13322349040\", ");
				stringBuilder.Append("\"texts\": [");
				stringBuilder.Append("\"" + Description + "\"");
				stringBuilder.Append("],");
				stringBuilder.Append("\"toPhones\": [");
				stringBuilder.Append("\"" + faxNumber + "\"");
				stringBuilder.Append("]");
				stringBuilder.Append("}");
				string text = stringBuilder.ToString();
				LogHelper.PrintGenericInfo("JSON SEND FAX=" + text);
				using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
				{
					streamWriter.Write(text);
				}
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
				{
					string text2 = streamReader.ReadToEnd();
					LogHelper.PrintError("Sendfax Api Result=" + text2);
					JObject jObject = JObject.Parse(text2.ToString());
					if (jObject["data"]["faxes"] != null)
					{
						string value = jObject["data"]["faxes"].ToString();
						dynamic val = JsonConvert.DeserializeObject<List<WordDocumentController.StatusJson>>(value);
						string faxID = val[0].id;
						string text3 = val[0].status;
						try
						{
							FileFaxStatu fileFaxStatu = db.FileFaxStatus.Where((FileFaxStatu _a) => _a.FileID == FileID_ && _a.IsActive == true).FirstOrDefault();
							if (fileFaxStatu != null)
							{
								fileFaxStatu.IsActive = false;
							}
							FileFaxStatu fileFaxStatu2 = new FileFaxStatu();
							fileFaxStatu2.AddedOn = DateTime.UtcNow;
							fileFaxStatu2.DateCreated = val[0].dateCreated;
							fileFaxStatu2.FaxID = faxID;
							fileFaxStatu2.FileID = FileID_;
							fileFaxStatu2.PageCount = val[0].pageCount;
							fileFaxStatu2.ToNumber = val[0].to;
							fileFaxStatu2.UpDocID = UpDocID;
							fileFaxStatu2.IsActive = true;
							fileFaxStatu2.Status = text3;
							db.FileFaxStatus.Add(fileFaxStatu2);
							db.SaveChanges();
						}
						catch (Exception ex)
						{
							LogHelper.PrintError("save status error " + ex);
						}
						return text3;
					}
				}
				return null;
			}
			catch (Exception ex2)
			{
				LogHelper.PrintError("Send fax Error =" + ex2);
			}
			return null;
		}

		private List<UserWithRole> UserList()
		{
			return (from user in db.AspNetUsers
				from roles in user.AspNetUserRoles
				join role in db.AspNetRoles on roles.RoleId equals role.Id
				select new UserWithRole
				{
					UserName = user.UserName,
					RoleName = role.Name
				}).ToList();
		}

		public ActionResult DownloadFile(int DocId)
		{
			try
			{
				UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == DocId).FirstOrDefault();
				if (uploadedDocument != null)
				{
					string text = uploadedDocument.FileName + "." + uploadedDocument.Extension;
					string text2 = text.Split('.')[0];
					string text3 = Path.GetExtension(text.ToLower()).Substring(1);
					string text4 = "";
					if (text3 == "docx" || text3 == "doc")
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(base.Server.MapPath("~\\Content\\TempDownload"));
						if (directoryInfo != null)
						{
							string text5 = base.Server.MapPath("~/Content/TempDownload/") + text;
							FileInfo fileInfo = new FileInfo(text5.ToString());
							text4 = ((!fileInfo.Exists) ? uploadedDocument.FilePath : ("/Content/TempDownload/" + text));
						}
						else
						{
							text4 = uploadedDocument.FilePath + text;
						}
						UserLog userLog = new UserLog();
						userLog.UserLogId = CurUserId;
						userLog.UserLogName = CurUserName;
						userLog.AudioID = uploadedDocument.FileID;
						userLog.FileName = uploadedDocument.FileName.Split('.')[0];
						userLog.FileType = uploadedDocument.Extension;
						userLog.Action = 0;
						userLog.Date = CustomTimeZone.Get_US_UTC_Time();
						userLog.StatusId = uploadedDocument.StatusId;
						userLog.DocID = uploadedDocument.Id;
						userLog.ProviderId = uploadedDocument.UploadedFile.DictatorID;
						UserLog entity = userLog;
						db.UserLogs.Add(entity);
						db.SaveChanges();
						return Json(new
						{
							success = true,
							Link = text4,
							AllowGet = JsonRequestBehavior.AllowGet
						});
					}
					return Json(new
					{
						success = false,
						AllowGet = JsonRequestBehavior.AllowGet
					});
				}
				return Json(new
				{
					success = false,
					AllowGet = JsonRequestBehavior.AllowGet
				});
			}
			catch (Exception innerException)
			{
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				LogHelper.PrintError("File Error Dated: " + DateTime.Now.ToString() + "->ExMessage->" + innerException.Message);
				return Json(new
				{
					found = false,
					AllowGet = JsonRequestBehavior.AllowGet
				});
			}
		}
	}
}
