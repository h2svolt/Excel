using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SqlServerTypes
{
	public class Utilities
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr LoadLibrary(string libname);

		public static void LoadNativeAssemblies(string rootApplicationPath)
		{
			string nativeBinaryPath = ((IntPtr.Size > 4) ? Path.Combine(rootApplicationPath, "SqlServerTypes\\x64\\") : Path.Combine(rootApplicationPath, "SqlServerTypes\\x86\\"));
			LoadNativeAssembly(nativeBinaryPath, "msvcr120.dll");
			LoadNativeAssembly(nativeBinaryPath, "SqlServerSpatial140.dll");
		}

		private static void LoadNativeAssembly(string nativeBinaryPath, string assemblyName)
		{
			string libname = Path.Combine(nativeBinaryPath, assemblyName);
			IntPtr intPtr = LoadLibrary(libname);
			if (intPtr == IntPtr.Zero)
			{
				throw new Exception($"Error loading {assemblyName} (ErrorCode: {Marshal.GetLastWin32Error()})");
			}
		}
	}
}
