
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darads.CoreInfrastruture.Persistence.IIntegration
{
    public interface IPaymentIOService
    {
        //create virtual accounts
        //Confirm payment via Webhook 
        Task<ApiResponse<Bankaccount2>> CreateVirtualAccount(string userEmail, string accessFrom);
        /* 
         Flow
        //create method in Customer-Service or Controller to call this service (Platform e.g Wb, TBot, WBot)

        use user Id to get user details and create param for this method
        Call this endpoint and when successful
        Save it response in respect to userstb, save virtual account number or into new table VirtualAccount Numbers with user id
        return Acount Number to user to copy or save it
        Inside that customer service - Add method to get user virtual account details
        GetPaymentTrans(Virtual account)- all paymnent confirm via webhook
        Delete Virtual Account if not needed again

        Implement Controller on Darad Website-customer dashboard

         */
       // Task<object> UpdateVirtualAccount(object param);// if requied
        Task ConfirmPaymentWebhook(PayIOWebhookPayload param); //To credit user Wallet balance when payment successful 
    }
}
