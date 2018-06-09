namespace GuardWebAPI.WeChat.Models.Goods
{
    public class GoodsPutRequest
    {
        public bool Latest { get; set; }

        public string Lat { get; set; }
        public string Lon { get; set; }
        public ChannelType Channel { get; set; }
        public long RawId { get; set; }
        public int BuyNum { get; set; }
        public bool All { get; set; }
        /// <summary>
        /// 省份名
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市名
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区县名
        /// </summary>
        public string District { get; set; }
    }
}