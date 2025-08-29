using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.Services.Interface

{
    public interface IEmailService
    {
        bool SendMail(string mailTo, string subject, string body, string displayName, bool useTemplate = true);
        bool SendMail_VendorWeb(string mailTo, string subject, string body, string displayName, bool useTemplate = true);
        bool SendMail_Attachment(string mailTo, string subject, string body, string displayName, byte[] attachment_file, string ReceiverName);
        bool SendMail_Attachment_XLSFile(string mailTo, string subject, string body, string displayName, byte[] attachment_file, string ReceiverName);
        bool SendHtmlMail(string mailTo, string subject, string body);

        Task SendMailHtmlMultipleWithCC(string mailTo, List<string> cc, string subject, string body);

        Task<dynamic> SendWhatsappMessage(string phonenumber, string Message);

        //Template and Bulk email
        Task SendMail_SendGrid_Template(string mailTo, string subject, string body, string displayName, string templateId);
        Task SendMail_SendGrid_Template_Bulk(BulkEmailParam param);
        Task SendBulkWhatsApp_Message(BulkEmailParam param);
        Task<bool> SendWhatsAppMessage(string phonenumber, string firstName, string Message);
        Task<MessagesSent> GetSentMessage(int Id);
        Task ReSendMailMessage(int sentId);
        Task<List<MessagesSent>> GetAllSentMessages();

        Task SendMail_Bulk_Gen(string subject, string Message, string audience);
        Task SendBulkWhatsApp_MessageGen(string Message, string audience);
    }
}
