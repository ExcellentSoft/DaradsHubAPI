using System.ComponentModel;
using System.Reflection;

namespace DaradsHubAPI.Domain.Enums;
public class Enum
{
    public enum StatusEnum
    {/// <summary>
     ///Display successful Message e.t toast or modal
     /// </summary>
        Success = 200,
        /// <summary>
        ///Display message to show the request failed or Why it faile
        /// </summary>
        Failed = 201,
        /// <summary>
        ///Display message e.g Record Not Found or No Data
        /// </summary>
        NoRecordFound = 204,
        /// <summary>
        /// Display message to show the validation is wrong due to parameters or logic
        /// </summary>
        Validation = 203,
        /// <summary>
        /// Display generic message to show what went wrong
        /// </summary>
        Message = 205,
        /// <summary>
        /// Error Messages
        /// </summary>
        ServerError = 500,
        SystemError = 999,
        Unauthorised = 401
    }

    public enum LogRequestEnum
    {
        Request, //0
        Order,
        Available
    }

    public enum UploadRequestEnum
    {
        Upload,
        Order
    }

    public enum ProductStatus
    {
        Saved = 1
    }

    public enum EntityStatusEnum
    {
        Active = 1,
        InActive,
        Delete,
        Suspended,
        Blocked
    }

    public enum TransactionTypeEnum
    {
        Debit,
        Credit
    }
    public enum TransactionStatusEnum
    {
        Pending,
        Complete
    }

    public enum OtpCodeStatusEnum
    {
        Sent = 1,
        Verified,
        Expired,
        Invalidated
    }

    public enum OtpVerificationPurposeEnum
    {
        EmailVerification,
        ForgetPassword
    }

    public enum OperationType
    {
        Increase,
        Decrease
    }

    public enum OrderStatus
    {
        [Description("Order")]
        Order = 1,
        [Description("Processing")]
        Processing,
        [Description("Cancelled")]
        Cancelled,
        [Description("Refunded")]
        Refunded,
        [Description("Completed")]
        Completed
    }
    public enum RequestStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Approved")]
        Approved,
        [Description("Rejected")]
        Rejected
    }
    public enum ProductRequestTypeEnum
    {
        [Description("Digital")]
        Digital = 1,
        [Description("Physical")]
        Physical
    }
    public enum DeliveryMethodType
    {
        Standard = 1,
        Express
    }

    public enum NotificationType
    {
        NewOrder = 1,
        OrderSent,
        OrderDelivered,
        Login,
        ForgetPassword,
        ChangeOrderStatus,
    }
}


