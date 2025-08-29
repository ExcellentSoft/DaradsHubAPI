using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Core.Models;

namespace DaradsHubAPI.Core.Services.Concrete
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly AppSettings _appSettings;
        private string WhatsAPIKEY;
        public HttpClient? Client;
        public WhatsAppService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            WhatsAPIKEY = _appSettings.WhatsAppKey;

        }
        public async Task<bool> SendWhatsAppMessage(string phonenumber, string msg, string CountryCode, bool useTemplate = true)
        {
            //like boosting of all 
            // social media accounts, selling of all social media account, boosting of WhatsApp views.All countries number’s verification, trading of all kinds of cryptocurrencies and giftcards, we also sell Chinese yuan.

            var adsEm = @$"
Visit  https://solo.to/Dartechlabs   to explore more of our website features, products and services.

Check out on https://solo.to/Dartechlabs for Quick access.";
            if (useTemplate)
                msg += adsEm;
            try
            {
                Client = new HttpClient();
                if (phonenumber.Substring(0, 1) == "0")
                {
                    phonenumber = phonenumber.Remove(0, 1);
                }

                var usernumber = CountryCode + phonenumber;
                string uri = _appSettings.WhatsAppUri;
                WhatsappParam whatsappcode = new WhatsappParam
                {
                    Type = "text",
                    ToNumber = usernumber,
                    Message = msg

                };
                var jsonwhatsapp = JsonConvert.SerializeObject(whatsappcode);
                var content = new StringContent(jsonwhatsapp, System.Text.Encoding.UTF8, "application/json");

                // Adding authentication header
                Client.DefaultRequestHeaders.Add("x-maytapi-key", WhatsAPIKEY);

                var response = await Client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    var responseDetails = WhatsappResponseDto.FromJson(responseData);

                    if (responseDetails.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}