using GuardWebAPI.WeChat.Models;
using System;

namespace GuardWebAPI.WeChat.JSSDK.Models
{
    public class JSSDK_ConfigResponse : ResultCore
    {
        public JSSDK_ConfigResponse()
        {
            NonceStr = CreateNonceStr();
            Timestamp = CreateTimeStamp();
        }

        public string AppId { get; set; }
        public long Timestamp { get; }
        public string NonceStr { get; set; }
        public string Signature { get; set; }

        private static string CreateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 15);
        }

        private static long CreateTimeStamp()
        {
            return (long)(DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds;
        }
    }
}