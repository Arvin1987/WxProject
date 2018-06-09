namespace GuardWebAPI.WeChat.Models.OAuth
{
    public class OAuthUserInfo
    {
        public string OpenId { get; set; }
        public string Nickname { get; set; }
        public int Sex { get; set; }
        public string Language { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string HeadImgUrl { get; set; }
        public string UnionId { get; set; }
    }
}