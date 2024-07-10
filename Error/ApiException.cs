namespace API.Error
{
    public class ApiException(int statuscode, string messages, string? details)
    {
        public int StatusCode { get; set; } = statuscode;
        public string Messages { get; set; } = messages;

        public string? Details { get; set; } = details;
    }
}
