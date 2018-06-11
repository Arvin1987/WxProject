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

        public byte[] GetWxCode(string scene, string page)
        {
            byte[] bytes = null;
            WebClient client = new WebClient();
            string tokenResult = client.DownloadString($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wxf9c88a89b2b71833&secret=abe4fd5bf0999781b804b6a7e9e72636");
            if (JObject.Parse(tokenResult).TryGetValue("access_token", out JToken value))
            {
                string dataJson = "{\"scene\":\"" + scene + "\",\"page\":\"" + page + "\",\"width\":430,\"auto_color\":false,\"line_color\":{\"r\":\"0\",\"g\":\"0\",\"b\":\"0\"},\"is_hyaline\":false}";

                HttpWebRequest httpRequest = WebRequest.CreateHttp($"https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token={ value.ToString() }");
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                byte[] buffer = Encoding.UTF8.GetBytes(dataJson);
                //httpRequest.ContentLength = buffer.LongLength;
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse())
                using (Stream resStream = response.GetResponseStream())
                {
                    // //在文件名前面加上时间，以防重名  
                    // string imgName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg";
                    // //文件存储相对于当前应用目录的虚拟目录  
                    // string path = "/image/";
                    // //获取相对于应用的基目录,创建目录  
                    // string imgPath = System.AppDomain.CurrentDomain.BaseDirectory + path;     //通过此对象获取文件名  
                    // StringHelper.CreateDirectory(imgPath);
                    // System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath(path + imgName), tt);//讲byte[]存储为图片  
                    ////return "/image/" + imgName;

                    List<byte> listBytes = new List<byte>();
                    int temp = resStream.ReadByte();
                    while (temp != -1)
                    {
                        listBytes.Add((byte)temp);
                        temp = resStream.ReadByte();
                    }
                    bytes = listBytes.ToArray();
                }
            }
            return bytes;
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

            string tokenResult = client.DownloadString($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wx3283d85d64449029&secret=f6b15e0e2c8eac475e45e3bb61c349e8");

            Console.WriteLine("gettoken url:" + $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wx3283d85d64449029&secret=f6b15e0e2c8eac475e45e3bb61c349e8" + "response:" + tokenResult);

            if (!string.IsNullOrEmpty(tokenResult))
            {
                H5Token token = JsonConvert.DeserializeObject<H5Token>(tokenResult);
                string h5UserInfoResult = client.DownloadString($"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={ token.access_token }&type=jsapi");

                Console.WriteLine("getticket url:" + $"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={ token.access_token }&type=jsapi" + "response:" + h5UserInfoResult);

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
