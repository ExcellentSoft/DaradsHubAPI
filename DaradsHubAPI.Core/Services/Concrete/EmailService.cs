using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Shared.Helpers;
using DaradsHubAPI.Shared.Extentions;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Static;
using DaradsHubAPI.Shared.Customs;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Core.Model.Request;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Concrete
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;
        private string SendGridAPIKEY;
        private readonly SmtpClient? _smtpServer;
        private List<EmailAddress> emailAddresses = new();
        private IUnitOfWork _unitOfWork;
        private IWhatsAppService _whatsAppService;
        private readonly ISendGridClient _sendGridClient;
        public EmailService(IOptions<AppSettings> appSettings, IUnitOfWork unitOfWork, ISendGridClient sendGridClient, IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
            _sendGridClient = sendGridClient;
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
            SendGridAPIKEY =
            ConvertersHelper.ConvertByteToString(ConvertersHelper.ConvertStringToByte(_appSettings.SendGridEncryptedKey));

        }
        public bool SendHtmlMail(string mailTo, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendMail(string mailTo, string subject, string body, string displayName, bool useTemplate = true)
        {
            if (!mailTo.IsValidEmail() && !mailTo.Contains(','))
            {
                throw new AppException(GenericStrings.InvalidEmail);

            }
            bool isSent = false;
            var adsEm = @$" <br/> <p> Visit  <a href='https://solo.to/Dartechlabs'>solo.to/Dartechlabs</a>  to explore more of our website features like boosting of all 
social media accounts, selling of all social media account, boosting of WhatsApp views, All countries number’s verification, trading of all kinds of cryptocurrencies and giftcards, we also sell Chinese yuan.
 </p> <p><br/>  Visit <a href='https://solo.to/Dartechlabs'> solo.to/Dartechlabs to explore more. </a></p> ";

            if (useTemplate)
                body += adsEm;
            try
            {
                var decryptKey = StringExtensions.Decrypt(SendGridAPIKEY);

                var client = new SendGridClient(decryptKey);

                string[] Emails = mailTo.Split(',');
                int le = Emails.Length;
                var from = new EmailAddress(_appSettings.EmailFrom, displayName);
                var to = new EmailAddress(mailTo, "");
                for (int i = 0; i <= le - 1; i++)
                {
                    if (Emails[i].Trim().IsValidEmail())
                    {
                        var email = new EmailAddress(Emails[i].Trim(), "");
                        emailAddresses.Add(email);
                    }
                }

                if (le <= 1)
                {
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", body);

                    var response = await client.SendEmailAsync(msg);
                    isSent = true;
                }
                else
                {
                    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emailAddresses, subject, "", body);
                    var response = await client.SendEmailAsync(msg);
                    isSent = true;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message.ToString();
                return isSent;
            }
            return isSent;
        }

        public Task SendMailHtmlMultipleWithCC(string mailTo, List<string> cc, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public bool SendMail_Attachment(string mailTo, string subject, string body, string displayName, byte[] attachment_file, string ReceiverName)
        {
            bool isSent = false;
            try
            {


                var client = new SendGridClient(SendGridAPIKEY);
                string[] Emails = mailTo.Split(',');
                int le = Emails.Length;

                var from = new EmailAddress(_appSettings.EmailFrom, displayName);
                var to = new EmailAddress(mailTo, "");
                var plainTextContent = body;

                for (int i = 0; i <= le - 1; i++)
                {




                    if (Emails[i].Trim().IsValidEmail())
                    {
                        var email = new EmailAddress(Emails[i].Trim(), "");
                        emailAddresses.Add(email);
                    }

                }

                if (le <= 1)
                {
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, "");
                    //Attach File
                    if (attachment_file != null)
                    {
                        var dt = DateTime.Now;
                        var Statement = new SendGrid.Helpers.Mail.Attachment()
                        {
                            Content = Convert.ToBase64String(attachment_file),
                            Type = "application/pdf",
                            Filename = subject + ".pdf",
                            Disposition = "inline",
                            ContentId = "SOA"

                        };
                        msg.AddAttachment(Statement);
                    }
                    var response = client.SendEmailAsync(msg);
                    isSent = true;
                }
                else
                {
                    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emailAddresses, subject, plainTextContent, "");
                    //attach File
                    var response = client.SendEmailAsync(msg);
                    if (attachment_file != null)
                    {

                        var Statement = new SendGrid.Helpers.Mail.Attachment()
                        {
                            Content = Convert.ToBase64String(attachment_file),
                            Type = "application/pdf",
                            Filename = subject + ".pdf",
                            Disposition = "inline",
                            ContentId = "SOA"
                        };
                        msg.AddAttachment(Statement);
                    }
                    isSent = true;
                }


            }
            catch (Exception ex)
            {


                return isSent;
            }

            return isSent;

        }

        public Task<dynamic> SendWhatsappMessage(string phonenumber, string Message)
        {
            throw new NotImplementedException();
        }

        public bool SendMail_Attachment_XLSFile(string mailTo, string subject, string body, string displayName, byte[] attachment_file, string ReceiverName)
        {
            bool isSent = false;
            try
            {


                var client = new SendGridClient(SendGridAPIKEY);


                string[] Emails = mailTo.Split(',');
                int le = Emails.Length;

                var from = new EmailAddress(_appSettings.EmailFrom, displayName);

                var to = new EmailAddress(mailTo, "");
                var plainTextContent = body;

                for (int i = 0; i <= le - 1; i++)
                {




                    if (Emails[i].Trim().IsValidEmail())
                    {
                        var email = new EmailAddress(Emails[i].Trim(), "");
                        emailAddresses.Add(email);
                    }

                }
                var dt = DateTime.Now;
                if (le <= 1)
                {
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", plainTextContent);
                    //Attach File
                    if (attachment_file != null)
                    {

                        //message.Filename = "gv_order" + SharedClass.GetCode();
                        //                message.FileType = "application/vnd.ms-excel";
                        //                message.FileExtension = "xls";
                        //                message.attachment_file = AttachFile;
                        var Statement = new SendGrid.Helpers.Mail.Attachment()
                        {
                            Content = Convert.ToBase64String(attachment_file),
                            Type = "application/vnd.ms-excel",
                            Filename = "order_" + dt.ToString() + CustomizeCodes.GetCode() + ".xls",
                            Disposition = "inline",
                            ContentId = "SOA"

                        };
                        msg.AddAttachment(Statement);
                    }
                    var response = client.SendEmailAsync(msg);
                    isSent = true;
                }
                else
                {
                    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emailAddresses, subject, "", plainTextContent);
                    //attach File
                    var response = client.SendEmailAsync(msg);
                    if (attachment_file != null)
                    {

                        var Statement = new SendGrid.Helpers.Mail.Attachment()
                        {
                            Content = Convert.ToBase64String(attachment_file),
                            Type = "application/vnd.ms-excel",
                            Filename = "order_" + dt.ToString() + CustomizeCodes.GetCode() + ".xls",
                            Disposition = "inline",
                            ContentId = "SOA"
                        };
                        msg.AddAttachment(Statement);
                    }
                    isSent = true;
                }


            }
            catch (Exception ex)
            {


                return isSent;
            }

            return isSent;
        }

        public Task SendMail_SendGrid_Template(string mailTo, string subject, string body, string displayName, string templateId)
        {
            throw new NotImplementedException();
        }

        public async Task SendMail_SendGrid_Template_Bulk(BulkEmailParam param)
        {

            var pageSize = 500;
            var getAudience = await _unitOfWork.Users.GetAudiences(param.EmailParam.AudienceType);
            if (getAudience == null) { throw new AppException("Invalid request, something went wrong"); }
            var subscriberCount = await _unitOfWork.Users.CountCustomers(param.EmailParam.AudienceType);
            var amountOfPages = (int)Math.Ceiling((double)subscriberCount / pageSize);
            int CountSent = 0;
            //CHECK sENDERpARAM TO DETERMINE customers and Body
            // param.SenderParam.VendorId 
            var msgId = param.EmailParam.DeliveryTYpe + CustomizeCodes.GetUniqueId();
            for (var pageIndex = 0; pageIndex < amountOfPages; pageIndex++)
            {
                var subscribers = await _unitOfWork.Users.GetCustomers(param.EmailParam.AudienceType, pageSize, pageIndex);
                if (subscribers.Count > pageSize) { throw new AppException("Something is wrong !"); }

                //subscribers.Add(
                //     new userstb { email = "JonahDarlington75@gmail.com", fullname = "DARLINGTON ", FirstName = "Mr DARLINGTON", phone = "000000" });

                var message = new SendGridMessage
                {
                    From = new EmailAddress(_appSettings.EmailFrom, _appSettings.MailDisplayName),
                    Subject = param.EmailParam.Subject,
                    HtmlContent = param.EmailParam.MessageBody,

                    Personalizations = subscribers.Select(s => new Personalization
                    {
                        Tos = new List<EmailAddress> { new EmailAddress(s.email, s.FirstName) },
                        TemplateData = new
                        {
                            firstname = s.FirstName,
                            sbodyContent = param.EmailParam.MessageBody
                        }
                    }).ToList(),
                };
                if (param.EmailParam.UseTemplate)
                {
                    var getTemplate = await _unitOfWork.Users.GetEmailTemplates(getAudience.TemplateId);
                    if (getTemplate != null)
                    {
                        message.TemplateId = getTemplate.TemplateId;
                    }

                }


                if (param.EmailParam.DeliveryTYpe == "S")
                {
                    if (param.EmailParam.ScheduleDays > 0 && param.EmailParam.ScheduleDays <= 3)
                    {
                        var sendAt = DateTimeOffset.Now.AddDays(param.EmailParam.ScheduleDays).ToUnixTimeSeconds();
                        message.SendAt = sendAt;
                    }
                    else if (param.EmailParam.ScheduleHours > 0)
                    {
                        var sendAt = DateTimeOffset.Now.AddHours(param.EmailParam.ScheduleHours).ToUnixTimeSeconds();
                        message.SendAt = sendAt;
                    }
                    else
                    {
                        var sendAt = DateTimeOffset.Now.AddMinutes(param.EmailParam.ScheduleMinutes).ToUnixTimeSeconds();
                        message.SendAt = sendAt;

                    }
                }


                var response = await _sendGridClient.SendEmailAsync(message);
                if (response.IsSuccessStatusCode)
                {
                    CountSent += 1;
                    await _unitOfWork.Users.SaveMessageSentLog(new MessagesSentLogs
                    {
                        CreateDate = GetLocalDateTime.CurrentDateTime(),
                        MessageType = $"Message Audience: {getAudience.Name}",
                        TotalSent = subscribers.Count,
                        MsgId = msgId
                    });
                    await Task.Delay(3000);
                }

            }
            await _unitOfWork.Users.SaveSentMessageDetails(new MessagesSent
            {
                SentDate = GetLocalDateTime.CurrentDateTime(),
                Message = param.EmailParam.MessageBody,
                Subject = param.EmailParam.Subject,
                Audience = getAudience.Name + ".=> " + subscriberCount.ToString() + " Emails ",
                AudienceCode = getAudience.Code,
                TemplateId = getAudience.TemplateId,
                Status = param.EmailParam.DeliveryTYpe,
                MsgId = msgId
            });
        }

        public async Task<MessagesSent> GetSentMessage(int Id)
        {
            return await _unitOfWork.Users.GetSentMessage(Id);
        }

        public async Task ReSendMailMessage(int sentId)
        {
            var getsent = await GetSentMessage(sentId);
            if (getsent != null)
            {
                BulkEmailParam param = new BulkEmailParam();
                param.EmailParam = new EmailItems
                {
                    DeliveryTYpe = "N",
                    UseTemplate = true,
                    AudienceType = getsent.AudienceCode,
                    ScheduleHours = 0,
                    ScheduleMinutes = 0,
                    ScheduleDays = 0,
                    MessageBody = getsent.Message,
                    Subject = getsent.Subject,
                    displayName = "",

                };
                await SendMail_SendGrid_Template_Bulk(param);
            }

        }

        public async Task<List<MessagesSent>> GetAllSentMessages()
        {
            return await _unitOfWork.Users.GetAllSentMessages();
        }

        public async Task SendBulkWhatsApp_Message(BulkEmailParam param)
        {

            var pageSize = 300;
            var getAudience = await _unitOfWork.Users.GetAudiences(param.EmailParam.AudienceType);
            if (getAudience == null) { throw new AppException("Invalid request, something went wrong"); }
            var subscriberCount = await _unitOfWork.Users.CountCustomers(param.EmailParam.AudienceType);
            var amountOfPages = (int)Math.Ceiling((double)subscriberCount / pageSize);
            int CountSent = 0;

            var msgId = "WBM" + CustomizeCodes.GetUniqueId();
            for (var pageIndex = 0; pageIndex < amountOfPages; pageIndex++)
            {
                var subscribers =// new List<userstb>();
                await _unitOfWork.Users.GetCustomers(param.EmailParam.AudienceType, pageSize, pageIndex);
                subscribers = subscribers.Where(x => !string.IsNullOrWhiteSpace(x.phone)).ToList();
                if (subscribers.Count > pageSize) { throw new AppException("Something is wrong !"); }

                subscribers.Add(new userstb { phone = "08074306999", FirstName = "Waliu W.O " });
                // subscribers.Add(new userstb { phone = "08118789239", FirstName = "Jonah DG " });

                foreach (var subscriber in subscribers)
                {
                    if (!string.IsNullOrEmpty(subscriber.phone))
                    {
                        if (subscriber.phone.IsValidPhoneNumber())
                        {//WhatsApp message
                            var msg = "Dear " + subscriber.FirstName + " , " +
                              param.EmailParam.MessageBody;
                            var msgSent = await _whatsAppService.SendWhatsAppMessage(subscriber.phone, msg, "+234");
                            if (msgSent) CountSent += 1;
                            //Delay

                        }

                    }
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                await _unitOfWork.Users.SaveMessageSentLog(new MessagesSentLogs
                {
                    CreateDate = GetLocalDateTime.CurrentDateTime(),
                    MessageType = $"WhatsApp-Message To : {getAudience.Name}",
                    TotalSent = CountSent,
                    MsgId = msgId
                });
                await Task.Delay(500);

            }
            await _unitOfWork.Users.SaveSentMessageDetails(new MessagesSent
            {
                SentDate = GetLocalDateTime.CurrentDateTime(),
                Message = param.EmailParam.MessageBody,
                Subject = param.EmailParam.Subject,
                Audience = getAudience.Name + " Total Sent => " + CountSent + " WhatsApp numbers",
                AudienceCode = getAudience.Code,
                TemplateId = getAudience.TemplateId,
                Status = "WB",
                MsgId = msgId
            });
        }

        public async Task<bool> SendWhatsAppMessage(string phonenumber, string firstName, string Message)
        {
            var msg = "Dear " + firstName + "  " +
                                 Message;
            var msgSent = await _whatsAppService.SendWhatsAppMessage(phonenumber, msg, "+234");
            return msgSent;

        }

        public async Task SendMail_Bulk_Gen(string subject, string Message, string audience)
        {
            Message = FormatEmailMessasage(Message);
            var pageSize = 500;
            var getAudience = await _unitOfWork.Users.GetAudiences(audience);
            if (getAudience == null) { throw new AppException("Invalid request, something went wrong"); }
            var subscriberCount = await _unitOfWork.Users.CountCustomers(audience);
            var amountOfPages = (int)Math.Ceiling((double)subscriberCount / pageSize);
            int CountSent = 0;


            for (var pageIndex = 0; pageIndex < amountOfPages; pageIndex++)
            {
                var subscribers = //new List<userstb>();
                    await _unitOfWork.Users.GetCustomers(audience, pageSize, pageIndex);
                //  if (subscribers.Count > pageSize) { throw new ApiGenericException("Something is wrong !"); }

                subscribers.Add(
                     new userstb { email = "iloriwaliu@gmail.com", fullname = "Waliu ", FirstName = "Mr OLADIPUPO", phone = "000000" });

                var message = new SendGridMessage
                {
                    From = new EmailAddress("info@kaybilltech.com", "KAYBILL TECHNOLOGIES"), //_appSettings.EmailFrom
                                                                                             //_appSettings.EmailFrom, _appSettings.MailDisplayName),
                    Subject = subject,
                    HtmlContent = Message,

                    Personalizations = subscribers.Select(s => new Personalization
                    {
                        Tos = new List<EmailAddress> { new EmailAddress(s.email, s.FirstName) },

                    }).ToList(),
                };

                var response = await _sendGridClient.SendEmailAsync(message);
                if (response.IsSuccessStatusCode)
                {
                    CountSent += 1;
                    await _unitOfWork.Users.SaveMessageSentLog(new MessagesSentLogs
                    {
                        CreateDate = GetLocalDateTime.CurrentDateTime(),
                        MessageType = $"General-Message Audience: {getAudience.Name}",
                        TotalSent = subscribers.Count,
                        MsgId = "GenMsg-" + GetLocalDateTime.CurrentDateTime()
                    });

                }

            }

        }

        public async Task SendBulkWhatsApp_MessageGen(string Message, string audience)
        {
            var pageSize = 100;
            var getAudience = await _unitOfWork.Users.GetAudiences(audience);
            if (getAudience == null) { throw new AppException("Invalid request, something went wrong"); }
            var subscriberCount = await _unitOfWork.Users.CountCustomers(audience);
            var amountOfPages = (int)Math.Ceiling((double)subscriberCount / pageSize);
            int CountSent = 0;

            var msgId = "WBGen" + CustomizeCodes.GetUniqueId();
            for (var pageIndex = 0; pageIndex < amountOfPages; pageIndex++)
            {
                var subscribers =
                await _unitOfWork.Users.GetCustomers(audience, pageSize, pageIndex);
                subscribers = subscribers.Where(x => !string.IsNullOrWhiteSpace(x.phone)).ToList();
                if (subscribers.Count > pageSize) { throw new AppException("Something is wrong !"); }

                subscribers.Add(new userstb { phone = "08101032506", FirstName = "Waliu W.O " });
                foreach (var subscriber in subscribers)
                {
                    if (!string.IsNullOrEmpty(subscriber.phone))
                    {
                        if (subscriber.phone.IsValidPhoneNumber())
                        {//WhatsApp message
                            var msg = "Hello " +
                              Message;
                            var msgSent = await _whatsAppService.SendWhatsAppMessage(subscriber.phone, msg, "+234");
                            if (msgSent) CountSent += 1;
                            //Delay

                        }

                    }
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
                await _unitOfWork.Users.SaveMessageSentLog(new MessagesSentLogs
                {
                    CreateDate = GetLocalDateTime.CurrentDateTime(),
                    MessageType = $"WhatsApp-Message To : {getAudience.Name}",
                    TotalSent = CountSent,
                    MsgId = msgId
                });
                //await Task.Delay(500);

            }

        }
        //To do : 

        /* 

        //test on PostMan
        //Now Create Bulk-Message Page  on Webform
        //Message, Subject, Medium (Email-Bulk: WhatsApp-Bulk), 
        Schedule :{Days, Hours, Minutes},  Send Now, Select Audience{All, LogCustomers, etc}, Use Template =>Pick From Templates
        //
        */
        private string FormatEmailMessasage(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = @$"Complement of the Season ! <br/>
<p>Are you selling  digital products online ?
Engaged  in gift cards or crypto exchanges?
Or deal in social media  advertising?<p/>

<b>This is Updates For YOU ! </b>

<p>Many online vendors make money by expanding and automate their sales business, What you truly need in 2024 to expand your business is a well-structured, secure, and creatively designed web and mobile-responsive applications. 

Not just a mere website, but applications that empower your customers to place orders, conduct seamless payments, receive instant notifications, and promptly obtain their items.<p/>
<br/>
<b>Ready to take the plunge? Begin now and avail yourself of a generous 20% discount !</b>

<p>Connect with us via WhatsApp at:</p>
<p> <b>+2348074306999
+2348101032506

Or drop us an email at kaybilltech@gmail.com.<b/>
<br/>

</p>
<p> Visit website : https://kaybilltech.com/ </p>

<br/>
Concerned about costs? Fear not !
<p>Your business's value and returns are our top priority. At KayBill Tech, we specialize in developing applications for  online or offline enterprise with swift delivery and unwavering 100% support.</p>

That's not all—we'll provide you with :
 1. An admin portal for streamlined business management, complete with dashboard statistics tracking all activities.
 2. over 10,000 auto email marketing targets with unique templates. 
<br/>

<p><b>Best regards,</b></p>
<br/>
<a href='https://kaybilltech.com'> KayBill Technologies.</a> ";
                return msg;
            }
            return msg;
        }

        public bool SendMail_VendorWeb(string mailTo, string subject, string body, string displayName, bool useTemplate = true)
        {
            //if (!mailTo.IsValidEmail() && !mailTo.Contains(','))
            //{
            //    throw new ApiGenericException(GenericStrings.InvalidEmail);

            //}
            bool isSent = false;
            //            var adsEm = @$" <br/> <p> Visit  <a href='https://solo.to/Dartechlabs'>solo.to/Dartechlabs</a>  to explore more of our website features like boosting of all 
            //social media accounts, selling of all social media account, boosting of WhatsApp views, All countries number’s verification, trading of all kinds of cryptocurrencies and giftcards, we also sell Chinese yuan.
            // </p> <p><br/>  Visit <a href='https://solo.to/Dartechlabs'> solo.to/Dartechlabs to explore more. </a></p> ";

            //            if (useTemplate)
            //                body += adsEm;
            try
            {

                var client = new SendGridClient(SendGridAPIKEY);

                string[] Emails = mailTo.Split(',');
                int le = Emails.Length;
                var from = new EmailAddress(_appSettings.EmailFrom, displayName);
                var to = new EmailAddress(mailTo, "");
                for (int i = 0; i <= le - 1; i++)
                {
                    if (Emails[i].Trim().IsValidEmail())
                    {
                        var email = new EmailAddress(Emails[i].Trim(), "");
                        emailAddresses.Add(email);
                    }
                }

                if (le <= 1)
                {
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", body);

                    var response = client.SendEmailAsync(msg);
                    isSent = true;
                }
                else
                {
                    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emailAddresses, subject, "", body);
                    var response = client.SendEmailAsync(msg);
                    isSent = true;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message.ToString();
                return isSent;
            }
            return isSent;
        }
    }
}