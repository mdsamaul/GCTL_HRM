namespace GCTL.Core.ViewModels.EmailSettingsViewModel
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MailFrom { get; set; }
        public bool EnableSsl { get; set; }
    }

    public class EmailRequestDTO
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class MailSettingsDTO
    {
        public string Email { get; set; } = "attendance@data-path.net";
        public string Password { get; set; } = "afA7n#Wvu25!";
        public string Host { get; set; } = "mail.data-path.net";
        public int Port { get; set; } = 587;
    }


}
