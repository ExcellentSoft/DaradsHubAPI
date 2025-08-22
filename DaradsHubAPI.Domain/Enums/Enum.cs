namespace DaradsHubAPI.Domain.Enums;
public class Enum
{
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
        Delete
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
        Order = 1,
        Accepted,
        Rejected,
        Shipped,
        Delivered,
        DeliveryConfirmed,
        Returned
    }
}
