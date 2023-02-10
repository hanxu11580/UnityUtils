using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utility {

    public static class ExcelUtils {
        public static IWorkbook LoadBook(string excelPath) {
            if (!File.Exists(excelPath)) {
                return new XSSFWorkbook();
            }
            else {
                using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    if (Path.GetExtension(excelPath) == ".xls") {
                        return new HSSFWorkbook(stream);
                    }
                    else {
                        return new XSSFWorkbook(stream);
                    }
                }
            }
        }
    }
}