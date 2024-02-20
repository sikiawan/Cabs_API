namespace Model.Models
{
    public class ResponseManager
    {
        public bool IsSuccess { get; set; }
        public dynamic? Response { get; set; }
        public IEnumerable<dynamic>? Errors { get; set; }

    }
}
