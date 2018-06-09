namespace GuardWebAPI.WeChat.Models
{
    public abstract class ResultCore
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}