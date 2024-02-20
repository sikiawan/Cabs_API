namespace CabsAPI.Helpers
{
    public enum JsonStatus
    {
        Error = 0,
        Exist = -1,
        Success = 1,
        Confirm = 2
    }

    public enum NotificationColor
    {
        Error,
        Success,
        Warning
    }

    public static class ErrorLevelExtensions
    {
        /// <summary>
        /// Get Color Name
        /// </summary>
        /// <param name="jsonColor"></param>
        /// <returns>string</returns>
        public static string ToColorName(this NotificationColor jsonColor) => jsonColor.ToString().ToLower();
    }
    public class ResultFormat
    {
        public ResultFormat()
        {
        }
        public ResultFormat(JsonStatus status, string management, dynamic msg,
                            string color, string link = "", string callback = "", int saveType = 0)
        {
            this.status = status;
            this.msg = msg;
            this.management = management;
            this.color = color;
            this.link = link;
            this.callback = callback;
            this.saveType = saveType;
        }
        public JsonStatus status { get; set; }
        public string link { get; set; }
        public string color { get; set; }
        public string management { get; set; }
        public string callback { get; set; }
        public int saveType { get; set; }
        public dynamic msg { get; set; }

        public static ResultFormat SuccessResult(dynamic msg, string management, string link = "", string callBack = "", int saveType = 1)
        {
            return new ResultFormat(JsonStatus.Success, management, msg, NotificationColor.Success.ToColorName(), link, callBack, saveType);
        }
        public static ResultFormat ErrorResult(dynamic msg, string management, string link = "")
        {
            return new ResultFormat(JsonStatus.Error, management, msg, NotificationColor.Error.ToColorName(), link);
        }
        public static ResultFormat ExistResult(dynamic msg, string management, string link = "", string callBack = "")
        {
            return new ResultFormat(JsonStatus.Exist, management, msg, NotificationColor.Error.ToColorName(), link, callBack);
        }
        public static ResultFormat WarningResult(dynamic msg, string management, string link = "")
        {
            return new ResultFormat(JsonStatus.Error, management, msg, NotificationColor.Warning.ToColorName(), link);
        }
    }
}
