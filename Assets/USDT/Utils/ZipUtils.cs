using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;


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
    }
}