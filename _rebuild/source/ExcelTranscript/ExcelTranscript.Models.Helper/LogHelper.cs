using System;
using log4net;
using log4net.Config;

namespace ExcelTranscript.Models.Helper
{
	public static class LogHelper
	{
		private static ILog genericLogger;

		private const string NA = "NA";

		private const string GENERIC_LOGGER = "GenericLogging";

		static LogHelper()
		{
			XmlConfigurator.Configure();
			genericLogger = LogManager.GetLogger("GenericLogging");
		}

		public static void PrintGenericInfo(string info)
		{
			try
			{
				if (genericLogger.IsInfoEnabled)
				{
					genericLogger.Info("@@Info: " + info + "\n");
				}
			}
			catch (Exception ex)
			{
				if (genericLogger.IsErrorEnabled)
				{
					genericLogger.Error("@@LogError: " + ex.StackTrace.ToString() + "\n");
				}
			}
		}

		public static void PrintGenericInfo(string info, string fileName, bool isMultipleFile)
		{
			genericLogger = LogManager.GetLogger(fileName);
			try
			{
				if (genericLogger.IsInfoEnabled)
				{
					genericLogger.Info("@@Info: " + info + "\n");
				}
			}
			catch (Exception ex)
			{
				if (genericLogger.IsErrorEnabled)
				{
					genericLogger.Error("@@LogError: " + ex.StackTrace.ToString() + "\n");
				}
			}
		}

		public static void PrintGenericInfo(string line, params object[] list)
		{
			try
			{
				string text = "@@Info: ";
				if (genericLogger.IsInfoEnabled)
				{
					genericLogger.InfoFormat(text + line, list);
				}
			}
			catch (Exception ex)
			{
				if (genericLogger.IsErrorEnabled)
				{
					genericLogger.Error(" @@LogError:" + ex.StackTrace.ToString() + "\n");
				}
			}
		}

		public static void PrintDebug(string debug)
		{
			try
			{
				if (genericLogger.IsDebugEnabled)
				{
					genericLogger.Debug(" @@Debug:" + debug + "\n");
				}
			}
			catch (Exception ex)
			{
				if (genericLogger.IsErrorEnabled)
				{
					genericLogger.Error(" @@LogError:" + ex.StackTrace.ToString() + "\n");
				}
			}
		}

		public static void PrintError(Exception error)
		{
			PrintError(string.Empty, error);
		}

		public static void PrintError(string customMessage)
		{
			PrintError(customMessage, null);
		}

		public static void PrintError(string customMessage, Exception error)
		{
			try
			{
				string text = "NA";
				string text2 = "NA";
				string text3 = "NA";
				string text4 = "NA";
				if (string.IsNullOrEmpty(customMessage))
				{
					customMessage = "NA";
				}
				if (error != null)
				{
					text = error.Message;
					text2 = error.StackTrace;
					if (error.InnerException != null)
					{
						text3 = error.InnerException.Message;
						text4 = error.InnerException.StackTrace;
					}
				}
				if (genericLogger.IsErrorEnabled)
				{
					if (error == null || error.InnerException == null)
					{
						genericLogger.Error($"{DateTime.Now:dd/MM/yyyy} \n@@Custom Message: {customMessage}\n-----------------\n@@Error Message: {text}\n-----------------\n@@Error Trace: {text2}");
					}
					else
					{
						genericLogger.Error($"{DateTime.Now:dd/MM/yyyy} \n@@Custom Message: {customMessage}\n-----------------\n@@Error Message: {text}\n-----------------\n@@Error Trace: {text2} \n **** INNER EXCEPTION ****************************** \n@@Inner Error Message: {text3}\n-----------------\n@@Inner Error Trace: {text4}");
					}
				}
			}
			catch (Exception ex)
			{
				if (genericLogger.IsErrorEnabled)
				{
					genericLogger.Error(" @@LogError: " + ex.StackTrace.ToString() + "\n");
				}
			}
		}

		public static void PrintLoggingDetails()
		{
			try
			{
				if (genericLogger.IsInfoEnabled)
				{
					genericLogger.Info("@@ IsInfoEnabled = TRUE.");
				}
				if (genericLogger.IsWarnEnabled)
				{
					genericLogger.Warn("@@ IsWarnEnabled = TRUE.");
				}
				if (genericLogger.IsFatalEnabled)
				{
					genericLogger.Fatal("@@ IsFatalEnabled = TRUE.");
				}
				if (genericLogger.IsDebugEnabled)
				{
					genericLogger.Debug("@@ IsDebugEnabled = TRUE.");
				}
				if (genericLogger.IsErrorEnabled)
				{
					genericLogger.Error("@@ IsErrorEnabled = TRUE.");
				}
			}
			catch (Exception ex)
			{
				if (genericLogger.IsErrorEnabled)
				{
					genericLogger.Error(" @@LogError: " + ex.StackTrace.ToString() + "\n");
				}
			}
		}
	}
}
