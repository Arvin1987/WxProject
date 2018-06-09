namespace GuardWebAPI.WeChat.Models.Goods
{
    /// <summary>
    /// 商品详情实体类。
    /// </summary>
    public class GoodsDetails
    {
        /// <summary>
        /// 商品标识
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 原始商品标识
        /// </summary>
        public long RawId { get; set; }
        /// <summary>
        /// 商品渠道来源
        /// </summary>
        public ChannelType Channel { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商品售卖价格
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 商品介绍图片
        /// </summary>
        public string Image { get; set; }

        #region JD
        /// <summary>
        /// 京东商品分类
        /// </summary>
        public long[] JDCategoryIds { get; set; }
        /// <summary>
        /// 京东商品是否已停止销售
        /// </summary>
        public bool StopSelling { get; set; }
        #endregion
    }
}