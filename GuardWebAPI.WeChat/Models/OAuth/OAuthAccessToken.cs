using Newtonsoft.Json;

namespace GuardWebAPI.WeChat.Models.OAuth
{
    public class OAuthAccessToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        public string OpenId { get; set; }
        public string Scope { get; set; }
    }
}