using CoreTweet;
using SocialSharing.Model;
using SocialSharing.Model.Twitter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialSharing.Service.Twitter
{
    public interface ITwitterService
    {
        // Summary: Get AuthorizeUri for twitter
        // 
        // Parameter:
        // twitterAuth => APIKey, APIKeySecret, BearerToken, RedirectUri
        //
        // Returns:
        // MvTwitterAuth
        Task<MvTwitterAuth> AuthorizeUriAsync(MvTwitterAuth twitterAuth);
        // Summary: Get AccessToken for twitter
        // 
        // Parameter:
        // twitterAuth => APIKey, APIKeySecret, BearerToken, RedirectUri  
        //
        // Returns:
        // Tokens
        Task<Tokens> AccessTokenAsync(MvTwitterAuth twitterAuth);
        // Summary: Update tweet status
        // 
        // Parameter:
        // twitterAuth => APIKey, APIKeySecret, BearerToken, RedirectUri
        // tokens => Tokens
        // message => Title, Description (status text for tweet)
        //
        // Returns: tweet id
        // string
        Task<string> PostTweetAsync(MvTwitterAuth twitterAuth, Tokens tokens, MvSocialSharing message);
        // Summary: Get list of tweets
        // 
        // Parameter:
        // twitterAuth => APIKey, APIKeySecret, BearerToken, RedirectUri
        // tokens => Tokens
        //
        // Returns: List of tweets
        // List<MvTweet>
        Task<List<MvTweet>> GetTweetsAsync(MvTwitterAuth twitterAuth, Tokens tokens);
    }
}
