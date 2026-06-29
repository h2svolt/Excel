using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI;
using ExcelTranscript.Models;
using ExcelTranscript.Models.BLL;
using ExcelTranscript.Models.Helper;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spire.Doc;
using Spire.Doc.Documents;

namespace ExcelTranscript.Controllers
{
	public class WordDocumentController : Controller
	{
		public class StatusJson
		{
			public string id { get; set; }

			public string from { get; set; }

			public string to { get; set; }

			public string status { get; set; }

			public string dateCreated { get; set; }

			public string pageCount { get; set; }
		}

		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public int CurUserId { get; set; }

		public string CurUserName { get; set; }

		private HttpClient Client { get; set; }

		private int GetCurrentUserId()
		{
			IIdentity identity = base.User.Identity;
			string currentUserId = base.User.Identity.GetUserId();
			AspNetUser aspNetUser = db.AspNetUsers.FirstOrDefault((AspNetUser x) => x.Id == currentUserId);
			CurUserId = aspNetUser.UserId;
			return aspNetUser.UserId;
		}

		private string GetCurrentUserName()
		{
			IIdentity identity = base.User.Identity;
			string currentUserId = base.User.Identity.GetUserId();
			AspNetUser aspNetUser = db.AspNetUsers.FirstOrDefault((AspNetUser x) => x.Id == currentUserId);
			CurUserName = aspNetUser.UserName;
			return aspNetUser.UserName;
		}

		public ActionResult Index(string FilePath, int? DocID)
		{
			if (FilePath == null)
			{
				return View();
			}
			try
			{
				base.Session["CurrentFileName"] = null;
				base.Session["DocID"] = null;
				base.Session["DocID"] = DocID;
				base.Session["CurrentFileName"] = FilePath;
				string fileName = Path.GetFileName(FilePath);
				string text = fileName.Split('.')[0];
				string text2 = Path.GetExtension(FilePath.ToLower()).Substring(1);
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
							text5 = FilePath.Split('.')[0];
							obj3 = base.Server.MapPath("~/Content/TempDownload/") + text5 + ".htm";
							text4 = base.Server.MapPath("~/Content/TempDownload/") + text5 + "_files";
							array = directoryInfo.GetFiles("*" + FilePath + "*.*");
						}
						else
						{
							text5 = FilePath.Split('.')[0];
							obj3 = base.Server.MapPath("~/Content/Temp/") + text5 + ".htm";
							text4 = base.Server.MapPath("~/Content/Temp/") + text5 + "_files";
							if (!Directory.Exists(base.Server.MapPath("~/Content/Temp/")))
							{
								Directory.CreateDirectory(base.Server.MapPath("~/Content/Temp/"));
							}
							DirectoryInfo directoryInfo2 = new DirectoryInfo(base.Server.MapPath("~\\Content\\Transcripts"));
							array = directoryInfo2.GetFiles("*" + FilePath + "*.*");
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
						array = directoryInfo3.GetFiles("*" + FilePath + "*.*");
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
					}
					document.Styles.ApplyDocDefaultsToNormalStyle();
					text = FilePath.Split('.')[0];
					string text6 = FilePath.Split('.')[1];
					string text7 = obj2.ToString().Split('.')[0];
					string text8 = "C:\\inetpub\\vhosts\\excelonwork.com\\staging.excelonwork.com\\Content\\Temp\\";
					string text9 = base.Server.MapPath("~\\Content\\Temp\\");
					string text10 = base.Server.MapPath("~/Content/Temp/");
					string text11 = base.Server.MapPath("~/Content/Temp/" + text + "." + text6);
					SendEmail("spt1: " + text9 + " |||| spt2: " + text10 + " |||| spt3: " + text11);
					string text12 = text8 + text + ".htm";
					string text13 = text8 + text + ".docx";
					HtmlDocument htmlDocument = new HtmlDocument();
					if (ConfigurationManager.AppSettings["appEnv"].ToString() == "Dev")
					{
						document.SaveToFile(obj2.ToString(), FileFormat.Docx2013);
						document.SaveToFile(text7 + ".htm", FileFormat.Html);
						htmlDocument.Load(text7 + ".htm");
					}
					else
					{
						document.SaveToFile(text13, FileFormat.Docx2013);
						document.SaveToFile(text12, FileFormat.Html);
						htmlDocument.Load(text12);
					}
					string text14 = "";
					if (ConfigurationManager.AppSettings["appEnv"].ToString() == "Dev")
					{
						text14 = obj2.ToString().Remove(0, obj2.ToString().IndexOf("Content"));
					}
					else
					{
						int num = text13.ToString().IndexOf("Content");
						text14 = text13.ToString().Remove(0, text13.ToString().IndexOf("Content"));
					}
					string imgPath = "/" + text14.Remove(text14.LastIndexOf("\\")).Replace("\\", "/");
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
					base.ViewBag.HTML = stringWriter.ToString();
					return View();
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
			return View();
		}

