using SocialSharing.Model;
using SocialSharing.Model.Facebook;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialSharing.Service
{
    public interface IFacebookService
    {
        /// <summary>
        /// Generate a facebook secret proof
        /// <seealso cref="http://stackoverflow.com/questions/20572523/c-sharp-help-required-to-create-facebook-appsecret-proof-hmacsha256"/>
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        string GenerateSecretProof(string accessToken, string appSecret);

        Task<MvFacebookToken> AccessTokenAsync(MvFacebookAuth facebookAuth, MvFacebookToken facebookToken, string code);

        Task<MvFacebookToken> GetPageAccessTokenAsync(MvFacebookAuth facebookAuth, MvFacebookToken facebookToken);

        Task<string> PostPageWallFeedAsync(MvFacebookAuth facebookAuth, MvFacebookToken facebookToken, MvSocialSharing message);

        Task<List<MvFacebookPagePost>> GetPageWallFeedAsync(MvFacebookAuth facebookAuth, MvFacebookToken facebookToken);
    }
}
