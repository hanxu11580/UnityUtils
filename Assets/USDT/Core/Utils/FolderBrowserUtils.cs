using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;


namespace USDT.Utils {

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenDialogFile {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenDialogDir {
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr pidlRoot = IntPtr.Zero;
        public String pszDisplayName = null;
        public String lpszTitle = null;
        public UInt32 ulFlags = 0;
        public IntPtr lpfn = IntPtr.Zero;
        public IntPtr lParam = IntPtr.Zero;
        public int iImage = 0;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    public static class FolderBrowserUtils {

        //// ���ļ�Ŀ¼
        //FolderBrowserUtils.OpenFolder(@"E:\temp");

        //// ѡ��JPG/PNGͼƬ�ļ�������ѡ���ļ�·��
        //FolderBrowserUtils.SelectFile(_ => Debug.Log(_), FolderBrowserUtils.IMAGEFILTER);

        //// ѡ���ļ�Ŀ¼������ѡ���ļ���·��
        //Debug.Log(FolderBrowserUtils.GetPathFromWindowsExplorer());


        #region Window

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName([In, Out] OpenDialogFile ofn);

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool GetSaveFileName([In, Out] OpenDialogFile ofn);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

        #endregion

        public const string IMAGEFILTER = "ͼƬ�ļ�(*.jpg;*.png)\0*.jpg;*.png";
        public const string ALLFILTER = "�����ļ�(*.*)\0*.*";

        /// <summary>
        /// ѡ���ļ�
        /// </summary>
        /// <param name="callback">����ѡ���ļ��е�·��</param>
        /// <param name="filter">�ļ�����ɸѡ��</param>
        public static void SelectFile(Action<string> callback, string filter = ALLFILTER) {
            try {
                OpenFileName openFileName = new OpenFileName();
                openFileName.structSize = Marshal.SizeOf(openFileName);
                openFileName.filter = filter;
                openFileName.file = new string(new char[256]);
                openFileName.maxFile = openFileName.file.Length;
                openFileName.fileTitle = new string(new char[64]);
                openFileName.maxFileTitle = openFileName.fileTitle.Length;
                openFileName.title = "ѡ���ļ�";
                openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
                if (GetSaveFileName(openFileName)) {
                    string filepath = openFileName.file; //ѡ����ļ�·��;  
                    if (File.Exists(filepath)) {
                        if (callback != null)
                            callback(filepath);
                        return;
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            if (callback != null)
                callback(string.Empty);
        }

        /// <summary>
        /// ����WindowsExploer ��������ѡ�ļ���·��
        /// </summary>
        /// <param name="dialogtitle">�򿪶Ի���ı���</param>
        /// <returns>��ѡ�ļ���·��</returns>
        public static string GetPathFromWindowsExplorer(string dialogtitle = "��ѡ������·��") {
            try {
                OpenDialogDir ofn2 = new OpenDialogDir();
                ofn2.pszDisplayName = new string(new char[2048]);
                ; // ���Ŀ¼·��������  
                ofn2.lpszTitle = dialogtitle; // ����  
                ofn2.ulFlags = 0x00000040; // �µ���ʽ,���༭��  
                IntPtr pidlPtr = SHBrowseForFolder(ofn2);

                char[] charArray = new char[2048];

                for (int i = 0; i < 2048; i++) {
                    charArray[i] = '\0';
                }

                SHGetPathFromIDList(pidlPtr, charArray);
                string res = new string(charArray);
                res = res.Substring(0, res.IndexOf('\0'));
                return res;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return string.Empty;
        }

        /// <summary>
        /// ��Ŀ¼
        /// </summary>
        /// <param name="path">��Ҫ�򿪵��ļ�Ŀ¼</param>
        public static void OpenFolder(string path) {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
    }

}