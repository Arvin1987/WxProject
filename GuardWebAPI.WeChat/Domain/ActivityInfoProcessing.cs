using GuardWebAPI.WeChat.Data;
using GuardWebAPI.WeChat.Models.ActivityInfo;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuardWebAPI.WeChat.Domain
{
    public class ActivityInfoProcessing
    {
        private ActivityInfoDbContext _dbContext;
        private object obj = new object();

        public ActivityInfoProcessing()
        {
            if (_dbContext == null)
            {
                lock (obj)
                {
                    if (_dbContext == null)
                    {
                        _dbContext = new ActivityInfoDbContext();
                    }
                }
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public UserInfoModel GetUserInfoModel(GetLotteryInfoReq reqModel)
        {
            UserInfoModel userInfo = _dbContext.Set<UserInfoModel>().Where(m => m.F_UId == reqModel.Uid).FirstOrDefault();
            return userInfo;
        }

        /// <summary>
        /// 获取用户已领取的宝箱
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public List<UserBoxInfo> GetUserBoxInfos(GetLotteryInfoReq reqModel)
        {
            List<UserBoxInfo> userBoxInfos = _dbContext.UserBoxInfo.Where(m => m.F_UId == reqModel.Uid).ToList();
            return userBoxInfos;
        }

        /// <summary>
        /// 用户中奖记录数量
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public List<UserLotteryCount> GetUserLotteryCount(GetLotteryInfoReq reqModel)
        {
            List<UserLotteryCount> results = _dbContext.UserLotteryCount.FromSql("select count(*) as F_IndexCount,F_UId,F_LotteryIndex from T_UserLotteryInfo where F_LotteryIndex!=-1 AND F_UId=@uid GROUP BY F_UId,F_LotteryIndex", new MySqlParameter("uid", reqModel.Uid)).ToList();
            return results;
        }

        /// <summary>
        /// 用户已经中了多少个奖项
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public int GetUserLotteryTypeCount(GetLotteryInfoReq reqModel)
        {
            int results = _dbContext.UserLotteryCount.FromSql("select count(*) as F_IndexCount,F_UId,F_LotteryIndex from T_UserLotteryInfo where F_LotteryIndex!=-1 AND F_UId=@uid GROUP BY F_UId,F_LotteryIndex", new MySqlParameter("uid", reqModel.Uid)).Count();
            return results;
        }

        public bool InserUserBox(GetLotteryInfoReq para)
        {
            UserBoxInfo userBoxInfo = new UserBoxInfo();
            userBoxInfo.F_BoxIndex = para.BoxIndex;
            userBoxInfo.F_CreateTime = DateTime.Now;
            userBoxInfo.F_UId = para.Uid;
            _dbContext.UserBoxInfo.Add(userBoxInfo);
            return _dbContext.SaveChanges() > 0;
        }

        public bool InsertUserInfo(string uid, string headUrl, string nickName)
        {
            UserInfoModel userInfoModel = new UserInfoModel();
            userInfoModel.F_UId = uid;
            userInfoModel.F_HeadUrl = headUrl;
            userInfoModel.F_NickName = nickName;
            userInfoModel.F_CreatTime = DateTime.Now;
            _dbContext.UserInfo.Add(userInfoModel);
            return _dbContext.SaveChanges() > 0;
        }

        public bool ExistUId(string uid)
        {
            return _dbContext.UserInfo.Count(m => m.F_UId == uid) > 0;
        }

        public Tuple<bool, int> InserLotteryInfo(GetLotteryInfoReq para, int lotteryIndex)
        {
            UserLotteryInfoModel userLotteryInfoModel = new UserLotteryInfoModel();
            DateTime dt = DateTime.Now;
            userLotteryInfoModel.F_UId = para.Uid;
            userLotteryInfoModel.F_CreateTime = dt;
            userLotteryInfoModel.F_CreateUId = string.IsNullOrWhiteSpace(para.HelperUid) ? para.Uid : para.HelperUid;
            userLotteryInfoModel.F_CreateDate = dt.ToString("yyyy-MM-dd");
            userLotteryInfoModel.F_LotteryIndex = lotteryIndex;
            _dbContext.UserLotteryInfo.Add(userLotteryInfoModel);
            bool result = _dbContext.SaveChanges() > 0;
            return new Tuple<bool, int>(result, lotteryIndex);
        }

        /// <summary>
        /// 用户最后一次助力者
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public List<UserLotteryHelp> GetUserLotteryHelp(GetLotteryInfoReq reqModel)
        {
            List<UserLotteryHelp> results = _dbContext.UserLotteryHelp.FromSql("SELECT x.F_Id,  x.F_CreateTime, x.F_UId,x.F_LotteryIndex,x.F_CreateUId,y.F_HeadUrl as F_CreateHeadUrl,y.F_NickName as  F_CreateNickName from T_UserLotteryInfo AS x LEFT JOIN t_userinfo AS y on x.F_CreateUId = y.F_UId where x.F_LotteryIndex!=-1 AND x.F_UId = @uid ", new MySqlParameter("uid", reqModel.Uid)).ToList();
            return results;
        }

        /// <summary>
        /// 今天帮助点亮者点亮次数
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public int GetHelperCount(GetLotteryInfoReq reqModel)
        {
            int results = _dbContext.UserLotteryInfo.Count(m => m.F_UId == reqModel.Uid && m.F_CreateUId == reqModel.HelperUid);
            return results;
        }

        /// <summary>
        /// 获取用户今天已经抽奖次数
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public int GetTodayLotteryCount(GetLotteryInfoReq reqModel)
        {
            int lotteryCount = _dbContext.UserLotteryInfo.Count(m => m.F_CreateDate == DateTime.Now.ToString("yyyy-MM-dd") && m.F_UId == reqModel.Uid && m.F_CreateUId == reqModel.Uid);
            return lotteryCount;
        }

        /// <summary>
        /// 获取抽奖概率
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public List<LotteryChanceModel> GetLotteryChance()
        {
            return _dbContext.LotteryChance.ToList();
        }
    }
}