using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExcelTranscript.Models;
using ExcelTranscript.Models.DTO;
using ExcelTranscript.Models.Helper;
using Ionic.Zip;
using Microsoft.AspNet.Identity;
using Spire.Doc;
using TagLib;

namespace ExcelTranscript.Controllers
{
	public class WordDocUploadController : Controller
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public int CurUserId { get; set; }

		public string CurUserName { get; set; }

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

		public ActionResult Index(bool? btn, int? DictatorID, int? ClinicID, int? TypistID, int? StatusId, DateTime? fromDate, DateTime? toDate)
		{
			try
			{
				GetCurrentUserId();
				GetCurrentUserName();
				base.ViewBag.Modality = (from sa in db.Modalities
					select new
					{
						value = sa.Id,
						text = sa.ModalityName
					} into s
					orderby s.text
					select s).ToList();
				if (btn.HasValue)
				{
					List<stp_GetDocuments_Result> model = (base.User.IsInRole("Provider") ? (from sa in db.stp_GetDocuments(DictatorID, ClinicID, TypistID, StatusId, fromDate, toDate)
						where sa.UploadBy == CurUserId
						select sa).ToList() : ((!base.User.IsInRole("Typist")) ? db.stp_GetDocuments(DictatorID, ClinicID, TypistID, StatusId, fromDate, toDate).ToList() : (from sa in db.stp_GetDocuments(DictatorID, ClinicID, TypistID, StatusId, fromDate, toDate)
						where sa.TypistAssignID == CurUserId
						select sa).ToList()));
					return PartialView("_Index", model);
				}
				base.ViewBag.StatusId = from s in db.Status.Select((Status sa) => new
					{
						value = sa.Id,
						text = sa.Name
					}).ToList()
					orderby s.value == 4, s.value
					select s;
				if (base.User.IsInRole("Provider"))
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
					base.ViewBag.ClinicID = (from sa in db.Clinics
						where sa.ClinicID == Dictator
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
				else if (!base.User.IsInRole("Typist"))
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
				return View();
			}
			catch (Exception ex)
			{
				HelperEmail.SendEmail("WordDoc Index: " + ex.StackTrace + " \n\n\n|||||||||Inner exception: " + ex.InnerException);
				throw;
			}
		}

		public ActionResult Upload()
		{
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> BulkRemoveDocuments(string DocIds)
		{
			try
			{
				List<string> IdsList = DocIds.Split(',').ToList();
				new List<string>();
				int failedDocsCount = 0;
				foreach (string Id in IdsList)
				{
					int DocId = Convert.ToInt32(Id);
					UploadedDocument document = await db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == DocId).FirstOrDefaultAsync();
					if (document != null)
					{
						int UpldDocStatus = document.StatusId.GetValueOrDefault();
						if (UpldDocStatus == 5)
						{
							failedDocsCount++;
							continue;
						}
						List<UserLog> logs = db.UserLogs.Where((UserLog l) => l.DocID == (int?)DocId).ToList();
						if (logs.Count >= 1)
						{
							db.UserLogs.RemoveRange(logs);
							await db.SaveChangesAsync();
						}
						db.UploadedDocuments.Remove(document);
						await db.SaveChangesAsync();
						string docPath = base.Server.MapPath("~\\Content\\Transcripts\\");
						FileInfo fileinfo = new FileInfo(docPath + document.FileName + "." + document.Extension);
						if (fileinfo.Exists)
						{
							fileinfo.Delete();
						}
						List<string> paths = new List<string>
						{
							base.Server.MapPath("~/Content/TempDownload/"),
							base.Server.MapPath("~/Content/Temp/")
						};
						foreach (string path in paths)
						{
							string filePath = path + document.FileName + "." + document.Extension;
							FileInfo tmpDirfileExists = new FileInfo(filePath.ToString());
							if (tmpDirfileExists.Exists)
							{
								tmpDirfileExists.Delete();
							}
							string imagesPath = path + document.FileName + "_images";
							if (Directory.Exists(imagesPath))
							{
								Directory.Delete(imagesPath, recursive: true);
							}
							string stylePath = path + document.FileName + "_styles.css";
							FileInfo styleFile = new FileInfo(stylePath.ToString());
							if (styleFile.Exists)
							{
								styleFile.Delete();
							}
							string htmPath = path + document.FileName + ".htm";
							FileInfo htmFile = new FileInfo(htmPath.ToString());
							if (htmFile.Exists)
							{
								htmFile.Delete();
							}
						}
					}
					else
					{
						failedDocsCount++;
					}
				}
				return Json(new
				{
					returnObj = "Document(s) deleted successfully."
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				return Json("Error deleting documents!", JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public JsonResult RemoveDocument(int DocId)
		{
			try
			{
				UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == DocId).FirstOrDefault();
				if (uploadedDocument != null)
				{
					int valueOrDefault = uploadedDocument.StatusId.GetValueOrDefault();
					if (valueOrDefault == 5)
					{
						return Json("Can't Delete 'Approved' document!", JsonRequestBehavior.AllowGet);
					}
					List<UserLog> list = db.UserLogs.Where((UserLog l) => l.DocID == (int?)DocId).ToList();
					if (list.Count >= 1)
					{
						db.UserLogs.RemoveRange(list);
						db.SaveChanges();
					}
					db.UploadedDocuments.Remove(uploadedDocument);
					db.SaveChanges();
					string text = base.Server.MapPath("~\\Content\\Transcripts\\");
					FileInfo fileInfo = new FileInfo(text + uploadedDocument.FileName + "." + uploadedDocument.Extension);
					if (fileInfo.Exists)
					{
						fileInfo.Delete();
					}
					List<string> list2 = new List<string>
					{
						base.Server.MapPath("~/Content/TempDownload/"),
						base.Server.MapPath("~/Content/Temp/")
					};
					foreach (string item in list2)
					{
						string text2 = item + uploadedDocument.FileName + "." + uploadedDocument.Extension;
						FileInfo fileInfo2 = new FileInfo(text2.ToString());
						if (fileInfo2.Exists)
						{
							fileInfo2.Delete();
						}
						string path = item + uploadedDocument.FileName + "_images";
						if (Directory.Exists(path))
						{
							Directory.Delete(path, recursive: true);
						}
						string text3 = item + uploadedDocument.FileName + "_styles.css";
						FileInfo fileInfo3 = new FileInfo(text3.ToString());
						if (fileInfo3.Exists)
						{
							fileInfo3.Delete();
						}
						string text4 = item + uploadedDocument.FileName + ".htm";
						FileInfo fileInfo4 = new FileInfo(text4.ToString());
						if (fileInfo4.Exists)
						{
							fileInfo4.Delete();
						}
					}
					return Json(true, JsonRequestBehavior.AllowGet);
				}
				return Json("Document's record not found!", JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				return Json("Error deleting document!", JsonRequestBehavior.AllowGet);
			}
		}

		public string ReverseString(string myStr)
		{
			char[] array = myStr.ToCharArray();
			Array.Reverse(array);
			return new string(array);
		}

		[HttpPost]
		public JsonResult UploadDocuments()
		{
			GetCurrentUserId();
			GetCurrentUserName();
			try
			{
				List<object> list = new List<object>();
				List<object> list2 = new List<object>();
				List<object> list3 = new List<object>();
				string FileName = "";
				HttpFileCollectionBase files = base.Request.Files;
				if (files.Count > 0)
				{
					for (int i = 0; i < files.Count; i++)
					{
						HttpPostedFileBase httpPostedFileBase = files[i];
						try
						{
							string ext = Path.GetExtension(httpPostedFileBase.FileName.ToLower()).Substring(1);
							FileName = Path.GetFileName(httpPostedFileBase.FileName);
							if (!(ext == "docx") && !(ext == "doc") && !(ext == "pdf"))
							{
								list2.Add(FileName + "#notSupported");
							}
							else
							{
								if (!(ext == "docx") && !(ext == "doc") && !(ext == "pdf"))
								{
									continue;
								}
								FileName = FileName.Split('.')[0];
								UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.FileName == FileName).FirstOrDefault();
								if (uploadedDocument != null)
								{
									list2.Add(httpPostedFileBase.FileName + "#alreadyExists");
									continue;
								}
								string text = ReverseString(FileName);
								string[] source = text.Split('-');
								string text2 = null;
								if (source.Count() > 1)
								{
									foreach (var item2 in source.Select((string value, int o) => new { o, value }))
									{
										if (item2.o != 0)
										{
											text2 = ((item2.o >= source.Count() - 1) ? (text2 + item2.value.ToString()) : (text2 + item2.value.ToString() + "-"));
										}
									}
								}
								else
								{
									text2 = text;
								}
								string fname;
								string item = (fname = ReverseString(text2));
								UploadedFile Uploadedlst = ((db.UploadedFiles.Where((UploadedFile sa) => sa.FileName == FileName && sa.IsActive == true).FirstOrDefault() != null) ? db.UploadedFiles.Where((UploadedFile sa) => sa.FileName == FileName && sa.IsActive == true).FirstOrDefault() : ((db.UploadedFiles.Where((UploadedFile sa) => sa.FileName.Contains(fname.ToString()) && sa.IsActive == true).FirstOrDefault() != null) ? db.UploadedFiles.Where((UploadedFile sa) => sa.FileName.Contains(fname.ToString()) && sa.IsActive == true).FirstOrDefault() : null));
								if (Uploadedlst != null)
								{
									string[] array = Uploadedlst.FileName.Split('.');
									string text3 = array[0].ToString().Trim();
									string text4 = ReverseString(FileName);
									string[] source2 = text4.Split('-');
									string text5 = null;
									if (source2.Count() > 1)
									{
										foreach (var item3 in source2.Select((string value, int o) => new { o, value }))
										{
											if (item3.o != 0)
											{
												text5 = ((item3.o >= source.Count() - 1) ? (text5 + item3.value.ToString()) : (text5 + item3.value.ToString() + "-"));
											}
										}
									}
									else
									{
										text5 = text4;
									}
									string text6 = ReverseString(text5);
									if (text6 == fname)
									{
										if (base.Request.Browser.Browser.ToUpper() == "IE" || base.Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
										{
											ext = Path.GetExtension(httpPostedFileBase.FileName).Substring(1);
											string[] array2 = httpPostedFileBase.FileName.Split('\\');
											FileName = array2[array2.Length - 1];
										}
										else
										{
											ext = Path.GetExtension(httpPostedFileBase.FileName).Substring(1);
											FileName = Path.GetFileName(FileName);
										}
										int contentLength = httpPostedFileBase.ContentLength;
										int num = contentLength / 1000;
										string text7 = "/Content/Transcripts/" + FileName + "." + ext;
										string fileName = base.Server.MapPath("~\\" + text7);
										httpPostedFileBase.SaveAs(base.Server.MapPath(text7));
										UploadedDocument uploadedDocument2 = new UploadedDocument();
										ext = Path.GetExtension(httpPostedFileBase.FileName).Substring(1);
										uploadedDocument2.FileID = Uploadedlst.FileID;
										uploadedDocument2.FileName = FileName.Split('.')[0];
										uploadedDocument2.FilePath = text7;
										uploadedDocument2.Extension = ext;
										uploadedDocument2.Size = num.ToString();
										uploadedDocument2.IsApproved = false;
										uploadedDocument2.UploadBy = CurUserId;
										uploadedDocument2.UploadOn = CustomTimeZone.Get_US_UTC_Time();
										Document document = new Document();
										document.LoadFromFile(fileName, FileFormat.Docx);
										string text8 = base.Server.MapPath("/Content/Transcripts/" + FileName.Split('.')[0]);
										string path = text8 + ".txt";
										string text9 = document.GetText();
										System.IO.File.WriteAllText(path, text9.ToString());
										string input = text9;
										string pattern = "\\[(.*?)\\]";
										List<string> list4 = new List<string>();
										foreach (Match item4 in Regex.Matches(input, pattern))
										{
											if (item4.Success && item4.Groups.Count > 0)
											{
												string value2 = item4.Groups[1].Value;
												list4.Add(value2);
												string text10 = "";
												switch (value2.Split('=')[0].ToString().ToLower().Trim())
												{
												case "pt":
													uploadedDocument2.PatientName = value2.Split('=')[1];
													break;
												case "dob":
													uploadedDocument2.DOB = value2.Split('=')[1];
													break;
												case "dos":
													uploadedDocument2.DOS = value2.Split('=')[1];
													break;
												case "mrn":
													uploadedDocument2.MRN = value2.Split('=')[1];
													break;
												case "fax":
													uploadedDocument2.FaxNumber = value2.Split('=')[1];
													break;
												case "ref":
													uploadedDocument2.RefDoctor = value2.Split('=')[1];
													break;
												}
											}
										}
										if (list4.Count() > 0)
										{
											document.Replace("DOB=", "", caseSensitive: true, wholeWord: false);
											document.Replace("PT=", "", caseSensitive: true, wholeWord: false);
											document.Replace("DOS=", "", caseSensitive: true, wholeWord: false);
											document.Replace("MRN=", "", caseSensitive: true, wholeWord: false);
											document.Replace("Ref=", "", caseSensitive: true, wholeWord: false);
											document.Replace("Fax=", "", caseSensitive: true, wholeWord: false);
											document.Replace("[", "", caseSensitive: true, wholeWord: false);
											document.Replace("]", "", caseSensitive: true, wholeWord: false);
										}
										if (ext == "doc")
										{
											document.SaveToFile(fileName, FileFormat.Doc);
										}
										else
										{
											document.SaveToFile(fileName, FileFormat.Docx2013);
										}
										document.Close();
										if (System.IO.File.Exists(text8 + ".txt"))
										{
											System.IO.File.Delete(text8 + ".txt");
										}
										int num2 = 0;
										int num3 = 0;
										int num4 = 0;
										int? numOfLine = 0;
										UploadedFile uploadedFile = db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == Uploadedlst.FileID).FirstOrDefault();
										if (uploadedFile != null)
										{
											numOfLine = ((uploadedFile.Dictator != null) ? uploadedFile.Dictator.NoOfLine : new int?(0));
										}
										string fileName2 = FileName + "." + ext;
										Tuple<int, int, int> wordCount = GetWordCount(fileName2, numOfLine);
										if (wordCount != null)
										{
											num2 = wordCount.Item1;
											num3 = wordCount.Item2;
											num4 = wordCount.Item3;
											uploadedDocument2.LineCount = num2;
											uploadedDocument2.WordCount = num4;
											uploadedDocument2.CharCount = num3;
										}
										else
										{
											uploadedDocument2.LineCount = null;
											uploadedDocument2.WordCount = null;
											uploadedDocument2.CharCount = null;
										}
										uploadedDocument2.StatusId = 3;
										db.UploadedDocuments.Add(uploadedDocument2);
										db.SaveChanges();
										UploadedDocument uploadedDocument3 = db.UploadedDocuments.Where((UploadedDocument sa) => sa.FileName == FileName && sa.Extension == ext).FirstOrDefault();
										UserLog entity = new UserLog
										{
											UserLogId = CurUserId,
											UserLogName = CurUserName,
											DocID = uploadedDocument3.Id,
											AudioID = uploadedDocument3.FileID,
											FileName = uploadedDocument3.FileName,
											FileType = ext,
											Action = 8,
											Date = CustomTimeZone.Get_US_UTC_Time(),
											ProviderId = uploadedDocument3.UploadedFile.DictatorID,
											StatusId = 3
										};
										db.UserLogs.Add(entity);
										db.SaveChanges();
										UploadedFile uploadedFile2 = db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == Uploadedlst.FileID).FirstOrDefault();
										if (uploadedFile2 != null)
										{
											uploadedFile2.StatusId = 3;
											db.SaveChanges();
										}
										list.Add(httpPostedFileBase.FileName);
									}
									else
									{
										list.Add(fname);
									}
								}
								list.Add(item);
								continue;
							}
						}
						catch (Exception ex)
						{
							LogHelper.PrintError(ex);
							return Json(ex.Message, JsonRequestBehavior.AllowGet);
						}
					}
				}
				list3 = list.Concat(list2).ToList();
				return Json(list3, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex2)
			{
				LogHelper.PrintError(ex2);
				return Json(ex2.Message, JsonRequestBehavior.AllowGet);
			}
		}

		private string BreakLine(int Num, string Text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			string[] array = Text.Split(' ');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (num + text.Length > Num)
				{
					stringBuilder.Append(Environment.NewLine);
					num = 0;
				}
				stringBuilder.Append(text + " ");
				num += text.Length + 1;
			}
			return stringBuilder.ToString();
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

		public void GetText(string FileName)
		{
			string text = base.Server.MapPath("/Content/Transcripts/");
			Document document = new Document();
			document.LoadFromFile(text + FileName);
			string text2 = text + FileName.Split('.')[0];
			string text3 = text2 + ".txt";
			string text4 = document.GetText();
			System.IO.File.WriteAllText(text3, text4.ToString());
			if (!text3.Contains("<Patient Name = Rayyan Nasir>"))
			{
			}
		}

		public Tuple<int, int, int> GetWordCount(string FileName, int? NumOfLine)
		{
			try
			{
				string text = "";
				text = ((!(ConfigurationManager.AppSettings["appEnv"].ToString() == "Dev")) ? "C:\\inetpub\\vhosts\\excelonwork.com\\httpdocs\\Content\\Transcripts\\" : base.Server.MapPath("~/Content/Transcripts/"));
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
				document.Close();
				if (System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);
				}
				return new Tuple<int, int, int>(item, num, item2);
			}
			catch (Exception ex)
			{
				LogHelper.PrintGenericInfo("Word Count Error=" + ex);
				return null;
			}
		}

		private string AudioDuration(string FileFullPath)
		{
			TagLib.File file = TagLib.File.Create(FileFullPath);
			int num = (int)file.Properties.Duration.TotalSeconds;
			int num2 = num / 60;
			int num3 = num % 60;
			return num2 + ":" + num3 + " minutes";
		}

		[HttpGet]
		public ActionResult DownloadFiles(List<int> STRMSelected)
		{
			try
			{
				GetCurrentUserId();
				GetCurrentUserName();
				using (ZipFile zipFile = new ZipFile())
				{
					List<UploadedDocument> list = ((STRMSelected == null) ? db.UploadedDocuments.Where((UploadedDocument sa) => sa.DocChildID == (int?)null).ToList() : db.UploadedDocuments.Where((UploadedDocument sa) => STRMSelected.Contains(sa.Id)).ToList());
					zipFile.AlternateEncodingUsage = ZipOption.AsNecessary;
					zipFile.AddDirectoryByName("DocumentFiles");
					foreach (UploadedDocument file in list)
					{
						string text = file.FileName + "." + file.Extension;
						string text2 = base.Server.MapPath("~\\Content\\Transcripts\\");
						DirectoryInfo directoryInfo = new DirectoryInfo(base.Server.MapPath("~\\Content\\TempDownload"));
						if (directoryInfo != null)
						{
							string text3 = base.Server.MapPath("~/Content/TempDownload/") + text;
							FileInfo fileInfo = new FileInfo(text3.ToString());
							if (fileInfo.Exists)
							{
								text2 = base.Server.MapPath("~\\Content\\TempDownload\\");
							}
						}
						FileInfo fileInfo2 = new FileInfo(text2 + file.FileName + "." + file.Extension);
						if (fileInfo2.Exists)
						{
							string text4 = file.UploadedFile.Dictator.DictatorID + "-" + file.UploadedFile.Dictator.FirstName;
							zipFile.AddFile(text2 + file.FileName + "." + file.Extension, "DocumentFiles/" + text4);
							UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == file.Id).FirstOrDefault();
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
						}
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
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
			}
		}

		[HttpPost]
		public JsonResult DownloadFiles1(IEnumerable<int?> STRMSelected)
		{
			bool flag = false;
			GetCurrentUserId();
			GetCurrentUserName();
			try
			{
				List<int?> list = new List<int?>();
				List<UploadedDocument> list2 = ((STRMSelected.Count() <= 0) ? db.UploadedDocuments.ToList() : db.UploadedDocuments.Where((UploadedDocument sa) => STRMSelected.Contains(sa.Id)).ToList());
				if (list2 != null)
				{
					string text = "";
					string text2 = "";
					string text3 = "";
					string text4 = "";
					int num = 1;
					foreach (UploadedDocument item2 in list2)
					{
						UploadedDocument item = item2;
						text4 = "D:\\inetpub\\vhosts\\excel.mangotech-apps.com\\Content\\Transcripts\\DocumentFiles";
						text = item.FilePath;
						if (!Directory.Exists(text4))
						{
							Directory.CreateDirectory(text4);
							using (WebClient webClient = new WebClient())
							{
								string path = "D:/DocumentFiles/" + num + "-" + item.FileName;
								string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
								webClient.DownloadFile("D:\\inetpub\\vhosts\\excel.mangotech-apps.com\\Content\\Transcripts\\" + item.FilePath, "D:/DocumentFiles/" + num + "-" + item.FileName);
								num++;
								flag = true;
								UploadedFile uploadedFile = db.UploadedFiles.Where((UploadedFile sa) => (int?)sa.FileID == item.FileID).FirstOrDefault();
								if (uploadedFile != null)
								{
									uploadedFile.StatusId = 4;
									db.SaveChanges();
								}
							}
							continue;
						}
						using (WebClient webClient2 = new WebClient())
						{
							string text5 = "D:/DocumentFiles/" + num + "-" + item.FileName;
							string text6 = text5;
							while (true)
							{
								if (System.IO.File.Exists(text5))
								{
									string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(text5);
									string[] array = fileNameWithoutExtension2.Split('-');
									int num2 = Convert.ToInt32(array[0]);
									string text7 = num + "-" + array[1];
									string path2 = "D:/DocumentFiles/" + text7 + "." + item.Extension;
									if (System.IO.File.Exists(path2))
									{
										string fileNameWithoutExtension3 = Path.GetFileNameWithoutExtension(path2);
										string[] array2 = fileNameWithoutExtension3.Split('-');
										int num3 = Convert.ToInt32(array2[0]);
										if (num3 == num)
										{
											num++;
											continue;
										}
									}
									webClient2.DownloadFile("D:\\inetpub\\vhosts\\excel.mangotech-apps.com\\Content\\Transcripts\\" + item.FilePath, "D:/DocumentFiles/" + num + "-" + item.FileName);
									flag = true;
									UploadedFile uploadedFile2 = db.UploadedFiles.Where((UploadedFile sa) => (int?)sa.FileID == item.FileID).FirstOrDefault();
									if (uploadedFile2 != null)
									{
										uploadedFile2.StatusId = 4;
										db.SaveChanges();
									}
								}
								else
								{
									webClient2.DownloadFile("D:\\inetpub\\vhosts\\excel.mangotech-apps.com\\Content\\Transcripts\\" + item.FilePath, "D:/DocumentFiles/" + num + "-" + item.FileName);
									num++;
									flag = true;
									UploadedFile uploadedFile3 = db.UploadedFiles.Where((UploadedFile sa) => (int?)sa.FileID == item.FileID).FirstOrDefault();
									if (uploadedFile3 != null)
									{
										uploadedFile3.StatusId = 4;
										db.SaveChanges();
									}
								}
								break;
							}
						}
					}
					return Json(flag, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception error)
			{
				LogHelper.PrintError(error);
				return Json(flag, JsonRequestBehavior.AllowGet);
			}
			return Json(flag, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult UpdateStatus(int Id)
		{
			GetCurrentUserId();
			GetCurrentUserName();
			UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == Id).FirstOrDefault();
			if (uploadedDocument != null)
			{
				UserLog userLog = new UserLog();
				userLog.UserLogId = CurUserId;
				userLog.UserLogName = CurUserName;
				userLog.FileName = uploadedDocument.FileName.Split('.')[0];
				userLog.FileType = uploadedDocument.Extension;
				userLog.Action = 0;
				userLog.ProviderId = uploadedDocument.UploadedFile.DictatorID;
				userLog.Date = CustomTimeZone.Get_US_UTC_Time();
				userLog.AudioID = uploadedDocument.FileID;
				userLog.DocID = uploadedDocument.Id;
				userLog.StatusId = uploadedDocument.StatusId;
				UserLog entity = userLog;
				db.UserLogs.Add(entity);
				db.SaveChanges();
			}
			return RedirectToAction("Index");
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

		public ActionResult DocumentReport(bool? btn, int? DictatorID, int? ClinicID, int? StatusId, DateTime? fromDate, DateTime? toDate, int? TypistAssignId)
		{
			List<Temp_DocumentList> list = new List<Temp_DocumentList>();
			if (btn.HasValue)
			{
				List<stp_DocumentReport_Result> source = (from sa in db.stp_DocumentReport(DictatorID, ClinicID, StatusId, fromDate, toDate, TypistAssignId)
					orderby sa.DocFile
					select sa).ToList();
				var list2 = source.Select((stp_DocumentReport_Result s) => new { s.ProviderId }).Distinct().ToList();
				foreach (var item in list2)
				{
					Temp_DocumentList temp_DocumentList = new Temp_DocumentList();
					temp_DocumentList.GTotalLines = Convert.ToDecimal(source.Sum((stp_DocumentReport_Result sa) => sa.LineCount));
					temp_DocumentList.GTotalAmount = Convert.ToDecimal(source.Sum((stp_DocumentReport_Result sa) => sa.LineCount * (decimal?)sa.NoOfCharacterPerLine));
					temp_DocumentList.GTotalDoc = source.Count();
					List<stp_DocumentReport_Result> source2 = (temp_DocumentList.Doclst = source.Where((stp_DocumentReport_Result x) => x.ProviderId == item.ProviderId).ToList());
					temp_DocumentList.TotalLines = Convert.ToDecimal(source2.Sum((stp_DocumentReport_Result sa) => sa.LineCount));
					temp_DocumentList.TotalAmount = Convert.ToDecimal(source2.Sum((stp_DocumentReport_Result sa) => sa.LineCount * (decimal?)sa.NoOfCharacterPerLine));
					temp_DocumentList.TotalDoc = source2.Count();
					temp_DocumentList.Clinic = source.Where((stp_DocumentReport_Result sa) => sa.ProviderId == item.ProviderId).FirstOrDefault().Clinic;
					temp_DocumentList.Provider = source.Where((stp_DocumentReport_Result sa) => sa.ProviderId == item.ProviderId).FirstOrDefault().Provider;
					list.Add(temp_DocumentList);
				}
				base.ViewBag.GetTypist = (from x in db.stp_GetTypist()
					select new
					{
						TypistAssignID = x.UserId,
						Name = x.UserName
					}).ToList();
				if (list == null || list.Count() == 0)
				{
					list = new List<Temp_DocumentList>();
				}
				if (list.Count() > 0)
				{
					return PartialView("_DocumentReport", list);
				}
				return PartialView("_DocumentReport", list);
			}
			base.ViewBag.DictatorID = (from sa in db.Dictators
				select new
				{
					value = sa.DictatorID,
					text = sa.LoginID
				} into s
				orderby s.text
				select s).ToList();
			base.ViewBag.GetTypist = from s in GetTypist()
				orderby s.Text
				select s;
			base.ViewBag.StatusID = from s in db.Status.Select((Status sa) => new
				{
					value = sa.Id,
					text = sa.Name
				}).ToList()
				orderby s.value == 4, s.value
				select s;
			base.ViewBag.ClinicID = (from sa in db.Clinics
				select new
				{
					value = sa.ClinicID,
					text = sa.Name
				} into s
				orderby s.text
				select s).ToList();
			return View("DocumentReport");
		}

		[HttpPost]
		public JsonResult CheckMark(int DocId, int ModalityId)
		{
			UploadedDocument uploadedDocument = db.UploadedDocuments.Where((UploadedDocument sa) => sa.Id == DocId).FirstOrDefault();
			bool flag = false;
			if (uploadedDocument != null)
			{
				if (ModalityId > 0)
				{
					uploadedDocument.ModalityId = ModalityId;
				}
				int num = Convert.ToInt32(uploadedDocument.ModalityId);
				db.SaveChanges();
				if (num > 0)
				{
					flag = true;
				}
			}
			return Json(flag);
		}
	}
}
