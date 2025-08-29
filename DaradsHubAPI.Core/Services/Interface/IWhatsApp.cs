using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface
{
    public interface IWhatsAppService
    {
        Task<bool> SendWhatsAppMessage(string phonenumber, string msg, string CountryCode, bool useTemplate = true);
    }
}
