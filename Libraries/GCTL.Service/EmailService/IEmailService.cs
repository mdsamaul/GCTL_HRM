using GCTL.Core.ViewModels.EmailSettingsViewModel;

namespace GCTL.Service.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequestDTO model);
        string GenerateLeaveRequestEmail(string employeeName, string leaveType, string startDate, string endDate, string reason);
        string GenerateLeaveApprovalEmail(string employeeName, string leaveType, string startDate, string endDate);
        string GenerateLeaveRejectionEmail(string employeeName, string leaveType, string rejectionReason);

    }
}
