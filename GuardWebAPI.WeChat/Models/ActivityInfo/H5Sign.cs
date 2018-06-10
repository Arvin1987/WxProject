using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Models.ActivityInfo
{
    public class H5Sign
    {
        public H5Sign()
        {
            NonceStr = CreateNonceStr();
            Timestamp = CreateTimeStamp();
        }
        public long Timestamp { get; }
        public string AppId { get; set; }
        public string url { get; set; }
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
