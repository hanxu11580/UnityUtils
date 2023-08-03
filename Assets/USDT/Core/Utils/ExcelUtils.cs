using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace USDT.Utils {

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

        public static List<string> GetSheetRow(ISheet sheet, int row) {
            IRow headerRow = sheet.GetRow(row);
            var fieldNames = new List<string>();
            for (int i = 0; i < headerRow.LastCellNum; i++) {
                var cell = headerRow.GetCell(i);
                if (cell == null || cell.CellType == CellType.Blank) break;
                cell.SetCellType(CellType.String);
                fieldNames.Add(cell.StringCellValue);
            }
            return fieldNames;
        }
    }
}