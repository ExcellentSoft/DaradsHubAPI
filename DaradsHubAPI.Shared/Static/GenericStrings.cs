using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Shared.Static
{
    public class GenericStrings
    {
        public readonly static string InvalidEmail = "Invalid email address";
        public const string DATE_FORMAT = "MMMM d, yyyy";
        public const string TIME_FORMAT = "hh:mm tt";
        public const string PROFILE_IMAGES_FOLDER_NAME = "profile_images";
        public const string PRODUCT_IMAGES_FOLDER_NAME = "product_images";
        public const string CATEGORY_IMAGES_FOLDER_NAME = "categories_images";
        public const string ORDERDESCRIPTION = "Your product has been placed successfully";
        public const string SHIPPEDESCRIPTION = "Your product has been shipped successfully";
        public const string DELIVEREDDESCRIPTION = "Your product has been delivered successfully";

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
    }
}
