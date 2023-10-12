namespace CafeteriaWeb.Services
{
    public class AuthMessageSenderOptions
    {
        public string? SmtpServer { get; set; }
        public string? SmtpPassword { get; set; }
        public string? SmtpUserName { get; set; }
        public string? EmailFrom { get; set; }
        public string? EmailDisplayName { get; set; }

    }
}
