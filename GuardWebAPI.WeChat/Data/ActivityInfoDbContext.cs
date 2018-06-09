using GuardWebAPI.WeChat.Models.ActivityInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Data
{
    public class ActivityInfoDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseMySQL(@"Server=rm-uf6w1w1u86nif1282mo.mysql.rds.aliyuncs.com;Database=activity618;User=root;Password=Xululu880;Charset=utf8;");

        public virtual DbSet<UserInfoModel> UserInfo { get; set; }
        public virtual DbSet<UserBoxInfo> UserBoxInfo { get; set; }
        public virtual DbSet<UserLotteryInfoModel> UserLotteryInfo { get; set; }
        public virtual DbSet<UserLotteryCount> UserLotteryCount { get; set; }
        public virtual DbSet<UserLotteryHelp> UserLotteryHelp { get; set; }
        public virtual DbSet<LotteryChanceModel> LotteryChance { get; set; }
    }
}