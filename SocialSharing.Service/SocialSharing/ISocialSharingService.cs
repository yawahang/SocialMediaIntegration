using SocialSharing.Model;
using SocialSharing.Model.Facebook;
using SocialSharing.Model.LinkedIn;
using SocialSharing.Model.Twitter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialSharing.Service.SocialSharing
{
    public interface ISocialSharingService
    {
        Task<List<MvFacebookPagePost>> FacebookPageShareAsync(MvFacebookAuth facebookAuth, string code, MvSocialSharing message);

        // Summary: Get AuthorizeUri for twitter
        // 
        // Parameter:
        // twitterAuth => APIKey, APIKeySecret, BearerToken, RedirectUri
        //
        // Returns:
        // MvTwitterAuth
        Task<MvTwitterAuth> TwitterAuthorizeUrlAsync(MvTwitterAuth twitterAuth);

        Task<List<MvTweet>> TwitterShareAsync(MvTwitterAuth twitterAuth, MvSocialSharing message);

        Task<List<MvLinkedInPost>> LinkedInShareAsync(MvLinkedInAuth linkedInAuth, string code, MvSocialSharing message);

    }
}
