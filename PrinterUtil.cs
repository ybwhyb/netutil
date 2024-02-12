using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace util
{
    class PrinterUtil
    {

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        /// <summary>
        /// Send byted to printer;
        /// </summary>
        /// <param name="szPrinterName"></param>
        /// <param name="pBytes"></param>
        /// <param name="dwCount"></param>
        /// <returns></returns>
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;
            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // # 프린터 열기
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // # Document 시작
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }

            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="szPrinterName">프린터 이름</param>
        /// <param name="szFileName">파일 string</param>
        /// <returns></returns>
        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;

            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            bytes = br.ReadBytes(nLength);
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="szPrinterName"></param>
        /// <param name="szString"></param>
        /// <returns></returns>
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;

            dwCount = (szString.Length + 1) * Marshal.SystemMaxDBCSCharSize;

            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }


    }
}
