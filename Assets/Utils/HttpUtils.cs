﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;

namespace Utility {
    public static class HttpUtils
    {
        public static HttpWebResponse CreateHttpWebResponse(string url, string postData)
        {
            var sendBytes = Encoding.UTF8.GetBytes(postData);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = sendBytes.Length;
            using (Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(sendBytes, 0, sendBytes.Length);
            }
            return (HttpWebResponse)webRequest.GetResponse();
        }

        // 模拟curl -u 
        public static Task<HttpResponseMessage> HttpCurl_U(string url, string acc, string pwd)
        {
            HttpClient client = new HttpClient();
            string str = $"{acc}:{pwd}";
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            string base64Str = Convert.ToBase64String(bytes);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Str);
            return client.GetAsync(url);
        }

        /// <summary>
        /// 发送文件，字符串示例
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <param name="savefileName"></param>
        /// <param name="savefilePath"></param>
        public static void Upload(string url ,string file, string savefileName, string savefilePath) {
            if (!File.Exists(file)) {
                //Log($"[HttpUtils.Upload] {DateTime.Now} upload failed {file} is not exists");
                return;
            }
            Stream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(savefileName), "file_name");
            formData.Add(new StringContent(savefilePath), "file_path");
            formData.Add(new StreamContent(fileStream), savefileName, savefileName);

            HttpClient client = new HttpClient();
            HttpResponseMessage result = client.PostAsync(url, formData).Result;
            string responseResult = result.Content.ReadAsStringAsync().Result;
            // 打印结果
            //Log($"[ValidatorUtils.Upload] responseResult:{responseResult}");
            client.Dispose();
            fileStream.Close();
        }

    }
}
