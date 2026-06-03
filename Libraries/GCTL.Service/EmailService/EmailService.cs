using GCTL.Core.ViewModels.EmailSettingsViewModel;
using System.Net;
using System.Net.Mail;

namespace GCTL.Service.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly MailSettingsDTO _mailSettings;

        public EmailService()
        {
            _mailSettings = new MailSettingsDTO();
        }

        public async Task SendEmailAsync(EmailRequestDTO model)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_mailSettings.Email, "Attendance System");
                mailMessage.To.Add(model.ToEmail);
                mailMessage.Subject = model.Subject;
                mailMessage.Body = model.Body;
                mailMessage.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port))
                {
                    smtpClient.Credentials = new NetworkCredential(_mailSettings.Email, _mailSettings.Password);
                    smtpClient.EnableSsl = true;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }

        public string GenerateLeaveRequestEmail(string employeeName, string leaveType, string startDate, string endDate, string reason)
        {
            return $@"<div style='font-family: Arial; padding: 20px; border: 1px solid #ddd;'>
                        <h2 style='color: #007bff;'>Leave Request Submitted</h2>
                        <p>Dear {employeeName},</p>
                        <p>Your leave request for <b>{leaveType}</b> from {startDate} to {endDate} is under review.</p>
                        <p><b>Reason:</b> {reason}</p>
                        <br><p>Regards,<br>HR Team</p>
                      </div>";
        }

        public string GenerateLeaveApprovalEmail(string employeeName, string leaveType, string startDate, string endDate)
        {
            return $@"<div style='font-family: Arial; padding: 20px; border: 1px solid #ddd;'>
                        <h2 style='color: #28a745;'>Leave Approved</h2>
                        <p>Dear {employeeName},</p>
                        <p>Your leave request for <b>{leaveType}</b> ({startDate} to {endDate}) has been <b>Approved</b>.</p>
                        <br><p>Regards,<br>HR Team</p>
                      </div>";
        }

        public string GenerateLeaveRejectionEmail(string employeeName, string leaveType, string rejectionReason)
        {
            return $@"<div style='font-family: Arial; padding: 20px; border: 1px solid #ddd;'>
                        <h2 style='color: #dc3545;'>Leave Rejected</h2>
                        <p>Dear {employeeName},</p>
                        <p>We regret to inform you that your request for <b>{leaveType}</b> has been rejected.</p>
                        <p><b>Reason:</b> {rejectionReason}</p>
                        <br><p>Regards,<br>HR Team</p>
                      </div>";
        }
    }
}