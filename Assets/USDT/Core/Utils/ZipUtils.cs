using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Diagnostics;
using System.IO;



namespace USDT.Utils {
    public static class ZipUtils {
        public static byte[] SharpZipLibCompress(byte[] data) {
            MemoryStream compressed = new MemoryStream();
            DeflaterOutputStream outputStream = new DeflaterOutputStream(compressed);
            outputStream.Write(data, 0, data.Length);
            outputStream.Close();
            return compressed.ToArray();
        }

        public static byte[] SharpZipLibDecompress(byte[] data) {
            MemoryStream compressed = new MemoryStream(data);
            MemoryStream decompressed = new MemoryStream();
            InflaterInputStream inputStream = new InflaterInputStream(compressed);
            inputStream.CopyTo(decompressed);
            return decompressed.ToArray();
        }

        #region 文件/文件夹压缩

        #region 代码
        /// <summary>
        /// 压缩文件和文件夹
        /// </summary>
        /// <param name="_fileOrDirectoryArray">文件夹路径和文件名</param>
        /// <param name="_outputPathName">压缩后的输出路径文件名</param>
        /// <param name="_password">压缩密码</param>
        /// <param name="_zipCallback">ZipCallback对象，负责回调</param>
        /// <param name="level">0速度最快</param>
        /// <returns></returns>
        public static bool Zip(string[] _fileOrDirectoryArray, string _outputPathName, int level = 6, string _password = null, ZipCallback _zipCallback = null) {
            if ((null == _fileOrDirectoryArray) || string.IsNullOrEmpty(_outputPathName)) {
                if (null != _zipCallback)
                    _zipCallback.OnFinished(false);

                return false;
            }

            ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(_outputPathName));
            zipOutputStream.SetLevel(level);
            if (!string.IsNullOrEmpty(_password))
                zipOutputStream.Password = _password;

            for (int index = 0; index < _fileOrDirectoryArray.Length; ++index) {
                bool result = false;
                string fileOrDirectory = _fileOrDirectoryArray[index];
                if (Directory.Exists(fileOrDirectory))
                    result = _ZipDirectory(fileOrDirectory, string.Empty, zipOutputStream, _zipCallback);
                else if (File.Exists(fileOrDirectory))
                    result = _ZipFile(fileOrDirectory, string.Empty, zipOutputStream, _zipCallback);

                if (!result) {
                    if (null != _zipCallback)
                        _zipCallback.OnFinished(false);

                    return false;
                }
            }

            zipOutputStream.Finish();
            zipOutputStream.Close();

            if (null != _zipCallback)
                _zipCallback.OnFinished(true);

            return true;
        }

        /// <summary>
        /// 解压Zip包
        /// </summary>
        /// <param name="_filePathName">Zip包的文件路径名</param>
        /// <param name="_outputPath">解压输出路径</param>
        /// <param name="_password">解压密码</param>
        /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
        /// <returns></returns>
        public static bool UnzipFile(string _filePathName, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null) {
            if (string.IsNullOrEmpty(_filePathName) || string.IsNullOrEmpty(_outputPath)) {
                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);

                return false;
            }

            try {
                return UnzipFile(File.OpenRead(_filePathName), _outputPath, _password, _unzipCallback);
            }
            catch (System.Exception _e) {
                lg.e("[ZipUtility.UnzipFile]: " + _e.ToString());

                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);

