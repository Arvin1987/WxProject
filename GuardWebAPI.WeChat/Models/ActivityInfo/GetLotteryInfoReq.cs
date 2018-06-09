using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Models.ActivityInfo
{
    public class GetLotteryInfoReq
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string Uid { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadUrl { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 帮助者uid
        /// </summary>
        public string HelperUid { get; set; }
        /// <summary>
        /// 帮助者头像
        /// </summary>
        public string HelperHeadUrl { get; set; }
        /// <summary>
        /// 帮助者昵称
        /// </summary>
        public string HelperNickName { get; set; }
        /// <summary>
        /// 宝箱index
        /// </summary>
        public int BoxIndex { get; set; }
    }
}
