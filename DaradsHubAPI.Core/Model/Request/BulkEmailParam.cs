namespace DaradsHubAPI.Core.Model.Request
{
#nullable disable
    public class BulkEmailParam
    {
        public EmailItems EmailParam { get; set; }
        public SenderParam SenderParam { get; set; }
    }
    public class EmailItems
    {
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public string displayName { get; set; }
        public string TemplateId { get; set; }
        public string AudienceType { get; set; } = "all";
        public string DeliveryTYpe { get; set; } = "N"; //Send Now; S=Schedule
        public int ScheduleDays { get; set; } //Max 3 days
        public int ScheduleHours { get; set; }
        public int ScheduleMinutes { get; set; }
        public bool UseTemplate { get; set; }
    }
    public class SenderParam
    {
        public string VendorId { get; set; }
        public int LogtypeId { get; set; }
        public string AdminId { get; set; }

    }
    public class SendGenParam
    {
        public string subject { get; set; }
        public string Message { get; set; } = string.Empty;
        public string audience { get; set; }
    }
}
