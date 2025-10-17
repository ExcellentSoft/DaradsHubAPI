
using Darads.CoreInfrastruture.Persistence.IIntegration;
using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Models;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Customs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Darads.Services.APILiveServices.IntegrationService
{
    public class PaymentIOService : IPaymentIOService
    {
        //Todo now 
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _clientFactory;
        // private readonly IUnitOfWork _unitOfWork;
        public IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public PaymentIOService(IOptions<AppSettings> settings, IUnitOfWork _unitOfWork, IEmailService emailService, IHttpClientFactory clientFactory, AppDbContext context) {

            _appSettings = settings.Value;
            _clientFactory = clientFactory;
            _context = context;
            _emailService = emailService;
        }
        public async Task ConfirmPaymentWebhook(PayIOWebhookPayload param)
        {
            //Verify payload and credit user Wallet
            webhookLog wlog = new()
            {
                LogDate = GetLocalDateTime.CurrentDateTime(),
                Create_at = GetLocalDateTime.CurrentDateTime(),
                WebHookEvent = "Webhook Received-PAYMENT-IO-Wallet",
                Status = param.TransactionStatus,
                EventType = param.NotificationStatus,
                payment_type = "Virtual-Payment-Credit",
                tx_ref = param.TransactionId,
                narration = param.Description,
                CustomerEmail = param.Customer.Email,
                CustomerCode = param.Customer.CustomerId,
                ErrorMsg = "",
                TransId = param.TransactionId,
                Amount = param.AmountPaid,
                Pgateway = "PAYMENTIO",
                WalletUpdate = "PAYMENT-IO-Response-Received-Pay Through: Bank- " + param.Sender.Bank + " AccountNumber" + param.Sender.AccountNumber,
                WalletNotUpdateReason = "PAYMENT-IO"
            };
            var accExist = await _context.CustomerVirtualAccounts.Where(c => c.AcctountNumber == param.Receiver.AccountNumber &&  c.Gateway == "PaymentIO").FirstOrDefaultAsync();
            if (accExist != null)
            { 
                //Account Number Exist
                var getUser= await _context.userstb.Where(c=>c.id==accExist.UserId).FirstOrDefaultAsync();
                if (getUser != null)
                {
                    var userWallet = await _unitOfWork.Users.GetWallet(getUser.email);
                    var transaction = await _context.GwalletTrans
                   .Where(gw => gw.areaCode == param.TransactionId && gw.Status == "C" && gw.transMedium == "VirtualPayIO")
                   .FirstOrDefaultAsync();

                    if (transaction is null)
                    {
                        var reference = CustomizeCodes.GetUniqueString(10);
                        using var dbTransaction = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            userWallet.Balance += Convert.ToDecimal(param.AmountPaid);
                            userWallet.LastfundAmt = Convert.ToDecimal(param.AmountPaid);
                            userWallet.UpdateDate = GetLocalDateTime.CurrentDateTime_();
                            await _context.SaveChangesAsync();
                            var newTrans = new GwalletTran
                            {
                                walletBal = userWallet.Balance,
                                amt = Convert.ToDecimal(param.AmountPaid),
                                userName = getUser.email,
                                areaCode = param.TransactionId,
                                CR = Convert.ToDecimal(param.AmountPaid),
                                refNo = reference,

                                transdate = GetLocalDateTime.CurrentDateTime(),
                                Status = "C",
                                transStatus = "C",
                                transType = "FundWallet",
                                transMedium = "VirtualPayIO",
                            };
                            await _unitOfWork.Users.SaveNewTransaction(newTrans);
                            await _context.SaveChangesAsync();
                            await dbTransaction.CommitAsync();
                            var alertEmail=getUser.email;
                            if (string.IsNullOrEmpty(alertEmail))
                            {
                                alertEmail = getUser.AlertEmail;
                            }
                            string message = $"Dear {getUser.fullname },{getUser.FirstName} your wallet has been successfully credited.via your PAYMENTIO virtual account number payment" + $" Please refer to the transaction history below for details." + $" <br/><br/> Amount Funded: {param.AmountPaid:N}" + $" <br/><br/> Wallet Balance: {userWallet.Balance:N}";

                            _emailService.SendMail(alertEmail, "Wallet Funded Successfully.", message, "Darads");
                        }
                        catch (Exception)
                        {
                            await dbTransaction.RollbackAsync();
                        }
                    }

                }

                //getWallet
                //Update Wallet
                //Save Transaction-CR

            }
            _context.webhookLog.Add(wlog);
            await _context.SaveChangesAsync();
        }

        public async Task<ApiResponse<Bankaccount2>> CreateVirtualAccount(string userEmail,string accessFrom)
        {
            //UserEmail, accessFrom
            var resData = new Bankaccount2
            {
                Message = "Invalid ! User credential not found."
            };
            //call endpoint, take response and Save it to database 
            var getUser= await _context.userstb.Where(x=>x.email == userEmail || x.AlertEmail==userEmail).FirstOrDefaultAsync();
            if (getUser is null) {

                return new ApiResponse<Bankaccount2> { Data = resData, Status = false, Message = "Invalid ! User credential not found." };
            }
            var param = new CreateVirtualAccParam
            {
                bankCode= new List<string> { "20946"},
                businessId = _appSettings.PaymentioBusinessId,
                email =getUser.email, phoneNumber =getUser.phone ,
                name =getUser.FirstName + " " + getUser.LastName ,  
               
        };
            
            string apiUrl = "https://api.paymentpoint.co/api/v1/createVirtualAccount";
            var client = _clientFactory.CreateClient();
            
            HttpContent httpContent = new StringContent(JsonSerializer.Serialize(param), Encoding.UTF8, "application/json");
            HttpRequestMessage mRequest = new(HttpMethod.Post, apiUrl)
            {
                Content = httpContent
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _appSettings.PaymentioApiSecret);
            if (!client.DefaultRequestHeaders.Contains("api-key"))
            {
                client.DefaultRequestHeaders.Add("api-key", _appSettings.PaymentioAPIKey);
            }
            var response = await client.SendAsync(mRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<PaymentIOApiResponse>(responseString);
                if (data != null)
                {
                    if (data.status == "success")
                    {
                        //Save info for the client

                        foreach (var dt in data.bankAccounts)
                        {
                            var accExist = await _context.CustomerVirtualAccounts.Where(c => c.UserId == getUser.id && c.BankCode == dt.bankCode && c.Gateway == "PaymentIO").FirstOrDefaultAsync();
                            if (accExist != null)
                            {
                                resData.accountName = accExist.AcctountName;
                                resData.accountNumber= accExist.AcctountNumber;
                                resData.bankName= accExist.BankName;
                                return new ApiResponse<Bankaccount2> { Data = resData, Status = true, Message = $"Your virtual account Number is  {accExist.AcctountNumber} " };

                            }
                            var va = new CustomerVirtualAccount 
                            {
                                AcctountName = dt.accountName,
                                AcctountNumber = dt.accountNumber,
                                BankName = dt.bankName,
                                BankCode = dt.bankCode,
                                Status = 1,
                                UserId = getUser.id,//get user Id with user email address
                                TrackingId = dt.Reserved_Account_Id,
                                TrackingRef = dt.Reserved_Account_Id,
                                TimeCreated = GetLocalDateTime.CurrentDateTime(),
                                CreatedFrom = accessFrom,
                                Gateway = "PaymentIO"
                            };


                            await _context.CustomerVirtualAccounts.AddAsync(va);
                            await _context.SaveChangesAsync();
                        }
                        //  return $"New virtual account Number: {data.bankAccounts.First().accountNumber} Created successfully";
                        resData.accountName = data.bankAccounts.First().accountName;
                        resData.accountNumber = data.bankAccounts.First().accountNumber;
                        resData.bankName = data.bankAccounts.First().bankName;
                        return new ApiResponse<Bankaccount2> { Data = resData, Status =true, Message = $"New virtual account Number: {data.bankAccounts.First().accountNumber} Created successfully" };

                    }
                    else
                    {resData.Message = $"Virtual account creation failed-{data.status}. Try it again ";

                      return  new ApiResponse<Bankaccount2> { Data = resData, Status = false, Message = $"Virtual account creation failed-{data.status}. Try it again" };
           

                    }


                }


                else
                {


                    return new ApiResponse<Bankaccount2> { Data = resData, Status = false, Message = $"Virtual account creation failed-Null data response. Try it again" };
                }
 
            }
            else
            {
                var res = await response.Content.ReadAsStringAsync();
                return new ApiResponse<Bankaccount2> {Data = resData, Status = false, Message = $"Virtual account creation failed-Null data response. Try it again" };

               
            }
        }
    }
}
