using GSBase.ConfigCenter;
using GuardWebAPI.WeChat.JSSDK.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Services.JSSDK
{
    public class JSSDKService
    {
        private IMemoryCache _memoryCache;

        public JSSDKService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<string> GetAccessTokenAsync(string appId, string secret)
        {
            if (!_memoryCache.TryGetValue("JSSDK_AccessToken_" + appId, out string accessToken))
            {
                WebClient client = new WebClient();
                string response = await client.DownloadStringTaskAsync($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={ appId }&secret={ secret }");
                if (JObject.Parse(response).TryGetValue("access_token", out JToken value))
                {
                    accessToken = value.ToString();
                    _memoryCache.Set("JSSDK_AccessToken_" + appId, accessToken, DateTimeOffset.Now.AddHours(1.5));
                }
            }

            return accessToken;
        }

        public async Task<string> GetTicketAsync(string accessToken)
        {
            WebClient client = new WebClient();
            string response = await client.DownloadStringTaskAsync($"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={ accessToken }&type=jsapi");
            if (JObject.Parse(response).TryGetValue("ticket", out JToken value))
            {
                return value.ToString();
            }

            return null;
        }

        public string Sign(string ticket, string url, JSSDK_ConfigResponse config)
        {
            // 注意这里参数名必须全部小写，且必须有序
            string input = $"jsapi_ticket={ ticket }&noncestr={ config.NonceStr }&timestamp={ config.Timestamp }&url={ url }";
            SHA1 sHA1 = SHA1.Create();
            byte[] buffer = sHA1.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(buffer).Replace("-", string.Empty).ToString().ToLower();
        }

        public async Task<string> ShortUrlAsync(string appId, string longUrl)
        {
            string secret = ConfigCenterClient.GetSection("WeChatAccount").GetValue<string>(appId, null);
            if (secret == null)
            {
                return null;
            }
            string accessToken = await GetAccessTokenAsync(appId, secret);

            HttpWebRequest httpRequest = WebRequest.CreateHttp($"https://api.weixin.qq.com/cgi-bin/shorturl?access_token={ accessToken }");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            byte[] buffer = Encoding.UTF8.GetBytes($"{{\"action\":\"long2short\",\"long_url\":\"{ longUrl }\"}}");
            httpRequest.ContentLength = buffer.LongLength;
            using (Stream stream = httpRequest.GetRequestStream())
            {
                stream.Write(buffer, 0, buffer.Length);
            }

            string shortUrl = null;
            using (WebResponse response = httpRequest.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                if (JObject.Parse(json).TryGetValue("short_url", out JToken value))
                {
                    shortUrl = value.ToString();
                }
            }

            return shortUrl;
        }
    }
}