using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Models.ActivityInfo
{
    [Table("T_UserInfo")]
    public class UserInfoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long F_Id { get; set; }
        public string F_UId { get; set; }
        public string F_HeadUrl { get; set; }
        public string F_NickName { get; set; }
        public DateTime F_CreatTime { get; set; }
    }
}