		public bool SendEmail(string messages = "")
		{
			MailAddress mailAddress = new MailAddress("firstmedicines@gmail.com", "Excel");
			MailAddress to = new MailAddress("airas.mangotech@gmail.com");
			try
			{
				SmtpClient smtpClient = new SmtpClient
				{
					Host = "smtp.gmail.com",
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(mailAddress.Address, "dparhakdzitxbyuy")
				};
				using (MailMessage message = new MailMessage(mailAddress, to)
				{
					IsBodyHtml = true,
					Subject = "Excel",
					Body = messages
				})
				{
					smtpClient.Send(message);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		[HttpPost]
		public ActionResult SaveDocumentss(string FileText, int value)
		{
			bool flag = false;
			try
			{
				SendEmail("In Save Document");
				GetCurrentUserId();
				GetCurrentUserName();
				string html = FileText;
				HtmlDocument htmlDocument = new HtmlDocument();
				FileText = "<html xmlns:v='urn:schemas-microsoft-com:vml'\r\n                        xmlns: o = 'urn:schemas-microsoft-com:office:office'\r\n                        xmlns: w = 'urn:schemas-microsoft-com:office:word'\r\n                        xmlns: m = 'http://schemas.microsoft.com/office/2007/12/omml'\r\n                        xmlns = 'http://www.w3.org/TR/REC-html40' >\r\n                        <head> " + FileText + "</body> </html>";
				htmlDocument.LoadHtml(html);
				if (!Directory.Exists(base.Server.MapPath("~/Content/TempDownload/")))
				{
					Directory.CreateDirectory(base.Server.MapPath("~/Content/TempDownload/"));
				}
				SendEmail("Download Path 419");
				DirectoryInfo directoryInfo = new DirectoryInfo(base.Server.MapPath("~\\Content\\TempDownload\\"));
				string text = base.Server.MapPath("~/Content/TempDownload/");
				string text2 = base.Session["CurrentFileName"].ToString();
				string fileName = string.Concat(directoryInfo, text2);
				string text3 = text2.Split('.')[0];
				string text4 = text2.Split('.')[1];
				FileInfo fileInfo = new FileInfo(fileName);
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
				}
				SendEmail("File path 437");
				DirectoryInfo directoryInfo2 = new DirectoryInfo(base.Server.MapPath("~\\Content\\Transcripts\\"));
				fileName = string.Concat(directoryInfo, text3, ".", text4);
				string text5 = fileName.ToString().Remove(0, fileName.ToString().IndexOf("Content"));
				string text6 = "/" + text5.Remove(text5.LastIndexOf("\\")).Replace("\\", "/");
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
					string value2 = link.Attributes["href"].Value;
					link.SetAttributeValue("href", value2);
				});
				list.ForEach(delegate(HtmlNode img)
				{
					string value3 = img.Attributes["src"].Value;
					img.SetAttributeValue("src", value3);
				});
				Document document = new Document();
				Section section = document.AddSection();
				string fileName2 = string.Concat(directoryInfo2, text3, ".", text4);
				Document document2 = new Document();
				document2.LoadFromFile(fileName2);
				HeaderFooter header = document2.Sections[0].HeadersFooters.Header;
				HeaderFooter footer = document2.Sections[0].HeadersFooters.Footer;
				foreach (Section section2 in document.Sections)
				{
					foreach (DocumentObject childObject in header.ChildObjects)
					{
						section2.HeadersFooters.Header.ChildObjects.Add(childObject.Clone());
					}
					foreach (DocumentObject childObject2 in footer.ChildObjects)
					{
						section2.HeadersFooters.Footer.ChildObjects.Add(childObject2.Clone());
					}
				}
				document2.Close();
				document2.Dispose();
				Paragraph paragraph = section.AddParagraph();
				string[] array = htmlDocument.Text.Split(new string[1] { "<p class=\"Footer\"" }, StringSplitOptions.None);
				paragraph.AppendHTML(array[0]);
				SendEmail(text + text2);
				document.SaveToFile(text + text2, FileFormat.Docx);
				document.Close();
				int DocID = Convert.ToInt32(base.Session["DocID"]);
				UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == DocID).FirstOrDefault();
				if (uploadedDocument != null)
				{
					if (value != 1)
					{
						FaxBLL faxBLL = new FaxBLL();
						string text7 = faxBLL.UploadFile(string.Concat(directoryInfo2, text3, ".", text4));
						string text8 = "";
						if (text7 != null)
						{
							text8 = SendFax(text7, uploadedDocument.FaxNumber, uploadedDocument.MRN, uploadedDocument.FileID ?? 0, uploadedDocument.Id);
						}
						if (text8 != null && text8 != "")
						{
							LogHelper.PrintGenericInfo("Fax Sent status=" + text8);
							uploadedDocument.Decription = text8;
						}
						uploadedDocument.IsApproved = true;
						uploadedDocument.StatusId = 5;
						UserLog entity = new UserLog
						{
							UserLogId = CurUserId,
							UserLogName = CurUserName,
							AudioID = uploadedDocument.FileID,
							FileName = uploadedDocument.FileName,
							FileType = uploadedDocument.Extension,
							Action = 5,
							Date = CustomTimeZone.Get_US_UTC_Time(),
							StatusId = 5,
							ProviderId = uploadedDocument.UploadedFile.DictatorID
						};
						db.UserLogs.Add(entity);
						db.Entry(uploadedDocument).State = EntityState.Modified;
						UserLog entity2 = new UserLog
						{
							UserLogId = CurUserId,
							UserLogName = CurUserName,
							AudioID = uploadedDocument.FileID,
							FileName = uploadedDocument.FileName,
							FileType = uploadedDocument.Extension,
							DocID = uploadedDocument.Id,
							Action = 7,
							Date = CustomTimeZone.Get_US_UTC_Time(),
							StatusId = 5,
							ProviderId = uploadedDocument.UploadedFile.DictatorID
						};
						db.UserLogs.Add(entity2);
						db.SaveChanges();
					}
					flag = true;
				}
				return Json(flag, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogHelper.PrintError("Save Document Errorrrrrrr=  " + ex.Message + "File Text =>" + FileText);
				return Json("Error occured during document save.", JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public ActionResult SaveDocument(string FileText, int value)
		{
			bool flag = false;
			try
			{
				GetCurrentUserId();
				GetCurrentUserName();
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
				string text2 = base.Session["CurrentFileName"].ToString();
				string fileName = string.Concat(directoryInfo, text2);
				string FileName = text2.Split('.')[0];
				string text3 = text2.Split('.')[1];
				FileInfo fileInfo = new FileInfo(fileName);
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
				}
				DirectoryInfo directoryInfo2 = new DirectoryInfo(base.Server.MapPath("~\\Content\\Transcripts\\"));
				fileName = string.Concat(directoryInfo, FileName, ".", text3);
				string text4 = fileName.ToString().Remove(0, fileName.ToString().IndexOf("Content"));
				string text5 = "/" + text4.Remove(text4.LastIndexOf("\\")).Replace("\\", "/");
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
					string value2 = link.Attributes["href"].Value;
					link.SetAttributeValue("href", value2);
				});
				list.ForEach(delegate(HtmlNode img)
				{
					string value3 = img.Attributes["src"].Value;
					img.SetAttributeValue("src", value3);
				});
				Document document = new Document();
				Section section = document.AddSection();
				string fileName2 = string.Concat(directoryInfo2, FileName, ".", text3);
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
				HtmlDocument htmlDocument2 = new HtmlDocument();
				try
				{
					document3.SaveToFile(text + "testttt_footer.htm", FileFormat.Html);
					htmlDocument2.Load(text + "testttt_footer.htm");
				}
				catch (Exception ex)
				{
					SendEmail(ex.StackTrace);
				}
				HtmlDocument htmlDocument3 = new HtmlDocument();
				htmlDocument3 = htmlDocument2;
				HtmlNode htmlNode = htmlDocument3.DocumentNode.SelectSingleNode("//div[contains(@class,'Section0')]");
				foreach (HtmlNode item in (IEnumerable<HtmlNode>)htmlNode.ChildNodes)
				{
					if (htmlDocument.Text.Contains(item.OuterHtml))
					{
						htmlDocument.Text = htmlDocument.Text.Replace(item.OuterHtml, "");
						htmlDocument.Text = htmlDocument.Text.Replace(item.InnerHtml, "");
						htmlDocument.Text = htmlDocument.Text.Replace(item.InnerText, "");
					}
				}
				htmlDocument.Text = htmlDocument.Text.Replace(htmlNode.InnerHtml, "");
				try
				{
					HtmlNode htmlNode2 = htmlDocument.DocumentNode.SelectNodes("//p[contains(@class,'Normal')]").Last();
					if (htmlNode.InnerText.StartsWith(htmlNode2.InnerText.Substring(0, 5)))
					{
						htmlDocument.Text = htmlDocument.Text.Replace(htmlNode2.OuterHtml, "");
					}
				}
				catch (Exception)
				{
				}
				string text6 = htmlDocument.Text.Replace(htmlNode.InnerHtml, "");
				Paragraph paragraph = section.AddParagraph();
				HtmlNode htmlNode3 = htmlDocument.DocumentNode.SelectNodes("//p").Last();
				HtmlNode htmlNode4 = htmlDocument.DocumentNode.SelectSingleNode("//p[contains(@class,'Header')]");
				int lineNum = htmlNode4.Line;
				string text7 = text6.Replace(htmlNode4.OuterHtml, "");
				IEnumerable<HtmlNode> enumerable = from x in htmlDocument.DocumentNode.SelectNodes("//p")
					where x.Line < lineNum
					select x;
				foreach (HtmlNode item2 in enumerable)
				{
					HtmlNode htmlNode5 = item2;
					text7 = text7.Replace(item2.OuterHtml, "");
				}
				if (value != 1)
				{
					string text8 = "";
					UploadedDocument upldDoc = db.UploadedDocuments.Where((UploadedDocument x) => x.FileName == FileName).FirstOrDefault();
					Dictator dictator = db.Dictators.Where((Dictator x) => (int?)x.DictatorID == upldDoc.UploadedFile.DictatorID).FirstOrDefault();
					if (dictator.AddSign != true)
					{
						string[] array = text7.Split(new string[1] { "<p class=\"Footer\"" }, StringSplitOptions.None);
						text7 = array[0];
					}
					else
					{
						Dictator dictator2 = db.Dictators.Where((Dictator x) => (int?)x.DictatorID == upldDoc.UploadedFile.DictatorID).FirstOrDefault();
						text8 = ((dictator2.MiddleName == null) ? (dictator2.Prefix + ". " + dictator2.FirstName + " " + dictator2.LastName) : (dictator2.Prefix + ". " + dictator2.FirstName + " " + dictator2.MiddleName + " " + dictator2.LastName));
						string text9 = CustomTimeZone.Get_US_UTC_Time().ToString("MM/dd/yyyy hh:mm:ss tt");
						string text10 = "<p class=\"Normal\" style=\"text-align:left;\"><span style=\"font-size:11pt;font-family:'Times New Roman';mso-fareast-font-family:'Times New Roman';mso-bidi-font-family:'Times New Roman';lang:EN-US;mso-fareast-language:EN-US;mso-ansi-language:AR-SA;\">Electronically Signed By " + text8 + " on " + text9 + " EST.</span></p>";
						string[] array = text7.Split(new string[1] { "<p class=\"Footer\"" }, StringSplitOptions.None);
						text7 = array[0] + text10;
					}
				}
				else
				{
					string[] array = text7.Split(new string[1] { "<p class=\"Footer\"" }, StringSplitOptions.None);
					text7 = array[0];
				}
				paragraph.AppendHTML(text7);
				ParagraphStyle paragraphStyle = new ParagraphStyle(document);
				Style style = document.Styles.FindByName("ParaFontStyle");
				if (style == null)
				{
					paragraphStyle.Name = "ParaFontStyle";
					paragraphStyle.CharacterFormat.FontName = "Times New Roman";
					paragraphStyle.CharacterFormat.FontSize = 11f;
					document.Styles.Add(paragraphStyle);
				}
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
					foreach (Paragraph paragraph4 in document.Sections[0].Paragraphs)
					{
						if (section6.Body.ChildObjects[num].DocumentObjectType == DocumentObjectType.Paragraph)
						{
							Paragraph paragraph3 = section6.Body.ChildObjects[num] as Paragraph;
							string text11 = paragraph3.Text;
							paragraph3.Format.BeforeAutoSpacing = false;
							paragraph3.Format.AfterAutoSpacing = false;
							if (paragraph3.CharCount <= 0)
							{
								paragraph3.Text = "";
							}
							if (Regex.IsMatch(paragraph3.Text, "((\\d).[\\s]{2,})"))
							{
								string text12 = Regex.Replace(paragraph3.Text, "((\\d).[\\s]{2,})", "");
								paragraph3.Text = text12;
								paragraph3.ListFormat.ApplyNumberedStyle();
								paragraph3.ApplyStyle("ParaFontStyle");
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
				document.SaveToFile(text + text2, FileFormat.Docx2013);
				document.Close();
				int DocID = Convert.ToInt32(base.Session["DocID"]);
				UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == DocID).FirstOrDefault();
				if (uploadedDocument != null)
				{
					if (value != 1)
					{
						FaxBLL faxBLL = new FaxBLL();
						string text13 = faxBLL.UploadFile(string.Concat(directoryInfo2, FileName, ".", text3));
						string text14 = "";
						if (text13 != null)
						{
							text14 = SendFax(text13, uploadedDocument.FaxNumber, uploadedDocument.MRN, uploadedDocument.FileID ?? 0, uploadedDocument.Id);
						}
						if (text14 != null && text14 != "")
						{
							LogHelper.PrintGenericInfo("Fax Sent status=" + text14);
							uploadedDocument.Decription = text14;
						}
						uploadedDocument.IsApproved = true;
						uploadedDocument.StatusId = 5;
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
								DocID = uploadedDocument.Id,
								Action = 5,
								Date = CustomTimeZone.Get_US_UTC_Time(),
								StatusId = 5,
								ProviderId = uploadedDocument.UploadedFile.DictatorID
							},
							new UserLog
							{
								UserLogId = CurUserId,
								UserLogName = CurUserName,
								AudioID = uploadedDocument.FileID,
								FileName = uploadedDocument.FileName,
								FileType = uploadedDocument.Extension,
								DocID = uploadedDocument.Id,
								Action = 7,
								Date = CustomTimeZone.Get_US_UTC_Time(),
								StatusId = 5,
								ProviderId = uploadedDocument.UploadedFile.DictatorID
							}
						};
						db.UserLogs.AddRange(entities);
						db.SaveChanges();
					}
					flag = true;
				}
				return Json(flag, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex3)
			{
				LogHelper.PrintError("Save Document Errorrrrrrr=  " + ex3.StackTrace + "File Text =>" + FileText);
				return Json("Error occured during document save.", JsonRequestBehavior.AllowGet);
			}
		}

		private IEnumerable<string> GetNextChars(string str, int iterateCount)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < str.Length; i += iterateCount)
			{
				if (str.Length - i >= iterateCount)
				{
					list.Add(str.Substring(i, iterateCount));
				}
				else
				{
					list.Add(str.Substring(i, str.Length - i));
				}
			}
			return list;
		}

