using System.ComponentModel;

namespace DaradsHubAPI.Shared.enums;

public class Enums
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

}
