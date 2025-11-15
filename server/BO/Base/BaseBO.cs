namespace server.BO.Base
{
    public class BaseBO
    {
        public DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime DeletedDate { get; set; }
        public string DeletedUser { get; set; }
        public bool IsDelete { get; set; }
    }

    public class BaseResponseBO
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
