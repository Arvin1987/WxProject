using GuardWebAPI.WeChat.Models;
using GuardWebAPI.WeChat.Models.ActivityInfo;
using GuardWebAPI.WeChat.Services.ActivityInfo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Linq;

namespace GuardWebAPI.WeChat.Controllers
{
    [Produces("application/json")]
    public class ActivityController : Controller
    {

        private IMemoryCache _memoryCache;

        public ActivityController(IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
        }

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
        public FileContentResult GetWxCode(string scene, string page = "pages/logs/main")
        {
            ActivityInfoService activityInfoService = new ActivityInfoService();
            byte[] resq = null;
            try
            {
                resq = activityInfoService.GetWxCode(scene, page);
            }
            catch (Exception ex)
            {
                return new FileContentResult(new byte[0], "image/jpeg");
            }
            //dynamic type = (new Program()).GetType();
            //string baseurl = System.IO.Path.GetDirectoryName(type.Assembly.Location);
            //var addrUrl = baseurl + "/qrcode.jpg";
            //System.IO.FileStream fs = new System.IO.FileStream(addrUrl, System.IO.FileMode.Open);
            if (resq != null && resq.Length > 0)
            {
                return new FileContentResult(resq, "image/jpeg");
            }
            else
            {
                return new FileContentResult(new byte[0], "image/jpeg");
            }
        }

        /// <summary>
        /// 查询H5用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("api/activity/geth5userinfo")]
        public ResultBase GetH5UserInfo(string code)
        {
            ActivityInfoService activityInfoService = new ActivityInfoService();
            H5UserInfo resq = null;
            try
            {
                resq = activityInfoService.GetH5UserInfo(code);
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

            if (resq != null && !string.IsNullOrEmpty(resq.openid))
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
        /// 查询H5用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("api/activity/getSignature")]
        public ResultBase GetH5Signature(string code = "", string url = "https://activity.topcn.xin/index.html")
        {
            ActivityInfoService activityInfoService = new ActivityInfoService();
            H5Sign resq = new H5Sign();
            try
            {
                if (!_memoryCache.TryGetValue("Cache_H5Ticket", out string ticket))
                {
                    ticket = activityInfoService.GetH5Ticket(code);
                    _memoryCache.Set("Cache_H5Ticket", ticket, DateTimeOffset.Now.AddHours(1.5));
                }

                if (Request.Headers.TryGetValue("Referer", out StringValues value) && value.Count > 0)
                {
                    resq.url = value[0];
                }
                else
                {
                    resq.url = url;
                }

                Console.WriteLine("SignRequestUrl：" + resq.url);

                resq.AppId = "wx3283d85d64449029";
                resq.Signature = activityInfoService.Sign(ticket, resq);
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

            if (resq != null && !string.IsNullOrEmpty(resq.Signature))
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