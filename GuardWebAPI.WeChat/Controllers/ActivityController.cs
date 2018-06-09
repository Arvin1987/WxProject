using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuardWebAPI.WeChat.Models;
using GuardWebAPI.WeChat.Models.ActivityInfo;
using GuardWebAPI.WeChat.Services.ActivityInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuardWebAPI.WeChat.Controllers
{
    [Produces("application/json")]
    public class ActivityController : Controller
    {
        /// <summary>
        /// 获取中奖记录总信息接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("api/activity/getlotteryinfo/{uid}")]
        public ResultBase GetLotteryInfo([FromBody] GetLotteryInfoReq req)
        {
            if (string.IsNullOrEmpty(req.Uid))
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ClientError_NotFound_Para,
                };
            }

            ActivityInfoService activityInfoService = new ActivityInfoService();
            GetLotteryInfoResq resq = new GetLotteryInfoResq();
            try
            {
                resq = activityInfoService.GetLotteryInfo(req);
            }
            catch (Exception ex)
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ServerError,
                    Message = ex.Message + ex.StackTrace
                };
            }

            if (resq != null)
            {
                return new ResultBase
                {
                    IsSuccess = true,
                    Code = CodeConstant.Success,
                    Data = resq
                };
            }
            else
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.DataNull
                };
            }
        }

        /// <summary>
        /// 抽奖接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("api/activity/getlottery/{uid}")]
        public ResultBase GetLottery([FromBody] GetLotteryInfoReq req)
        {
            if (string.IsNullOrEmpty(req.Uid))
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ClientError_NotFound_Para,
                };
            }

            ActivityInfoService activityInfoService = new ActivityInfoService();
            Tuple<int[], GetLotteryInfoResq> resq = null;
            try
            {
                resq = activityInfoService.GetLottery(req);
            }
            catch (Exception ex)
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ServerError,
                    Message = ex.Message + ex.StackTrace
                };
            }

            if (resq != null && resq.Item1.Count() > 0)
            {
                resq.Item2.lotteryIndex = resq.Item1;
                return new ResultBase
                {
                    IsSuccess = true,
                    Code = CodeConstant.Success,
                    Data = resq.Item2
                };
            }
            else
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ClientError_TodayCountIsOver
                };
            }
        }

        /// <summary>
        /// 开启宝箱接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("api/activity/openbox/{uid}")]
        public ResultBase OpenBox([FromBody] GetLotteryInfoReq req)
        {
            if (string.IsNullOrWhiteSpace(req.Uid))
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ClientError_NotFound_Para
                };
            }

            ActivityInfoService activityInfoService = new ActivityInfoService();
            bool resq = false;
            try
            {
                resq = activityInfoService.GetBox(req);
            }
            catch (Exception ex)
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ServerError,
                    Message = ex.Message + ex.StackTrace
                };
            }

            if (resq)
            {
                return new ResultBase
                {
                    IsSuccess = true,
                    Code = CodeConstant.Success,
                };
            }
            else
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ClientError_CountIsNotEnough
                };
            }
        }

        /// <summary>
        /// 帮助点亮接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("api/activity/helpgetlottery/{uid}")]
        public ResultBase HelpGetLottery([FromBody] GetLotteryInfoReq req)
        {
            if (string.IsNullOrWhiteSpace(req.Uid) || string.IsNullOrWhiteSpace(req.HelperUid))
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ClientError_NotFound_Para
                };
            }

            ActivityInfoService activityInfoService = new ActivityInfoService();
            Tuple<bool, int[], GetLotteryInfoResq> resq = new Tuple<bool, int[], GetLotteryInfoResq>(false, new int[] { -1 }, null);
            try
            {
                resq = activityInfoService.HelpGetLottery(req);
            }
            catch (Exception ex)
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ServerError,
                    Message = ex.Message + ex.StackTrace
                };
            }

            if (resq.Item1)
            {
                resq.Item3.lotteryIndex = resq.Item2;
                return new ResultBase
                {
                    IsSuccess = true,
                    Code = CodeConstant.Success,
                    Data = resq.Item3
                };
            }
            else
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ClientError_TodayCountIsOver
                };
            }
        }

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("api/activity/getuserinfo/{uid}")]
        public ResultBase GetUserInfo([FromBody] GetLotteryInfoReq req)
        {
            if (string.IsNullOrWhiteSpace(req.Uid))
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ClientError_NotFound_Para
                };
            }

            ActivityInfoService activityInfoService = new ActivityInfoService();
            UserInfoModel resq = null;
            try
            {
                resq = activityInfoService.GetUserInfo(req);
            }
            catch (Exception ex)
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ServerError,
                    Message = ex.Message + ex.StackTrace
                };
            }

            if (resq != null)
            {
                return new ResultBase
                {
                    IsSuccess = true,
                    Code = CodeConstant.Success,
                    Data = resq
                };
            }
            else
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.DataNull
                };
            }
        }

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("api/activity/getwxuid")]
        public ResultBase GetUId(string code)
        {
            ActivityInfoService activityInfoService = new ActivityInfoService();
            WXUIdInfo resq = null;
            try
            {
                resq = activityInfoService.GetUId(code);
            }
            catch (Exception ex)
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ServerError,
                    Message = ex.Message + ex.StackTrace
                };
            }

            if (resq != null && string.IsNullOrEmpty(resq.errcode))
            {
                return new ResultBase
                {
                    IsSuccess = true,
                    Code = CodeConstant.Success,
                    Data = resq
                };
            }
            else
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.DataNull
                };
            }
        }

        [HttpGet("api/activity/getwxcode")]
        public ResultBase GetWxCode(string scene, string page = "pages/index/index")
        {
            ActivityInfoService activityInfoService = new ActivityInfoService();
            string resq = null;
            try
            {
                resq = activityInfoService.GetWxCode(scene, page);
            }
            catch (Exception ex)
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.ServerError,
                    Message = ex.Message + ex.StackTrace
                };
            }
            if (!string.IsNullOrWhiteSpace(resq))
            {
                return new ResultBase
                {
                    IsSuccess = true,
                    Code = CodeConstant.Success,
                    Data = resq
                };
            }
            else
            {
                return new ResultBase
                {
                    IsSuccess = false,
                    Code = CodeConstant.DataNull
                };
            }
        }
    }
}