namespace GuardWebAPI.WeChat.Models
{
    public class CodeConstant
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const int Success = 200;
        /// <summary>
        /// 入参appid不存在
        /// </summary>

        public const int ClientError_NotFound_AppId = 401;
        /// <summary>
        /// 必要入参不足
        /// </summary>

        public const int ClientError_NotFound_Para = 402;
        /// <summary>
        /// 今日抽奖次数已不足
        /// </summary>

        public const int ClientError_TodayCountIsOver = 403;
        /// <summary>
        /// 开宝箱所需点亮数量不足
        /// </summary>

        public const int ClientError_CountIsNotEnough = 405;
        /// <summary>
        /// 未查到数据
        /// </summary>

        public const int DataNull = 205;
        /// <summary>
        /// 服务器异常
        /// </summary>

        public const int ServerError = 500;
    }
}