using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Models.ActivityInfo
{
    [Table("T_UserLotteryInfo")]
    public class UserLotteryInfoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long F_Id { get; set; }
        public string F_UId { get; set; }
        public int F_LotteryIndex { get; set; }
        public string F_CreateUId { get; set; }
        public string F_CreateDate { get; set; }
        public DateTime F_CreateTime { get; set; }
        public string F_Remark { get; set; }
    }
}
