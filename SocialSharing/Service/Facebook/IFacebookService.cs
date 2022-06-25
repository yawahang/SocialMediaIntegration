using System.Threading.Tasks;

namespace SocialSharing.Service
{
    public interface IFacebookService
    {
        Task<string> GetAccessToken(string code);
    }
}
