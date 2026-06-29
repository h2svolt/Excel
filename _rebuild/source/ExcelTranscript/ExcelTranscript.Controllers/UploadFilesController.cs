using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExcelTranscript.Helpers;
using ExcelTranscript.Models;
using ExcelTranscript.Models.Helper;
using Ionic.Zip;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using NAudio.Wave;
using TagLib;

namespace ExcelTranscript.Controllers
{
	[Authorize]
	public class UploadFilesController : Controller
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public int CurUserId { get; set; }

		public string CurUserName { get; set; }

		[HttpPost]
		public async Task<JsonResult> SingleUploadAsync(int? DictatorID, string duration = "")
		{
			try
			{
				GetCurrentUser();
				List<ViewDataUploadFilesResult> resultList = new List<ViewDataUploadFilesResult>();
				string FileName = "";
				HttpFileCollectionBase files = base.Request.Files;
				if (files.Count > 0)
				{
					string folder = "";
					for (int i = 0; i < files.Count; i++)
					{
						HttpPostedFileBase file = files[i];
						try
						{
							FileName = file.FileName.Split('.')[0];
							if ((await db.UploadedFiles.Where((UploadedFile sa) => sa.FileName == FileName && sa.IsActive == true).FirstOrDefaultAsync())?.IsActive ?? false)
							{
								ViewDataUploadFilesResult uploadFiles = new ViewDataUploadFilesResult
								{
									name = file.FileName,
									size = file.ContentLength,
									message = "File Already Exists!"
								};
								resultList.Add(uploadFiles);
								continue;
							}
							string ext = Path.GetExtension(file.FileName.ToLower()).Substring(1);
							FileName = Path.GetFileName(file.FileName);
							int num;
							switch (ext)
							{
							default:
								num = ((!(ext == "mp4")) ? 1 : 0);
								break;
							case "mp3":
							case "ds2":
							case "dss":
							case "ogg":
							case "wav":
							case "avi":
							case "mov":
							case "3gp":
								num = 0;
								break;
							}
							if (num != 0)
							{
								ViewDataUploadFilesResult uploadFiles2 = new ViewDataUploadFilesResult
								{
									name = file.FileName,
									size = file.ContentLength,
									message = "File Not Supported!"
								};
								resultList.Add(uploadFiles2);
								continue;
							}
							int num2;
							switch (ext)
							{
							default:
								num2 = ((ext == "mp4") ? 1 : 0);
								break;
							case "mp3":
							case "ds2":
							case "dss":
							case "ogg":
							case "wav":
							case "avi":
							case "mov":
							case "3gp":
								num2 = 1;
								break;
							}
							if (num2 != 0)
							{
								if (base.Request.Browser.Browser.ToUpper() == "IE" || base.Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
								{
									Path.GetExtension(file.FileName).Substring(1);
									string[] testfiles = file.FileName.Split('\\');
									_ = testfiles[testfiles.Length - 1];
								}
								else
								{
									Path.GetExtension(file.FileName).Substring(1);
									_ = file.FileName;
									FileName = Path.GetFileName(file.FileName);
								}
								Dictator objDic = db.Dictators.Where((Dictator sa) => (int?)sa.DictatorID == DictatorID).FirstOrDefault();
								if (objDic != null)
								{
									folder = objDic.FirstName;
								}
								string Dirpath = base.Server.MapPath("/Content/Transcripts/");
								string Dirpathfile = Dirpath + folder;
								if (!Directory.Exists(Dirpathfile))
								{
									Directory.CreateDirectory(Dirpathfile);
								}
								string path = "/Content/Transcripts/" + folder + "/" + FileName;
								int fileSize = file.ContentLength;
								int Size = fileSize / 1000;
								using (FileStream fs = new FileStream(base.Server.MapPath(path), FileMode.Create))
								{
									byte[] bytes = await GetBytesFromStreamAsync(file.InputStream);
									await fs.WriteAsync(bytes, 0, bytes.Length);
								}
								UploadedFile model = new UploadedFile
								{
									DictatorID = DictatorID,
									ClinicID = db.Dictators.Where((Dictator sa) => (int?)sa.DictatorID == DictatorID).FirstOrDefault().AccountID
								};
								Path.GetFullPath(file.FileName);
								ext = Path.GetExtension(file.FileName).Substring(1);
								if (ext == "dss" || ext == "ds2")
								{
									model.Duration = GetDssDuration(base.Server.MapPath(path)).ToString();
								}
								else if (ext == "wav" && !string.IsNullOrEmpty(duration))
								{
									model.Duration = duration;
								}
								else
								{
									model.Duration = GetFileDuration(base.Server.MapPath(path));
								}
								model.FileName = FileName.Split('.')[0];
								model.StatusId = 1;
								model.Extension = ext;
								model.Size = Size.ToString();
								model.IsActive = true;
								model.FilePath = path;
								model.UploadBy = CurUserId;
								model.UploadOn = CustomTimeZone.Get_US_UTC_Time();
								db.UploadedFiles.Add(model);
								await db.SaveChangesAsync();
								UserLog log = new UserLog
								{
									UserLogId = CurUserId,
									UserLogName = CurUserName,
									FileName = FileName.Split('.')[0],
									FileType = ext,
									Action = 1,
									Date = CustomTimeZone.Get_US_UTC_Time(),
									StatusId = 1,
									ProviderId = DictatorID
								};
								int AudioID = db.UploadedFiles.OrderByDescending((UploadedFile sa) => sa.FileID).FirstOrDefault().FileID;
								log.AudioID = AudioID;
								db.UserLogs.Add(log);
								await db.SaveChangesAsync();
								ViewDataUploadFilesResult uploadFiles3 = new ViewDataUploadFilesResult
								{
									name = file.FileName,
									size = file.ContentLength,
									message = "uploaded"
								};
								resultList.Add(uploadFiles3);
							}
							if (!string.IsNullOrEmpty(duration))
							{
								var response = new
								{
									message = "Audio Uploaded"
								};
								return Json(response, JsonRequestBehavior.AllowGet);
							}
							return Json(new JsonFiles(resultList));
						}
						catch (Exception error)
						{
							LogHelper.PrintError(error);
							ViewDataUploadFilesResult uploadFiles4 = new ViewDataUploadFilesResult
							{
								name = file.FileName,
								size = file.ContentLength,
								message = "Error uploading file."
							};
							resultList.Add(uploadFiles4);
							return Json(new JsonFiles(resultList));
						}
					}
				}
				ViewDataUploadFilesResult upload = new ViewDataUploadFilesResult
				{
					name = "",
					size = 0,
					message = "Error Uploading file"
				};
				resultList.Add(upload);
				return Json(new JsonFiles(resultList));
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				if (base.User.IsInRole("Provider"))
				{
					base.ViewBag.ClinicID = new SelectList(db.Clinics.Where((Clinic sa) => sa.AddedBy == CurUserId), "ClinicID", "Name");
					base.ViewBag.GetTypist = GetTypist();
					base.ViewBag.DictatorID = (from sa in db.Dictators
						where sa.LoginID == CurUserName
						select new
						{
							value = sa.DictatorID,
							Text = sa.FirstName
						}).ToList();
				}
				else if (!base.User.IsInRole("Typist"))
				{
					base.ViewBag.ClinicID = new SelectList(db.Clinics, "ClinicID", "Name");
					base.ViewBag.GetTypist = GetTypist();
					base.ViewBag.DictatorID = db.Dictators.Select((Dictator sa) => new
					{
						value = sa.DictatorID,
						Text = sa.FirstName
					}).ToList();
				}
				else
				{
					base.ViewBag.ClinicID = new SelectList(db.Clinics.Where((Clinic sa) => sa.AddedBy == CurUserId), "ClinicID", "Name");
					base.ViewBag.GetTypist = GetTypist();
					base.ViewBag.DictatorID = (from sa in db.Dictators
						where sa.AddedBy == CurUserId
						select new
						{
							value = sa.DictatorID,
							Text = sa.FirstName
						}).ToList();
				}
				LogHelper.PrintError(ex2);
				return Json(ex2.Message, JsonRequestBehavior.AllowGet);
			}
		}

		private async Task<byte[]> GetBytesFromStreamAsync(Stream input)
		{
			byte[] buffer = new byte[input.Length];
			using (MemoryStream ms = new MemoryStream())
			{
				while (true)
				{
					int num;
					int read = (num = input.Read(buffer, 0, buffer.Length));
					if (num <= 0)
					{
						break;
					}
					await ms.WriteAsync(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		private string GetCurrentUser()
		{
			IIdentity identity = base.User.Identity;
			string currentUserId = base.User.Identity.GetUserId();
			AspNetUser aspNetUser = db.AspNetUsers.FirstOrDefault((AspNetUser x) => x.Id == currentUserId);
			CurUserName = aspNetUser.UserName;
			CurUserId = aspNetUser.UserId;
			return aspNetUser.UserName;
		}

		public ActionResult Index(bool? btn, string DictatorID, string ClinicID, int? TypistID, int? StatusId, DateTime? fromDate, DateTime? toDate)
		{
			GetCurrentUser();
			if (btn.HasValue)
			{
				try
				{
					List<stp_GetDictations_Result> model = (base.User.IsInRole(UserRoles.Provider) ? (from sa in db.stp_GetDictations(DictatorID, ClinicID, TypistID, StatusId, fromDate, toDate)
						where sa.DictatorID == Convert.ToInt32(DictatorID)
						select sa).ToList() : (base.User.IsInRole(UserRoles.Manager) ? db.stp_GetDictations(DictatorID, ClinicID, TypistID, StatusId, fromDate, toDate).ToList() : ((!base.User.IsInRole(UserRoles.Typist)) ? db.stp_GetDictations(DictatorID, ClinicID, TypistID, StatusId, fromDate, toDate).ToList() : (from sa in db.stp_GetDictations(DictatorID, ClinicID, TypistID, StatusId, fromDate, toDate)
						where sa.TypistAssignID == CurUserId
						select sa).ToList())));
					return PartialView("_Index", model);
				}
				catch (Exception)
				{
					return View();
				}
			}
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
			else if (base.User.IsInRole(UserRoles.Manager))
			{
				int managerID = db.Managers.Where((Manager m) => m.Username == CurUserName).FirstOrDefault().ManagerID;
				List<int> managerClinics = (from m in db.ManagerClinics
					where m.ManagerID == managerID
					select m into x
					select x.ClinicID).ToList();
				List<int> managerProviders = (from m in db.ManagerClinics
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
			return View();
		}

		public ActionResult Upload()
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

		[HttpGet]
		public ActionResult DownloadFiles(List<int> STRMSelected)
		{
			GetCurrentUser();
			using (ZipFile zipFile = new ZipFile())
			{
				List<UploadedFile> list = ((STRMSelected == null) ? db.UploadedFiles.ToList() : db.UploadedFiles.Where((UploadedFile sa) => STRMSelected.Contains(sa.FileID)).ToList());
				zipFile.AlternateEncodingUsage = ZipOption.AsNecessary;
				string text = "";
				foreach (UploadedFile file in list)
				{
					text = file.Dictator.FirstName;
					string text2 = base.Server.MapPath("\\Content\\Transcripts\\" + text + "\\");
					try
					{
						zipFile.AddFile(text2 + file.FileName + "." + file.Extension, text);
					}
					catch (Exception)
					{
					}
					UploadedFile uploadedFile = db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == file.FileID).FirstOrDefault();
					uploadedFile.IsDownloaded = true;
					db.Entry(uploadedFile).State = EntityState.Modified;
					UserLog userLog = new UserLog();
					userLog.UserLogId = CurUserId;
					userLog.UserLogName = CurUserName;
					userLog.AudioID = uploadedFile.FileID;
					userLog.FileName = uploadedFile.FileName.Split('.')[0];
					userLog.FileType = uploadedFile.Extension;
					userLog.Action = 0;
					userLog.Date = CustomTimeZone.Get_US_UTC_Time();
					userLog.StatusId = uploadedFile.StatusId;
					userLog.ProviderId = uploadedFile.DictatorID;
					UserLog entity = userLog;
					db.UserLogs.Add(entity);
					db.SaveChanges();
				}
				string fileDownloadName = string.Format("FilesZip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
				using (MemoryStream memoryStream = new MemoryStream())
				{
					zipFile.Save(memoryStream);
					return File(memoryStream.ToArray(), "application/zip", fileDownloadName);
				}
			}
		}

		[HttpPost]
		public JsonResult DownloadFiles1(List<int> STRMSelected)
		{
			bool flag = false;
			GetCurrentUser();
			try
			{
				List<string> list = new List<string>();
				List<UploadedFile> list2 = ((STRMSelected.Count() <= 0) ? db.UploadedFiles.ToList() : db.UploadedFiles.Where((UploadedFile sa) => STRMSelected.Contains(sa.FileID)).ToList());
				if (list2 != null)
				{
					string text = "";
					string text2 = "";
					string text3 = "";
					string text4 = "";
					int num = 1;
					int num2 = 0;
					list.AddRange(list2.Select((UploadedFile sa) => sa.Dictator.Clinic.Name).ToList());
					foreach (UploadedFile item2 in list2)
					{
						UploadedFile item = item2;
						text3 = item.Dictator.Clinic.Name;
						text = "C:\\";
						text4 = text + text3;
						text = item.FilePath;
						string text5 = "";
						if (list.Contains(item.Dictator.Clinic.Name))
						{
							if (!Directory.Exists(text4))
							{
								Directory.CreateDirectory(text4);
								using (WebClient webClient = new WebClient())
								{
									string path = "C:/" + text3 + " / " + item.FileName;
									string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
									text5 = "D:\\Latest Office Project And Scripts\\ExcelTranscript\\ExcelTranscript\\Content\\Transcripts";
									webClient.DownloadFile(text5, "C:/" + text3 + " / " + item.FileName);
									num++;
									flag = true;
									UploadedFile uploadedFile = db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == item.FileID).FirstOrDefault();
									if (uploadedFile != null)
									{
										uploadedFile.StatusId = 2;
										db.SaveChanges();
									}
								}
								continue;
							}
							using (WebClient webClient2 = new WebClient())
							{
								text5 = "D:\\Projects\\ExcelTrans\\ExcelTranscript\\ExcelTranscript\\Content\\Transcripts";
								webClient2.DownloadFile(text5, "C:/" + text3 + " / " + item.FileName);
								flag = true;
								UploadedFile uploadedFile2 = db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == item.FileID).FirstOrDefault();
								if (uploadedFile2 != null)
								{
									uploadedFile2.StatusId = 2;
									db.SaveChanges();
								}
							}
							continue;
						}
						list.Add(item.Dictator.Clinic.Name);
						Directory.CreateDirectory(text4);
						using (WebClient webClient3 = new WebClient())
						{
							text5 = "D:\\Latest Office Project And Scripts\\ExcelTranscript\\ExcelTranscript\\Content\\Transcripts";
							webClient3.DownloadFile(text5, base.Server.MapPath("~/Downloads/" + text3 + " / " + item.FileName));
							flag = true;
							UploadedFile uploadedFile3 = db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == item.FileID).FirstOrDefault();
							if (uploadedFile3 != null)
							{
								uploadedFile3.StatusId = 2;
								db.SaveChanges();
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

		public ActionResult TypistList()
		{
			List<UploadedFile> model = db.UploadedFiles.Where((UploadedFile sa) => sa.TypistAssignID != (int?)null).ToList();
			return View(model);
		}

		public ActionResult AddTypistRole()
		{
			List<UploadedFile> model = db.UploadedFiles.ToList();
			base.ViewBag.GetTypist = GetTypist();
			return View(model);
		}

		[HttpPost]
		public async Task<JsonResult> AssignRoles(int AudioID, int TypistID)
		{
			GetCurrentUser();
			bool IsSuccess = false;
			UploadedFile uploadedFile = db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == AudioID).FirstOrDefault();
			if (uploadedFile != null)
			{
				uploadedFile.TypistAssignID = TypistID;
				uploadedFile.StatusId = 2;
				db.Entry(uploadedFile).State = EntityState.Modified;
				await db.SaveChangesAsync();
				IsSuccess = true;
				UserLog log = new UserLog
				{
					UserLogId = CurUserId,
					UserLogName = CurUserName,
					FileName = uploadedFile.FileName,
					FileType = uploadedFile.Extension,
					Action = 2,
					Date = CustomTimeZone.Get_US_UTC_Time(),
					StatusId = 2,
					ProviderId = uploadedFile.DictatorID,
					AudioID = AudioID
				};
				db.UserLogs.Add(log);
				await db.SaveChangesAsync();
			}
			return Json(IsSuccess, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public async Task<JsonResult> TypistRole(List<string> lst, int? TypistID)
		{
			GetCurrentUser();
			int c = 0;
			if (lst != null && lst.Count() > 0)
			{
				foreach (string item in lst)
				{
					int AudioID = Convert.ToInt32(item);
					UploadedFile obj = await db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == AudioID).FirstOrDefaultAsync();
					if (obj != null)
					{
						obj.TypistAssignID = TypistID;
						obj.StatusId = 2;
						db.Entry(obj).State = EntityState.Modified;
						c += db.SaveChanges();
						UserLog log = new UserLog
						{
							UserLogId = CurUserId,
							UserLogName = CurUserName,
							FileName = obj.FileName,
							FileType = obj.Extension,
							Action = 2,
							Date = CustomTimeZone.Get_US_UTC_Time(),
							StatusId = 2,
							ProviderId = obj.DictatorID,
							AudioID = AudioID
						};
						db.UserLogs.Add(log);
						await db.SaveChangesAsync();
					}
				}
			}
			if (c > 0)
			{
				return Json(c, JsonRequestBehavior.AllowGet);
			}
			return Json(-1, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public async Task<JsonResult> GetDocumentCounts(int? AudioID)
		{
			var data = await (from sa in db.UploadedDocuments
				where sa.FileID == AudioID
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
			List<object> arr = new List<object>();
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
			return Json(arr, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public async Task<JsonResult> BulkRemove(string DicIds)
		{
			try
			{
				GetCurrentUser();
				List<string> IdsList = DicIds.Split(',').ToList();
				foreach (string Id in IdsList)
				{
					int FileID = Convert.ToInt32(Id);
					UploadedFile obj = await db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == FileID).FirstOrDefaultAsync();
					if (obj == null)
					{
						continue;
					}
					if (obj.StatusId == 5)
					{
						obj.IsActive = false;
						db.Entry(obj).State = EntityState.Modified;
					}
					else if (await db.UploadedDocuments.CountAsync((UploadedDocument f) => f.FileID == (int?)FileID) > 0)
					{
						obj.IsActive = false;
						db.Entry(obj).State = EntityState.Modified;
					}
					else
					{
						List<UserLog> logs = await db.UserLogs.Where((UserLog l) => l.UploadedFile.FileID == FileID).ToListAsync();
						if (logs.Count > 0)
						{
							db.UserLogs.RemoveRange(logs);
						}
						db.UploadedFiles.Remove(obj);
					}
					await db.SaveChangesAsync();
					string folder = "";
					Dictator objDic = await db.Dictators.Where((Dictator sa) => (int?)sa.DictatorID == obj.DictatorID).FirstOrDefaultAsync();
					if (objDic != null)
					{
						folder = objDic.FirstName;
					}
					string path = base.Server.MapPath("/Content/Transcripts/" + folder + "/" + obj.FileName + "." + obj.Extension);
					FileInfo chkfile = new FileInfo(path);
					if (chkfile.Exists)
					{
						chkfile.Delete();
					}
				}
				return Json(new
				{
					result = "Dictations removed successfully!"
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				return Json("Error deleting dictation!", JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public async Task<JsonResult> RemoveDictation(int FileID)
		{
			try
			{
				GetCurrentUser();
				UploadedFile obj = await db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == FileID).FirstOrDefaultAsync();
				if (obj != null)
				{
					int DictatorID = obj.DictatorID.GetValueOrDefault();
					if (obj.StatusId == 5)
					{
						obj.IsActive = false;
						db.Entry(obj).State = EntityState.Modified;
					}
					else if (await db.UploadedDocuments.CountAsync((UploadedDocument f) => f.FileID == (int?)FileID) > 0)
					{
						obj.IsActive = false;
						db.Entry(obj).State = EntityState.Modified;
					}
					else
					{
						List<UserLog> logs = await db.UserLogs.Where((UserLog l) => l.UploadedFile.FileID == FileID).ToListAsync();
						if (logs.Count > 0)
						{
							db.UserLogs.RemoveRange(logs);
						}
						db.UploadedFiles.Remove(obj);
					}
					await db.SaveChangesAsync();
					string folder = "";
					Dictator objDic = await db.Dictators.Where((Dictator sa) => sa.DictatorID == DictatorID).FirstOrDefaultAsync();
					if (objDic != null)
					{
						folder = objDic.FirstName;
					}
					string path = base.Server.MapPath("/Content/Transcripts/" + folder + "/" + obj.FileName + "." + obj.Extension);
					FileInfo chkfile = new FileInfo(path);
					if (chkfile.Exists)
					{
						chkfile.Delete();
					}
					return Json(true, JsonRequestBehavior.AllowGet);
				}
				return Json("Dictation record not found!", JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				return Json("Error deleting dictation!", JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public ActionResult UpdateStatus(int FileID)
		{
			GetCurrentUser();
			DateTime? dateTime = null;
			DateTime now = DateTime.Now;
			UploadedFile uploadedFile = db.UploadedFiles.Where((UploadedFile sa) => sa.FileID == FileID && sa.IsActive == true).FirstOrDefault();
			uploadedFile.IsDownloaded = true;
			db.Entry(uploadedFile).State = EntityState.Modified;
			db.SaveChanges();
			UserLog entity = new UserLog
			{
				UserLogId = CurUserId,
				UserLogName = CurUserName,
				AudioID = uploadedFile.FileID,
				FileName = uploadedFile.FileName,
				FileType = uploadedFile.Extension,
				Action = 0,
				Date = CustomTimeZone.Get_US_UTC_Time(),
				StatusId = uploadedFile.StatusId,
				ProviderId = uploadedFile.DictatorID
			};
			db.UserLogs.Add(entity);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult SearchTypistList(bool? btn, int? DictatorID, int? TypistAssignId, int? StatusId, int? ClinicID, DateTime? fromDate, DateTime? toDate)
		{
			if (btn.HasValue)
			{
				base.ViewBag.GetTypist = (from x in db.stp_GetTypist()
					select new
					{
						TypistAssignID = x.UserId,
						Name = x.UserName
					} into s
					orderby s.Name
					select s).ToList();
				List<stp_GetUploadedAudioInfo_Result> list = new List<stp_GetUploadedAudioInfo_Result>();
				list = ((StatusId == 1) ? (from sa in db.stp_GetUploadedAudioInfo(DictatorID, ClinicID, null, fromDate, toDate, TypistAssignId).DistinctBy((stp_GetUploadedAudioInfo_Result d) => d.AudioFile)
					where !sa.TypistAssignID.HasValue
					orderby sa.TypistAssignID
					select sa).ToList() : ((StatusId != 2) ? (from sa in db.stp_GetUploadedAudioInfo(DictatorID, ClinicID, null, fromDate, toDate, TypistAssignId).DistinctBy((stp_GetUploadedAudioInfo_Result d) => d.AudioFile)
					orderby sa.TypistAssignID
					select sa).ToList() : (from sa in db.stp_GetUploadedAudioInfo(DictatorID, ClinicID, null, fromDate, toDate, TypistAssignId).DistinctBy((stp_GetUploadedAudioInfo_Result d) => d.AudioFile)
					where sa.TypistAssignID.HasValue
					orderby sa.TypistAssignID
					select sa).ToList()));
				string text = "";
				foreach (stp_GetUploadedAudioInfo_Result item in list)
				{
					if (!string.IsNullOrEmpty(item.Duration))
					{
						try
						{
							item.Duration.Trim();
							text = item.Duration.Substring(0, 4);
						}
						catch (Exception)
						{
						}
					}
				}
				if (list == null || list.Count() == 0)
				{
					list = new List<stp_GetUploadedAudioInfo_Result>();
				}
				if (list.Count() > 0)
				{
					return PartialView("_AddTypistRole", list);
				}
				return PartialView("_AddTypistRole", list);
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
			base.ViewBag.ClinicID = (from sa in db.Clinics
				select new
				{
					value = sa.ClinicID,
					text = sa.Name
				} into s
				orderby s.text
				select s).ToList();
			return View("AddTypistRole");
		}

		private string GetFileDuration(string FilePath)
		{
			TagLib.File file = TagLib.File.Create(FilePath);
			double totalSeconds = file.Properties.Duration.TotalSeconds;
			int num = Convert.ToInt32(totalSeconds / 60.0);
			int num2 = Convert.ToInt32(totalSeconds % 60.0);
			return $"{num:0#}:{num2:0#}";
		}

		private TimeSpan GetDssDuration(string fileName)
		{
			using (FileStream fileStream = System.IO.File.OpenRead(fileName))
			{
				byte[] array = new byte[69];
				fileStream.Read(array, 0, 68);
				byte[] array2 = new byte[2];
				byte[] array3 = new byte[2];
				byte[] array4 = new byte[2];
				Array.Copy(array, 62, array2, 0, 2);
				Array.Copy(array, 64, array3, 0, 2);
				Array.Copy(array, 66, array4, 0, 2);
				return new TimeSpan(int.Parse(Encoding.ASCII.GetString(array2)), int.Parse(Encoding.ASCII.GetString(array3)), int.Parse(Encoding.ASCII.GetString(array4)));
			}
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

		public static int GetSoundLength(string fileName)
		{
			using (WaveFileReader waveFileReader = new WaveFileReader(fileName))
			{
				return (int)waveFileReader.TotalTime.TotalMilliseconds;
			}
		}

		public static TimeSpan GetWavFileDuration(string fileName)
		{
			using (Mp3FileReader sourceProvider = new Mp3FileReader(fileName))
			{
				WaveFileWriter.CreateWaveFile("myfile", sourceProvider);
			}
			WaveFileReader waveFileReader = new WaveFileReader(fileName);
			return waveFileReader.TotalTime;
		}
	}
}
