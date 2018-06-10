using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GuardWebAPI.WeChat.Data;
using GuardWebAPI.WeChat.Domain;
using GuardWebAPI.WeChat.JSSDK.Models;
using GuardWebAPI.WeChat.Models.ActivityInfo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GuardWebAPI.WeChat.Services.ActivityInfo
{
    public class ActivityInfoService
    {
        public GetLotteryInfoResq GetLotteryInfo(GetLotteryInfoReq para)
        {
            ActivityInfoProcessing activityInfoProcessing = new ActivityInfoProcessing();

            if (!activityInfoProcessing.ExistUId(para.Uid))
            {
                activityInfoProcessing.InsertUserInfo(para.Uid, para.HeadUrl, para.NickName);
            }

            GetLotteryInfoResq getLotteryInfoResq = new GetLotteryInfoResq();
            getLotteryInfoResq.AvailableCount = 2 - activityInfoProcessing.GetTodayLotteryCount(para);
            getLotteryInfoResq.userInfo = activityInfoProcessing.GetUserInfoModel(para);
            getLotteryInfoResq.userBoxIndex = activityInfoProcessing.GetUserBoxInfos(para).Select(m => m.F_BoxIndex).ToArray();

            List<UserLotteryCount> userLotteryDetails = activityInfoProcessing.GetUserLotteryCount(para);
            List<UserLotteryHelp> userLotteryHelps = activityInfoProcessing.GetUserLotteryHelp(para);

            List<UserLotteryDetail> details = new List<UserLotteryDetail>();
            foreach (UserLotteryCount item in userLotteryDetails)
            {
                var userHelp = userLotteryHelps.Where(m => m.F_LotteryIndex == item.F_LotteryIndex).OrderByDescending(m => m.F_CreateTime).FirstOrDefault();
                if (userHelp != null)
                {
                    UserLotteryDetail detail = new UserLotteryDetail();
                    detail.F_LotteryCount = item.F_IndexCount;
                    detail.F_LotteryIndex = item.F_LotteryIndex;
                    detail.F_UId = item.F_UId;
                    detail.F_CreateUId = userHelp.F_CreateUId;
                    detail.F_CreateNickName = userHelp.F_CreateNickName;
                    detail.F_CreateHeadUrl = userHelp.F_CreateHeadUrl;
                    details.Add(detail);
                }
            }
            getLotteryInfoResq.userLottery = details;
            return getLotteryInfoResq;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public UserInfoModel GetUserInfo(GetLotteryInfoReq para)
        {
            ActivityInfoProcessing activityInfoProcessing = new ActivityInfoProcessing();
            return activityInfoProcessing.GetUserInfoModel(para);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public bool GetBox(GetLotteryInfoReq para)
        {
            ActivityInfoProcessing activityInfoProcessing = new ActivityInfoProcessing();
            int count = activityInfoProcessing.GetUserLotteryTypeCount(para);
            bool result = false;
            switch (para.BoxIndex)
            {
                case 0:
                    result = (count >= 1);
                    break;
                case 1:
                    result = (count >= 3);
                    break;
                case 2:
                    result = (count >= 5);
                    break;
                case 3:
                    result = (count == 6);
                    break;
            }

            if (result)
            {
                activityInfoProcessing.InserUserBox(para);
            }

            return result;
        }

        public Tuple<bool, int[], GetLotteryInfoResq> HelpGetLottery(GetLotteryInfoReq req)
        {
            ActivityInfoProcessing activityInfoProcessing = new ActivityInfoProcessing();
            if (activityInfoProcessing.GetHelperCount(req) >= 1)
            {
                return new Tuple<bool, int[], GetLotteryInfoResq>(false, new int[] { -1 }, null);
            }

            if (!activityInfoProcessing.ExistUId(req.HelperUid))
            {
                activityInfoProcessing.InsertUserInfo(req.HelperUid, req.HelperHeadUrl, req.HelperNickName);
            }

            int rondom = GetRondomLottery();
            activityInfoProcessing.InserLotteryInfo(req, rondom);
            return new Tuple<bool, int[], GetLotteryInfoResq>(true, new int[] { rondom }, GetLotteryInfo(req));

        }

        public int GetTodayCount(GetLotteryInfoReq req)
        {
            ActivityInfoProcessing activityInfoProcessing = new ActivityInfoProcessing();
            return activityInfoProcessing.GetTodayLotteryCount(req);
        }

        public Tuple<int[], GetLotteryInfoResq> GetLottery(GetLotteryInfoReq para)
        {
            ActivityInfoProcessing activityInfoProcessing = new ActivityInfoProcessing();
            int todayCount = activityInfoProcessing.GetTodayLotteryCount(para);

            if (todayCount == 0)
            {
                int rondom = LotteryProbability.GetRondom();
                activityInfoProcessing.InserLotteryInfo(para, rondom);
                return new Tuple<int[], GetLotteryInfoResq>(new int[] { rondom, rondom, rondom }, GetLotteryInfo(para));
            }
            else if (todayCount == 1)
            {
                int index0 = LotteryProbability.GetRondom();
                int index1 = LotteryProbability.GetRondom();
                int index2 = LotteryProbability.GetRondom();

                if (index0 == index1 && index1 == index2)
                {
                    activityInfoProcessing.InserLotteryInfo(para, index0);
                }
                else
                {
                    activityInfoProcessing.InserLotteryInfo(para, -1);
                }

                return new Tuple<int[], GetLotteryInfoResq>(new int[] { index0, index1, index2 }, GetLotteryInfo(para));
            }
            else  //两次机会都用完了
            {
                return new Tuple<int[], GetLotteryInfoResq>(new int[] { }, null);
            }
        }

        public int GetRondomLottery()
        {
            ActivityInfoProcessing activityInfoProcessing = new ActivityInfoProcessing();
            List<LotteryChanceModel> lotteryChanceModels = activityInfoProcessing.GetLotteryChance();
            int result = LotteryProbability.Get(lotteryChanceModels.Select(m => m.F_LotteryChance));
            return lotteryChanceModels[result].F_LotteryIndex;
        }

        internal WXUIdInfo GetUId(string code)
        {
            WebClient client = new WebClient();
            string response = client.DownloadString($"https://api.weixin.qq.com/sns/jscode2session?appid=wxf9c88a89b2b71833&secret=abe4fd5bf0999781b804b6a7e9e72636&js_code={ code }&grant_type=authorization_code");
            if (string.IsNullOrWhiteSpace(response))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<WXUIdInfo>(response);
        }

        public string GetWxCode(string scene, string page)
        {
            string result = string.Empty;
            result = "data:image/jpeg;base64,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";
            //WebClient client = new WebClient();
            //string tokenResult = client.DownloadString($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wx8bb9499f182375a6&secret=0d938f0722d3dca1d9631bb45b2e7b24");
            //if (JObject.Parse(tokenResult).TryGetValue("access_token", out JToken value))
            //{
            //    WxCodeInfo wxCodeInfo = new WxCodeInfo();
            //    wxCodeInfo.scene = scene;
            //    wxCodeInfo.page = page;

            //    HttpWebRequest httpRequest = WebRequest.CreateHttp($"https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token={ value.ToString() }");
            //    httpRequest.Method = "POST";
            //    httpRequest.ContentType = "application/json";
            //    byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(wxCodeInfo));
            //    httpRequest.ContentLength = buffer.LongLength;
            //    using (Stream stream = httpRequest.GetRequestStream())
            //    {
            //        stream.Write(buffer, 0, buffer.Length);
            //    }
            //    using (WebResponse response = httpRequest.GetResponse())
            //    using (Stream resStream = response.GetResponseStream())
            //    {
            //        byte[] bytes = new byte[response.ContentLength];
            //        int restult = resStream.Read(bytes, 0, bytes.Length);
            //        result = Convert.ToBase64String(bytes);
            //    }
            //}
            return result;
        }

        public H5UserInfo GetH5UserInfo(string code)
        {
            H5UserInfo h5UserInfo = null;
            WebClient client = new WebClient();

            string tokenResult = client.DownloadString($"https://mpx.wetalk.im/sns/oauth2/access_token?appid=wxc1a7dbfa678d92ce&secret=rmgMs5fnuoRBJ3YyZexNV2w00huW0M&code={ code }&grant_type=authorization_code");

            if (!string.IsNullOrEmpty(tokenResult))
            {
                H5Token token = JsonConvert.DeserializeObject<H5Token>(tokenResult);
                string h5UserInfoResult = client.DownloadString($"https://mpx.wetalk.im/sns/userinfo?access_token={ token.access_token }&openid={ token.openid }&lang=zh_CN");

                if (!string.IsNullOrEmpty(h5UserInfoResult))
                {
                    h5UserInfo = JsonConvert.DeserializeObject<H5UserInfo>(h5UserInfoResult);
                }
            }
            return h5UserInfo;
        }

        /// <summary>
        /// 获取h5票据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetH5Ticket(string code)
        {
            string ticke = string.Empty;
            WebClient client = new WebClient();

            string tokenResult = client.DownloadString($"https://mpx.wetalk.im/cgi-bin/token?grant_type=client_credential&appid=wxc1a7dbfa678d92ce&secret=rmgMs5fnuoRBJ3YyZexNV2w00huW0M");
            //string tokenResult = client.DownloadString($"https://mpx.wetalk.im/sns/oauth2/access_token?appid=wxc1a7dbfa678d92ce&secret=rmgMs5fnuoRBJ3YyZexNV2w00huW0M&code={ code }&grant_type=authorization_code");
            if (!string.IsNullOrEmpty(tokenResult))
            {
                H5Token token = JsonConvert.DeserializeObject<H5Token>(tokenResult);
                string h5UserInfoResult = client.DownloadString($"https://mpx.wetalk.im/cgi-bin/ticket/getticket?access_token={ token.access_token }&type=jsapi");
                if (JObject.Parse(h5UserInfoResult).TryGetValue("ticket", out JToken value))
                {
                    ticke = value.ToString();
                }
            }
            return ticke;
        }

        /// <summary>
        /// 获取h5签名
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        internal string Sign(string ticket, H5Sign config)
        {
            // 注意这里参数名必须全部小写，且必须有序
            string input = $"jsapi_ticket={ ticket }&noncestr={ config.NonceStr }&timestamp={ config.Timestamp }&url={config.url}";
            SHA1 sHA1 = SHA1.Create();
            byte[] buffer = sHA1.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(buffer).Replace("-", string.Empty).ToString().ToLower();
        }
    }
}
