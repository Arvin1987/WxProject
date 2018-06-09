using GSBase.ConfigCenter;
using GuardWebAPI.WeChat.JSSDK.Models;
using GuardWebAPI.WeChat.Models;
using GuardWebAPI.WeChat.Services.JSSDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Controllers
{
    public class JSSDKController : Controller
    {
        private IMemoryCache _memoryCache;
        private JSSDKService _jsSDKService;

        public JSSDKController(IMemoryCache memoryCache, JSSDKService jsSDKService)
        {
            _memoryCache = memoryCache;
            _jsSDKService = jsSDKService;
        }

        // GET api/wechat/config/wxeeb48df50f322507
        [HttpGet("api/wechat/config/{appId}")]
        public async Task<JSSDK_ConfigResponse> ConfigAsync(string appId)
        {
            string secret = ConfigCenterClient.GetSection("WeChatAccount").GetValue<string>(appId, null);
            if (secret == null)
            {
                return new JSSDK_ConfigResponse
                {
                    IsSuccess = false,
                    Code = CodeConstant.ClientError_NotFound_AppId
                };
            }

            if (!_memoryCache.TryGetValue("Ticket_" + appId, out string ticket))
            {
                string accessToken = await _jsSDKService.GetAccessTokenAsync(appId, secret);
                ticket = await _jsSDKService.GetTicketAsync(accessToken);
                if (ticket == null)
                {
                    return new JSSDK_ConfigResponse
                    {
                        IsSuccess = false,
                        Code = 402
                    };
                }
                _memoryCache.Set("Ticket_" + appId, ticket, DateTimeOffset.Now.AddHours(1.5));
            }

            JSSDK_ConfigResponse config = new JSSDK_ConfigResponse
            {
                IsSuccess = true,
                Code = CodeConstant.Success,
                AppId = appId
            };

            if (Request.Headers.TryGetValue("Referer", out StringValues value) && value.Count > 0)
            {
                config.Signature = _jsSDKService.Sign(ticket, value[0], config);
                return config;
            }
            else
            {
                return new JSSDK_ConfigResponse
                {
                    IsSuccess = false,
                    Code = 403
                };
            }
        }

        // POST api/wechat/shorturl/wxeeb48df50f322507 长链接转短链接接口
        [HttpPost("api/wechat/shorturl/{appId}")]
        [Produces("application/json", "application/x-www-form-urlencoded")]
        public async Task<ResultBase> ShortUrlAsync(string appId, string longUrl)
        {
            string shortUrl = await _jsSDKService.ShortUrlAsync(appId, longUrl);

            if (string.IsNullOrWhiteSpace(shortUrl))
            {
                return new ResultBase
                {
                    IsSuccess = false
                };
            }
            else
            {
                return new ResultBase
                {
                    IsSuccess = true,
                    Code = CodeConstant.Success,
                    Data = shortUrl
                };
            }
        }
    }
}