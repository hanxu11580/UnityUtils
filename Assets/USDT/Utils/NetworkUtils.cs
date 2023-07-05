using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace USDT.Utils {
    public static class NetworkUtils {
        public static string GetLocalIP() {
            try {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++) {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork) {
                        return IpEntry.AddressList[i].ToString();
                    }
                }

                return "";
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }

        /// <summary>
        /// 判断是否为有效URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsValiadURL(string url) {
            return !(url == null || url.Length == 0 || !url.Contains("http") || !url.Contains("https"));
        }

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

        public static bool CheckNetwork() {
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                //无网络
                return false;
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) {
                //流量
                return true;
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
                //wifi
                return true;
            }
            return false;
        }

        #region UnityWebRequest
        public static IEnumerator Upload(string url, string field, byte[] bytes, string name, string mime, Action<bool> complete = null, Action<string> error = null) {
            WWWForm form = new WWWForm();
            form.AddBinaryData(field, bytes, name, mime);
            url = Uri.EscapeUriString(url);
            UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
                LogUtils.Log(webRequest.error);
                error?.Invoke(webRequest.error);
            }
            else {
                complete?.Invoke(true);
            }
        }
        public static IEnumerator Download(string url, Action<byte[]> complete = null, Action<string> error = null) {
            url = Uri.EscapeUriString(url);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
                LogUtils.Log(webRequest.error);
                error?.Invoke(webRequest.error);
            }
            else {
                byte[] bytes = webRequest.downloadHandler.data;
                complete?.Invoke(bytes);
            }
        }
        public static IEnumerator Download(string url, Action<Texture2D> complete = null, Action<string> error = null) {
            url = Uri.EscapeUriString(url);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            DownloadHandlerTexture download = new DownloadHandlerTexture(true);
            webRequest.downloadHandler = download;
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
                LogUtils.Log(webRequest.error);
                error?.Invoke(webRequest.error);
            }
            else {
                complete?.Invoke(download.texture);
            }
        }
        public static IEnumerator Download(string url, Action<Texture2D, byte[]> complete = null, Action<string> error = null) {
            url = Uri.EscapeUriString(url);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            DownloadHandlerTexture download = new DownloadHandlerTexture(true);
            webRequest.downloadHandler = download;
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
                LogUtils.Log(webRequest.error);
                error?.Invoke(webRequest.error);
            }
            else {
                complete?.Invoke(download.texture, download.data);
            }
        }
        public static IEnumerator Download(string url, AudioType type, Action<AudioClip> complete = null, Action<string> error = null) {
            url = Uri.EscapeUriString(url);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            DownloadHandlerAudioClip download = new DownloadHandlerAudioClip(webRequest.url, type);
            webRequest.downloadHandler = download;
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
                LogUtils.Log(webRequest.error);
                error?.Invoke(webRequest.error);
            }
            else {
                complete?.Invoke(download.audioClip);
            }
        }
        public static IEnumerator Download(string url, AudioType type, Action<AudioClip, byte[]> complete = null, Action<string> error = null) {
            url = Uri.EscapeUriString(url);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            DownloadHandlerAudioClip download = new DownloadHandlerAudioClip(webRequest.url, type);
            webRequest.downloadHandler = download;
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
                LogUtils.Log(webRequest.error);
                error?.Invoke(webRequest.error);
            }
            else {
                complete?.Invoke(download.audioClip, download.data);
            }
        }
        public static IEnumerator Download(string url, Action<AssetBundle> complete = null, Action<string> error = null) {
            url = Uri.EscapeUriString(url);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            DownloadHandlerAssetBundle download = new DownloadHandlerAssetBundle(webRequest.url, uint.MaxValue);
            webRequest.downloadHandler = download;
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
                LogUtils.Log(webRequest.error);
                error?.Invoke(webRequest.error);
            }
            else {
                complete?.Invoke(download.assetBundle);
            }
        }
        public static IEnumerator Download(string url, Action<AssetBundle, byte[]> complete = null, Action<string> error = null) {
            url = Uri.EscapeUriString(url);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            DownloadHandlerAssetBundle download = new DownloadHandlerAssetBundle(webRequest.url, uint.MaxValue);
            webRequest.downloadHandler = download;
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
                LogUtils.Log(webRequest.error);
                error?.Invoke(webRequest.error);
            }
            else {
                complete?.Invoke(download.assetBundle, download.data);
            }
        }

        #endregion

    }
}
