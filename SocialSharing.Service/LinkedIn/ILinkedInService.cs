using SocialSharing.Model;
using SocialSharing.Model.LinkedIn;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialSharing.Service.LinkedIn
{
    public interface ILinkedInService
    {
        Task<MvLinkedInToken> LinkedInAccessTokenAsync(MvLinkedInAuth linkedInAuth, MvLinkedInToken linkedInToken, string code);

        Task<string> CompanyPostAsync(MvLinkedInAuth linkedInAuth, MvLinkedInToken linkedInToken, MvSocialSharing message);

        Task<List<MvLinkedInPost>> GetCompanyPostAsync(MvLinkedInAuth linkedInAuth, MvLinkedInToken linkedInToken);
    }
}