                return false;
            }
        }

        /// <summary>
        /// 解压Zip包
        /// </summary>
        /// <param name="_fileBytes">Zip包字节数组</param>
        /// <param name="_outputPath">解压输出路径</param>
        /// <param name="_password">解压密码</param>
        /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
        /// <returns></returns>
        public static bool UnzipFile(byte[] _fileBytes, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null) {
            if ((null == _fileBytes) || string.IsNullOrEmpty(_outputPath)) {
                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);

                return false;
            }

            bool result = UnzipFile(new MemoryStream(_fileBytes), _outputPath, _password, _unzipCallback);
            if (!result) {
                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);
            }

            return result;
        }

        /// <summary>
        /// 解压Zip包
        /// </summary>
        /// <param name="_inputStream">Zip包输入流</param>
        /// <param name="_outputPath">解压输出路径</param>
        /// <param name="_password">解压密码</param>
        /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
        /// <returns></returns>
        public static bool UnzipFile(Stream _inputStream, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null) {
            if ((null == _inputStream) || string.IsNullOrEmpty(_outputPath)) {
                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);

                return false;
            }

            // 创建文件目录
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);

            // 解压Zip包
            ZipEntry entry = null;
            using (ZipInputStream zipInputStream = new ZipInputStream(_inputStream)) {
                if (!string.IsNullOrEmpty(_password))
                    zipInputStream.Password = _password;

                while (null != (entry = zipInputStream.GetNextEntry())) {
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    if ((null != _unzipCallback) && !_unzipCallback.OnPreUnzip(entry))
                        continue;   // 过滤

                    string filePathName = Path.Combine(_outputPath, entry.Name);

                    // 创建文件目录
                    if (entry.IsDirectory) {
                        Directory.CreateDirectory(filePathName);
                        continue;
                    }

                    // 写入文件
                    try {
                        using (FileStream fileStream = File.Create(filePathName)) {
                            byte[] bytes = new byte[1024];
                            while (true) {
                                int count = zipInputStream.Read(bytes, 0, bytes.Length);
                                if (count > 0)
                                    fileStream.Write(bytes, 0, count);
                                else {
                                    if (null != _unzipCallback)
                                        _unzipCallback.OnPostUnzip(entry);

                                    break;
                                }
                            }
                        }
                    }
                    catch (System.Exception _e) {
                        lg.e("[ZipUtility.UnzipFile]: " + _e.ToString());

                        if (null != _unzipCallback)
                            _unzipCallback.OnFinished(false);

                        return false;
                    }
                }
            }

            if (null != _unzipCallback)
                _unzipCallback.OnFinished(true);

            return true;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="_filePathName">文件路径名</param>
        /// <param name="_parentRelPath">要压缩的文件的父相对文件夹</param>
        /// <param name="_zipOutputStream">压缩输出流</param>
        /// <param name="_zipCallback">ZipCallback对象，负责回调</param>
        /// <returns></returns>
        private static bool _ZipFile(string _filePathName, string _parentRelPath, ZipOutputStream _zipOutputStream, ZipCallback _zipCallback = null) {

            //Crc32 crc32 = new Crc32();
            ZipEntry entry = null;
            FileStream fileStream = null;
            try {
                string entryName = _parentRelPath + '/' + Path.GetFileName(_filePathName);
                entry = new ZipEntry(entryName);
                entry.DateTime = System.DateTime.Now;

                if ((null != _zipCallback) && !_zipCallback.OnPreZip(entry))
                    return true;    // 过滤

                fileStream = File.OpenRead(_filePathName);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();

                entry.Size = buffer.Length;

                //crc32.Reset();
                //crc32.Update(buffer);
                //entry.Crc = crc32.Value;

                _zipOutputStream.PutNextEntry(entry);
                _zipOutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (System.Exception _e) {
                lg.e("[ZipUtility.ZipFile]: " + _e.ToString());
                return false;
            }
            finally {
                if (null != fileStream) {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }

            if (null != _zipCallback)
                _zipCallback.OnPostZip(entry);

            return true;
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="_path">要压缩的文件夹</param>
        /// <param name="_parentRelPath">要压缩的文件夹的父相对文件夹</param>
        /// <param name="_zipOutputStream">压缩输出流</param>
        /// <param name="_zipCallback">ZipCallback对象，负责回调</param>
        /// <returns></returns>
        private static bool _ZipDirectory(string _path, string _parentRelPath, ZipOutputStream _zipOutputStream, ZipCallback _zipCallback = null) {
            ZipEntry entry = null;
            try {
                string entryName = Path.Combine(_parentRelPath, Path.GetFileName(_path) + '/');
                entry = new ZipEntry(entryName);
                entry.DateTime = System.DateTime.Now;
                entry.Size = 0;

                if ((null != _zipCallback) && !_zipCallback.OnPreZip(entry))
                    return true;    // 过滤

                _zipOutputStream.PutNextEntry(entry);
                _zipOutputStream.Flush();

                string[] files = Directory.GetFiles(_path);
                for (int index = 0; index < files.Length; ++index) {
                    // 排除Unity中可能的 .meta 文件
                    if (files[index].EndsWith(".meta") == true) {
                        lg.i(files[index] + " not to zip");
                        continue;
                    }

                    _ZipFile(files[index], Path.Combine(_parentRelPath, Path.GetFileName(_path)), _zipOutputStream, _zipCallback);
                }
            }
            catch (System.Exception _e) {
                lg.e("[ZipUtility.ZipDirectory]: " + _e.ToString());
                return false;
            }

            string[] directories = Directory.GetDirectories(_path);
            for (int index = 0; index < directories.Length; ++index) {
                if (!_ZipDirectory(directories[index], Path.Combine(_parentRelPath, Path.GetFileName(_path)), _zipOutputStream, _zipCallback)) {
                    return false;
                }
            }

            if (null != _zipCallback)
                _zipCallback.OnPostZip(entry);

            return true;
        }

        #endregion

        #region ZipCallback
        public abstract class ZipCallback {
            /// <summary>
            /// 压缩单个文件或文件夹前执行的回调
            /// </summary>
            /// <param name="_entry"></param>
            /// <returns>如果返回true，则压缩文件或文件夹，反之则不压缩文件或文件夹</returns>
            public virtual bool OnPreZip(ZipEntry _entry) {
                return true;
            }

            /// <summary>
            /// 压缩单个文件或文件夹后执行的回调
            /// </summary>
            /// <param name="_entry"></param>
            public virtual void OnPostZip(ZipEntry _entry) { }

            /// <summary>
            /// 压缩执行完毕后的回调
            /// </summary>
            /// <param name="_result">true表示压缩成功，false表示压缩失败</param>
            public virtual void OnFinished(bool _result) { }
        }
        #endregion

        #region UnzipCallback
        public abstract class UnzipCallback {
            /// <summary>
            /// 解压单个文件或文件夹前执行的回调
            /// </summary>
            /// <param name="_entry"></param>
            /// <returns>如果返回true，则压缩文件或文件夹，反之则不压缩文件或文件夹</returns>
            public virtual bool OnPreUnzip(ZipEntry _entry) {
                return true;
            }

            /// <summary>
            /// 解压单个文件或文件夹后执行的回调
            /// </summary>
            /// <param name="_entry"></param>
            public virtual void OnPostUnzip(ZipEntry _entry) { }

            /// <summary>
            /// 解压执行完毕后的回调
            /// </summary>
            /// <param name="_result">true表示解压成功，false表示解压失败</param>
            public virtual void OnFinished(bool _result) { }
        }
        #endregion

        public static void ZipTest_1() {
            Stopwatch sw = new Stopwatch();
            var zipPath = @"C:\Users\A\Desktop\Bundles";
            var unzipPath = @"‪C:\Users\A\Desktop\BundlesZip.zip";
            //// 55100.21ms
            //// 315.3 MB
            //var unzipPath_1 = @"C:\Users\A\Desktop\BundlesZip_1.zip";
            //sw.Start();
            //Zip(new string[] { zipPath }, unzipPath_1);
            //sw.Stop();
            //lg.i($"压缩时间: {((float)sw.ElapsedTicks / Stopwatch.Frequency) * 1000}");
            //var zipSize_1 = FileUtils.GetFileSize(unzipPath_1);
            //lg.i(UnityEditor.EditorUtility.FormatBytes(zipSize_1));

            //// 解压时间: 19831.79
            //sw.Restart();
            //var unzipPath_2 = @"C:\Users\A\Desktop\BundlesUnZip_1";
            //UnzipFile(unzipPath_1, unzipPath_2);
            //sw.Stop();
            //lg.i($"解压时间: {((float)sw.ElapsedTicks / Stopwatch.Frequency) * 1000}");

            //拆开压缩 大小和上面没区别， 
            //var unzipPath_2 = @"C:\Users\A\Desktop\BundlesZip_2.zip";
            //sw.Restart();
            //Zip(Directory.GetFiles(zipPath), unzipPath_2);
            //sw.Stop();
            //lg.i($"压缩时间: {((float)sw.ElapsedTicks / Stopwatch.Frequency) * 1000}");
            //var zipSize_2 = FileUtils.GetFileSize(unzipPath_2);
            //lg.i(EditorUtility.FormatBytes(zipSize_2));

            // 8112.77ms
            // 362.1 MB
            //sw.Restart();
            //var unzipPath_2 = @"C:\Users\A\Desktop\BundlesZip_2.zip";
            //Zip(new string[] { zipPath }, unzipPath_2, 0);
            //sw.Stop();
            //lg.i($"压缩时间: {((float)sw.ElapsedTicks / Stopwatch.Frequency) * 1000}");
            //var zipSize_2 = FileUtils.GetFileSize(unzipPath_2);
            //lg.i(EditorUtility.FormatBytes(zipSize_2));

            //  55480.32ms
            // 315.2 MB
            //sw.Restart();
            //var unzipPath_2 = @"C:\Users\A\Desktop\BundlesZip_2.zip";
            //Zip(new string[] { zipPath }, unzipPath_2, 9);
            //sw.Stop();
            //lg.i($"压缩时间: {((float)sw.ElapsedTicks / Stopwatch.Frequency) * 1000}");
            //var zipSize_2 = FileUtils.GetFileSize(unzipPath_2);
            //lg.i(EditorUtility.FormatBytes(zipSize_2));

            sw.Restart();
            var output = @"C:\Users\A\Desktop\BundlesUnZipOutput";
            FastZip fastZip = new FastZip();
            fastZip.CreateZip(zipPath, unzipPath, true, null);
            sw.Stop();
            lg.i($"压缩时间: {((float)sw.ElapsedTicks / Stopwatch.Frequency) * 1000}");
            sw.Restart();
            fastZip.ExtractZip(unzipPath, output, null);
            lg.i($"解压时间: {((float)sw.ElapsedTicks / Stopwatch.Frequency) * 1000}");
        }

        #endregion

    }
}