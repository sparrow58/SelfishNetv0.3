using u003cCppImplementationDetailsu003e;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PcapNet
{
	public class Driver : IDisposable
	{
		public static unsafe /* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* driver_name;

		static Driver()
		{
			Driver.driver_name = (/* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte*)(&<Module>.??_C@_03GPOPMJNO@NPF?$AA@);
		}

		public Driver()
		{
		}

		private void ~Driver()
		{
		}

		public unsafe bool create()
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			if (!File.Exists(string.Concat(currentDirectory, "\\NPF.sys")))
			{
				MessageBox.Show("driver npf.sys not found !");
				return false;
			}
			IntPtr hGlobalAnsi = Marshal.StringToHGlobalAnsi(string.Concat(currentDirectory, "\\npf.sys"));
			/* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* pointer = (/* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte*)hGlobalAnsi.ToPointer();
			if (!Driver.RegisterDriver(Driver.driver_name, pointer))
			{
				MessageBox.Show("problem registering the driver");
				return false;
			}
			if (Driver.StartDriver(Driver.driver_name))
			{
				return true;
			}
			MessageBox.Show("problem starting the driver");
			return false;
		}

		protected virtual void Dispose(bool flag)
		{
			if (!flag)
			{
				this.Finalize();
			}
		}

		public sealed override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public unsafe void* openDeviceDriver(/* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* szDriverName)
		{
			$ArrayType$$$BY0MI@D _u0024ArrayTypeu0024u0024u0024BY0MIu0040D = new $ArrayType$$$BY0MI@D();
			<Module>.wsprintfA(ref _u0024ArrayTypeu0024u0024u0024BY0MIu0040D, ref <Module>.??_C@_06DIBEOEIP@?2?2?4?2?$CFs?$AA@, szDriverName);
			void* voidPointer = <Module>.CreateFileA(ref _u0024ArrayTypeu0024u0024u0024BY0MIu0040D, -1073741824, 0, 0, 3, 128, 0);
			if (voidPointer == null)
			{
				voidPointer = (void*)0;
			}
			return voidPointer;
		}

		public static unsafe bool RegisterDriver(/* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* szDriverName, /* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* szPathName)
		{
			SC_HANDLE__* sCHANDLE_Pointer = <Module>.OpenSCManagerA(0, 0, 983103);
			if (sCHANDLE_Pointer == null)
			{
				return false;
			}
			/* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* sByteu0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eIsSignUnspecifiedByteu0029u0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eIsConstu0029Pointer = szDriverName;
			SC_HANDLE__* sCHANDLE_Pointer1 = <Module>.CreateServiceA(sCHANDLE_Pointer, sByteu0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eIsSignUnspecifiedByteu0029u0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eIsConstu0029Pointer, sByteu0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eIsSignUnspecifiedByteu0029u0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eIsConstu0029Pointer, 983551, 1, 3, 1, szPathName, 0, 0, 0, 0, 0);
			if (sCHANDLE_Pointer1 != null)
			{
				<Module>.CloseServiceHandle(sCHANDLE_Pointer1);
				<Module>.CloseServiceHandle(sCHANDLE_Pointer);
				return true;
			}
			/* modopt(System.Runtime.CompilerServices.CallConvStdcall), modopt(System.Runtime.CompilerServices.IsLong) */ uint lastError = <Module>.GetLastError();
			<Module>.CloseServiceHandle(sCHANDLE_Pointer);
			return (byte)(lastError == null);
		}

		public static unsafe bool StartDriver(/* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* szDriverName)
		{
			SC_HANDLE__* sCHANDLE_Pointer = <Module>.OpenSCManagerA(0, 0, 983103);
			if (sCHANDLE_Pointer == null)
			{
				return false;
			}
			SC_HANDLE__* sCHANDLE_Pointer1 = <Module>.OpenServiceA(sCHANDLE_Pointer, szDriverName, 983551);
			if (sCHANDLE_Pointer1 == null)
			{
				<Module>.CloseServiceHandle(sCHANDLE_Pointer);
				return false;
			}
			int lastError = <Module>.StartServiceA(sCHANDLE_Pointer1, 0, 0);
			if (lastError == 0)
			{
				lastError = <Module>.GetLastError() == null;
			}
			<Module>.CloseServiceHandle(sCHANDLE_Pointer1);
			<Module>.CloseServiceHandle(sCHANDLE_Pointer);
			return (lastError != 0 ? 1 : 0);
		}

		public static unsafe bool StopDriver(/* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* szDriverName)
		{
			_SERVICE_STATUS _SERVICESTATU = new _SERVICE_STATUS();
			SC_HANDLE__* sCHANDLE_Pointer = <Module>.OpenSCManagerA(0, 0, 983103);
			if (sCHANDLE_Pointer == null)
			{
				return false;
			}
			SC_HANDLE__* sCHANDLE_Pointer1 = <Module>.OpenServiceA(sCHANDLE_Pointer, szDriverName, 983551);
			if (sCHANDLE_Pointer1 == null)
			{
				<Module>.CloseServiceHandle(sCHANDLE_Pointer);
				return false;
			}
			int num = <Module>.ControlService(sCHANDLE_Pointer1, 1, ref _SERVICESTATU);
			if (num == 0 && <Module>.GetLastError() == null)
			{
				num = 1;
			}
			<Module>.CloseServiceHandle(sCHANDLE_Pointer1);
			<Module>.CloseServiceHandle(sCHANDLE_Pointer);
			return (num != 0 ? 1 : 0);
		}

		public static unsafe bool UnregisterDriver(/* modopt(System.Runtime.CompilerServices.IsConst), modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* szDriverName)
		{
			SC_HANDLE__* sCHANDLE_Pointer = <Module>.OpenSCManagerA(0, 0, 983103);
			if (sCHANDLE_Pointer == null)
			{
				return false;
			}
			SC_HANDLE__* sCHANDLE_Pointer1 = <Module>.OpenServiceA(sCHANDLE_Pointer, szDriverName, 983551);
			if (sCHANDLE_Pointer1 == null)
			{
				<Module>.CloseServiceHandle(sCHANDLE_Pointer);
				return false;
			}
			/* modopt(System.Runtime.CompilerServices.CallConvStdcall) */ int int32u0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eCallConvStdcallu0029 = <Module>.DeleteService(sCHANDLE_Pointer1);
			<Module>.CloseServiceHandle(sCHANDLE_Pointer1);
			<Module>.CloseServiceHandle(sCHANDLE_Pointer);
			return (int32u0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eCallConvStdcallu0029 != 0 ? 1 : 0);
		}
	}
}