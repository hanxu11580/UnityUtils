using System;
using System.Data;
using System.IO;
using System.Text;

namespace Utility {
    public class CsvUtils {
        /// <summary>
        /// д��CSV
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <param name="dt">Ҫд���datatable</param>
        public static void WriteCSV(string fileName, DataTable dt) {
            FileStream fs;
            StreamWriter sw;
            string data = null;

            //�ж��ļ��Ƿ����,���ھͲ��ٴ�д������
            if (!File.Exists(fileName)) {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.UTF8);

                //д��������
                for (int i = 0; i < dt.Columns.Count; i++) {
                    data += dt.Columns[i].ColumnName.ToString();
                    if (i < dt.Columns.Count - 1) {
                        data += ",";//�м��ã�����
                    }
                }
                sw.WriteLine(data);
            }
            else {
                fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.UTF8);
            }

            //д����������
            for (int i = 0; i < dt.Rows.Count; i++) {
                data = null;
                for (int j = 0; j < dt.Columns.Count; j++) {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1) {
                        data += ",";//�м��ã�����
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }



        /// <summary>
        /// ��ȡCSV�ļ�
        /// </summary>
        /// <param name="fileName">�ļ�·��</param>
        public static DataTable ReadCSV(string fileName) {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);

            //��¼ÿ�ζ�ȡ��һ�м�¼
            string strLine = null;
            //��¼ÿ�м�¼�еĸ��ֶ�����
            string[] arrayLine = null;
            //�ָ���
            string[] separators = { "," };
            //�жϣ����ǵ�һ�Σ�������ͷ
            bool isFirst = true;

            //���ж�ȡCSV�ļ�
            while ((strLine = sr.ReadLine()) != null) {
                strLine = strLine.Trim();//ȥ��ͷβ�ո�
                arrayLine = strLine.Split(separators, StringSplitOptions.RemoveEmptyEntries);//�ָ��ַ�������������
                int dtColumns = arrayLine.Length;//�еĸ���

                if (isFirst)  //������ͷ
                {
                    for (int i = 0; i < dtColumns; i++) {
                        dt.Columns.Add(arrayLine[i]);//ÿһ������
                    }
                }
                else   //������
                {
                    DataRow dataRow = dt.NewRow();//�½�һ��
                    for (int j = 0; j < dtColumns; j++) {
                        dataRow[j] = arrayLine[j];
                    }
                    dt.Rows.Add(dataRow);//���һ��
                }
            }
            sr.Close();
            fs.Close();

            return dt;
        }
    }
}