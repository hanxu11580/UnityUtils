using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using USDT.Utils;

namespace Bujuexiao.Editor {

    public struct BJXSaleExcel {
        public string date;
        public string project;
        public string employee;
        public float income;
        public float commission;
        public string platform;
        public bool isJiaZhong;
    }

    public class BJXSaleCommission {
        public string employee;
        public float commission;
    }

    public class BJXSaleExcelProcessing {

        private string _path;
        private List<BJXSaleExcel> _bJXSaleExcels;
        private Dictionary<string, BJXSaleCommission> _bJXSaleCommissionsLut;

        public BJXSaleExcelProcessing(string path) {
            _path = path;
            _bJXSaleExcels = new List<BJXSaleExcel>();
            _bJXSaleCommissionsLut = new Dictionary<string, BJXSaleCommission>();
        }

        public void Processing() {
            var book = ExcelUtils.LoadBook(_path);
            if(book != null) {
                var sheet = book.GetSheetAt(0);
                if(sheet != null) {
                    LogUtils.Log($"销售总数 : {sheet.LastRowNum}");
                    for (int i = 1; i <= sheet.LastRowNum; i++) {
                        var rowls = ExcelUtils.GetSheetRow(sheet, i);
                        if (rowls != null) {
                            BJXSaleExcel bJXSaleExcel = new BJXSaleExcel();
                            bJXSaleExcel.date = rowls[0];
                            bJXSaleExcel.project = rowls[1];
                            bJXSaleExcel.employee = rowls[2];
                            bJXSaleExcel.income = float.Parse(rowls[3]);
                            bJXSaleExcel.commission = float.Parse(rowls[4]);
                            bJXSaleExcel.platform = rowls[5];
                            if(rowls.Count < 7) {
                                bJXSaleExcel.isJiaZhong = false;
                            }
                            else {
                                bJXSaleExcel.isJiaZhong = rowls[6] == "1";
                            }
                            _bJXSaleExcels.Add(bJXSaleExcel);
                        }
                    }
                }
            }
        }

        public void CalcCommissions() {
            if(_bJXSaleExcels != null) {
                foreach (var item in _bJXSaleExcels) {
                    if (_bJXSaleCommissionsLut.TryGetValue(item.employee, out BJXSaleCommission bJXSaleCommission)) {
                        bJXSaleCommission.commission += item.commission;
                    }
                    else {
                        var newKV = new BJXSaleCommission() {
                            employee = item.employee,
                            commission = item.commission
                        };
                        _bJXSaleCommissionsLut.Add(item.employee, newKV);
                    }
                }

                foreach (var kv in _bJXSaleCommissionsLut) {
                    LogUtils.Log($"{kv.Key} : {kv.Value.commission}");
                }
            }
        }

        public void CalcTotalIncome() {
            if (_bJXSaleExcels != null) {
                float calcTotalIncome = 0;
                foreach (var item in _bJXSaleExcels) {
                    calcTotalIncome += item.income;
                }

                LogUtils.Log($"总收入 : {calcTotalIncome}");
            }
        }
    }
}