		public Tuple<int, int, int> GetWordCount(string FileName, int? NumOfLine)
		{
			string text = "C:\\inetpub\\vhosts\\excelonwork.com\\httpdocs\\Content\\Transcripts\\";
			string fileName = text + FileName;
			Document document = new Document();
			document.LoadFromFile(fileName);
			string text2 = text + FileName.Split('.')[0];
			string path = text2 + ".txt";
			string text3 = document.GetText();
			System.IO.File.WriteAllText(path, text3.ToString());
			string text4 = System.IO.File.ReadAllText(path);
			int item = System.IO.File.ReadLines(path).Count();
			string text5 = text4;
			text5 = text5.Trim();
			Regex regex = new Regex("[^a-zA-Z0-9% ._]");
			text5 = regex.Replace(text5, " ");
			string[] source = text5.Split(null);
			int num = text5.Count((char c) => !char.IsWhiteSpace(c));
			if (NumOfLine.HasValue && NumOfLine > 0)
			{
				item = num / Convert.ToInt16(NumOfLine);
			}
			int item2 = source.Count();
			return new Tuple<int, int, int>(item, num, item2);
		}

		public ActionResult GetFaxStatusList()
		{
			return null;
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
				faxNumber = (string.IsNullOrWhiteSpace(faxNumber) ? "" : faxNumber.Replace("-", "").Trim());
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
						dynamic val = JsonConvert.DeserializeObject<List<StatusJson>>(value);
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
							fileFaxStatu2.AddedOn = CustomTimeZone.Get_US_UTC_Time();
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

		[HttpGet]
		public JsonResult FaxStatus(string FaxId)
		{
			try
			{
				FaxId = "6204c54d9d820436b80927f3";
				string requestUriString = "https://api.cocofax.com/v1/fax/?id=" + FaxId;
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
				httpWebRequest.Headers["x-api-key"] = "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0";
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
					{
						string value = streamReader.ReadToEnd();
						Console.WriteLine(value);
					}
				}
				HttpWebResponse httpWebResponse2 = (HttpWebResponse)httpWebRequest.GetResponse();
				using (StreamReader streamReader2 = new StreamReader(httpWebResponse2.GetResponseStream()))
				{
					string text = streamReader2.ReadToEnd();
				}
				return Json(httpWebResponse2.StatusCode, JsonRequestBehavior.AllowGet);
			}
			catch (Exception innerException)
			{
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
			}
			return Json(0);
		}

