using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Models.ActivityInfo
{
    public class GetLotteryInfoResq
    {
        public int[] lotteryIndex { get; set; } = { };
        public UserInfoModel userInfo { get; set; }
        public int AvailableCount { get; set; }
        public List<UserLotteryDetail> userLottery { get; set; }
        public int[] userBoxIndex { get; set; } = { };
    }

    public class UserLotteryDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int F_LotteryIndex { get; set; }
        public int F_LotteryCount { get; set; }
        public string F_UId { get; set; }
        public string F_CreateUId { get; set; }
        public string F_CreateHeadUrl { get; set; }
        public string F_CreateNickName { get; set; }
    }

    public class UserLotteryCount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int F_LotteryIndex { get; set; }
        public int F_IndexCount { get; set; }
        public string F_UId { get; set; }
    }

    public class UserLotteryHelp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int F_Id { get; set; }
        public int F_LotteryIndex { get; set; }
        public string F_UId { get; set; }
        public string F_CreateUId { get; set; }
        public string F_CreateHeadUrl { get; set; }
        public string F_CreateNickName { get; set; }
        public DateTime F_CreateTime { get; set; }
    }
}
