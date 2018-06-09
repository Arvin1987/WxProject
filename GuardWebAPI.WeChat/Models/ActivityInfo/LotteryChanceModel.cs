using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Models.ActivityInfo
{
    [Table("t_lotterychance")]
    public class LotteryChanceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long F_Id { get; set; }
        public int F_LotteryIndex { get; set; }
        public int F_LotteryChance { get; set; }
    }
}
