using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Models.ActivityInfo
{
    [Table("T_UserBoxInfo")]
    public class UserBoxInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long F_Id { get; set; }
        public string F_UId { get; set; }
        public int F_BoxIndex { get; set; }
        public DateTime F_CreateTime { get; set; }
        public string F_Remark { get; set; }
    }
}