		public ActionResult UploadFax(string FileName)
		{
			try
			{
				GetCurrentUserId();
				GetCurrentUserName();
				string text = "/Content/Transcripts/" + FileName;
				string text2 = base.Server.MapPath("~\\" + text);
				string fileName = text2;
				string fName = FileName.ToString().Split('.')[0];
				string fExt = FileName.ToString().Split('.')[1];
				UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.FileName == fName).FirstOrDefault();
				if (uploadedDocument != null)
				{
					FaxBLL faxBLL = new FaxBLL();
					string text3 = faxBLL.UploadFile(fileName);
					string text4 = "";
					if (text3 != null)
					{
						text4 = SendFax(text3, uploadedDocument.FaxNumber, uploadedDocument.Decription, uploadedDocument.FileID ?? 0, uploadedDocument.Id);
					}
					if (text4 != null && text4 != "")
					{
						UploadedDocument uploadedDocument2 = db.UploadedDocuments.Where((UploadedDocument sa) => sa.FileName == fName && sa.Extension == fExt).FirstOrDefault();
						uploadedDocument2.Decription = text4;
						db.Entry(uploadedDocument2).State = EntityState.Modified;
						db.SaveChanges();
					}
					UserLog entity = new UserLog
					{
						UserLogId = CurUserId,
						UserLogName = CurUserName,
						AudioID = uploadedDocument.FileID,
						FileName = uploadedDocument.FileName,
						FileType = uploadedDocument.Extension,
						DocID = uploadedDocument.Id,
						Action = 7,
						Date = CustomTimeZone.Get_US_UTC_Time(),
						ProviderId = uploadedDocument.UploadedFile.DictatorID
					};
					db.UserLogs.Add(entity);
					db.SaveChanges();
					return RedirectToAction("Index", "Home");
				}
				return RedirectToAction("Index", "Home");
			}
			catch (Exception error)
			{
				LogHelper.PrintError(error);
				return RedirectToAction("Index", "Home");
			}
		}

		public JsonResult UploadAsync(string fileName)
		{
			try
			{
				FaxStatus("");
				string text = "/Content/Transcripts/" + fileName;
				string path = base.Server.MapPath("~\\" + text);
				string requestUriString = "https://api.cocofax.com/v1/file/upload";
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
				httpWebRequest.Method = "POST";
				httpWebRequest.Headers["x-api-key"] = "BC64485A1FF959ADDE2BA9C6FED8540FC58BB71379EC18311FADB6FEF4A61E0";
				httpWebRequest.ContentType = "application/json";
				byte[] bytes = System.IO.File.ReadAllBytes(path);
				BitArray bitArray = new BitArray(bytes);
				string text2 = string.Join("", bitArray);
				string value = "{\r\n                                \"files\": " + text2 + "}";
				using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
				{
					streamWriter.Write(value);
				}
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
				{
					string text3 = streamReader.ReadToEnd();
				}
				return Json(httpWebResponse.StatusCode, JsonRequestBehavior.AllowGet);
			}
			catch (Exception innerException)
			{
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
			}
			return Json(0);
		}
	}
}
