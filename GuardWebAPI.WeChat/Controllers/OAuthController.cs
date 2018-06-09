using GSBase.ConfigCenter;
using GuardWebAPI.WeChat.Models;
using GuardWebAPI.WeChat.Models.OAuth;
using GuardWebAPI.WeChat.Services.JSSDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace GuardWebAPI.WeChat.Controllers
{
    public class OAuthController : Controller
    {
        private JSSDKService _jsSDKService;

        public OAuthController(JSSDKService jsSDKService)
        {
            _jsSDKService = jsSDKService;
        }

        // GET api/wechat/oauth/authorize/wxeeb48df50f322507
        [HttpGet("api/wechat/oauth/authorize/{appId}")]
        public async Task<IActionResult> AuthorizeAsync(string appId, string code, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(returnUrl))
            {
                return NotFound();
            }

            string secret = ConfigCenterClient.GetSection("WeChatAccount").GetValue<string>(appId, null);
            if (secret == null)
            {
                return NotFound();
            }

            OAuthAccessToken accessTokenResponse = await GetAccessTokenAsync(appId, secret, code);
            OAuthUserInfo userInfo = new OAuthUserInfo
            {
                OpenId = accessTokenResponse?.OpenId
            };
            if (accessTokenResponse != null && accessTokenResponse.Scope.Contains("snsapi_userinfo"))
            {
                userInfo = await GetUserInfoAsync(accessTokenResponse.AccessToken, accessTokenResponse.AccessToken);
            }

            returnUrl = HttpUtility.UrlDecode(returnUrl);
            string userInfoParams = accessTokenResponse.Scope.Contains("snsapi_userinfo") ? $"nickname={ HttpUtility.UrlEncode(userInfo.Nickname) }&headimgurl={ HttpUtility.UrlEncode(userInfo.HeadImgUrl) }" : string.Empty;
            return Redirect($"{ returnUrl }{ (returnUrl.Contains("?") ? "&" : "?") }openId={ userInfo.OpenId }&{ userInfoParams }");
        }

        // POST api/wechat/oauth/url/wxeeb48df50f322507
        [HttpPost("api/wechat/oauth/url/{appId}")]
        [Produces("application/json", "application/x-www-form-urlencoded")]
        public async Task<ResultBase> UrlAsync(string appId, string scope, string returnUrl, bool @short = true)
        {
            string authorizeUrl = $"{Request.Scheme}://{ Request.Host }{ Request.PathBase }/api/wechat/oauth/authorize/{ appId }?returnUrl={ HttpUtility.UrlEncode(returnUrl) }";
            string url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={ appId }&redirect_uri={ HttpUtility.UrlEncode(authorizeUrl) }&response_type=code&scope={ scope }#wechat_redirect";
            string shortUrl = @short ? (await _jsSDKService.ShortUrlAsync(appId, url)) : url;
            return new ResultBase
            {
                IsSuccess = true,
                Code = CodeConstant.Success,
                Data = shortUrl
            };
        }

        private async Task<OAuthAccessToken> GetAccessTokenAsync(string appId, string secret, string code)
        {
            WebClient client = new WebClient();
            string response = await client.DownloadStringTaskAsync($"https://api.weixin.qq.com/sns/oauth2/access_token?appid={ appId }&secret={ secret }&code={ code }&grant_type=authorization_code");
            if (string.IsNullOrWhiteSpace(response))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<OAuthAccessToken>(response);
        }

        private async Task<OAuthUserInfo> GetUserInfoAsync(string accessToken, string openId)
        {
            WebClient client = new WebClient();
            string response = await client.DownloadStringTaskAsync($"https://api.weixin.qq.com/sns/userinfo?access_token={ accessToken }&openid={ openId }&lang=zh_CN");
            if (string.IsNullOrWhiteSpace(response))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<OAuthUserInfo>(response);
        }
    }
}