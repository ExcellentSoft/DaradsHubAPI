using System.ComponentModel;
using System.Reflection;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Shared.Static
{
    public static class GenericStrings
    {
        public readonly static string InvalidEmail = "Invalid email address";
        public const string DATE_FORMAT = "MMMM d, yyyy";
        public const string TIME_FORMAT = "hh:mm tt";
        public const string PROFILE_IMAGES_FOLDER_NAME = "profile_images";
        public const string PRODUCT_IMAGES_FOLDER_NAME = "product_images";
        public const string CATEGORY_IMAGES_FOLDER_NAME = "categories_images";
        public const string ORDERDESCRIPTION = "Your product has been placed successfully";
        public const string CANCELLEDDESCRIPTION = "Your order has been cancelled successfully";
        public const string COMPLETEDDESCRIPTION = "Your order has been completed successfully";
        public const string REFUNDEDDESCRIPTION = "Your order has been refunded successfully";
        public const string PROCESSINGDESCRIPTION = "Your order currently processing";

        //public static string GetStatus(OrderStatus status)
        //{
        //    var response = status switch
        //    {
        //        OrderStatus.Order => "Ordered",
        //        OrderStatus.Shipped => "Shipped",
        //        OrderStatus.Delivered => "Delivered",
        //        OrderStatus.Accepted => "Accepted",
        //        OrderStatus.Rejected => "Rejected",
        //        OrderStatus.DeliveryConfirmed => "Delivery Confirmed",
        //        OrderStatus.Returned => "Returned",
        //        _ => "Nil",
        //    };
        //    return response;
        //}
        public static string GetDescription(OrderStatus status, string orderCode)
        {
            string description = status switch
            {
                OrderStatus.Order => ORDERDESCRIPTION,
                OrderStatus.Cancelled => CANCELLEDDESCRIPTION,
                OrderStatus.Completed => COMPLETEDDESCRIPTION,
                OrderStatus.Refunded => REFUNDEDDESCRIPTION,
                OrderStatus.Processing => PROCESSINGDESCRIPTION,
                _ => ""
            };
            return description;
        }
        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString() ?? "");
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (_Attribs != null && _Attribs.Count() > 0)
                {
                    return ((DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }

            return GenericEnum.ToString() ?? "";
        }
    }
}
