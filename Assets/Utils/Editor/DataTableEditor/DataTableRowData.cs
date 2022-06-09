using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility.Editor {

    [Serializable]
    public class DataTableRowData
    {
        public List<string> Data { get; set; }

        public DataTableRowData()
        {
            Data = new List<string>();
        }

        public DataTableRowData(string[] data) {
            Data = new List<string>();
            Data = data.ToList();
        }
    }
}