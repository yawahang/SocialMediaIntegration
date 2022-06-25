using CoreTweet;
using SocialSharing.Model;
using SocialSharing.Model.Facebook;
using SocialSharing.Model.LinkedIn;
using SocialSharing.Model.Twitter;
using SocialSharing.Service.LinkedIn;
using SocialSharing.Service.Twitter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialSharing.Service.SocialSharing
{
    public class SocialSharingService : ISocialSharingService
    {
        private readonly IFacebookService _fbs;
        private readonly ITwitterService _ts;
        private readonly ILinkedInService _ls;

        public SocialSharingService(IFacebookService fbs, ITwitterService ts, ILinkedInService ls)
        {
            _fbs = fbs;
            _ts = ts;
            _ls = ls;
        }

        public async Task<List<MvFacebookPagePost>> FacebookPageShareAsync(MvFacebookAuth facebookAuth, string code, MvSocialSharing message)
        {
            try
            {
                MvFacebookToken facebookToken = new MvFacebookToken { };
                List<MvFacebookPagePost> facebookPagePost = new List<MvFacebookPagePost>();

                facebookToken = await _fbs.AccessTokenAsync(facebookAuth, facebookToken, code);

                if (!string.IsNullOrEmpty(facebookToken?.AccessToken))
                {
                    facebookToken = await _fbs.GetPageAccessTokenAsync(facebookAuth, facebookToken);
                    if (!string.IsNullOrEmpty(facebookToken?.PageAccessToken))
                    {
                        string pageWallPostId = await _fbs.PostPageWallFeedAsync(facebookAuth, facebookToken, message);
                        if (!string.IsNullOrEmpty(pageWallPostId))
                        {
                            facebookPagePost = await _fbs.GetPageWallFeedAsync(facebookAuth, facebookToken);
                        }
                    }
                }
                return facebookPagePost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<MvTwitterAuth> TwitterAuthorizeUrlAsync(MvTwitterAuth twitterAuth)
        {
            try
            {
                twitterAuth = _ts.AuthorizeUriAsync(twitterAuth).Result;
                return Task.FromResult(twitterAuth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MvTweet>> TwitterShareAsync(MvTwitterAuth twitterAuth, MvSocialSharing message)
        {
            try
            {
                List<MvTweet> twitterTweet = new List<MvTweet>();

                Tokens tokens = await _ts.AccessTokenAsync(twitterAuth);

                if (!string.IsNullOrEmpty(tokens?.AccessToken))
                {
                    string tweetId = await _ts.PostTweetAsync(twitterAuth, tokens, message);
                    if (!string.IsNullOrEmpty(tweetId))
                    {
                        twitterTweet = await _ts.GetTweetsAsync(twitterAuth, tokens);
                    }
                }
                return twitterTweet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MvLinkedInPost>> LinkedInShareAsync(MvLinkedInAuth linkedInAuth, string code, MvSocialSharing message)
        {
            try
            {
                List<MvLinkedInPost> linkedInPost = new List<MvLinkedInPost>();
                MvLinkedInToken linkedInToken = new MvLinkedInToken { };

                linkedInToken = await _ls.LinkedInAccessTokenAsync(linkedInAuth, linkedInToken, code);
                if (!string.IsNullOrEmpty(linkedInToken?.AccessToken))
                {
                    string pageWallPostId = await _ls.CompanyPostAsync(linkedInAuth, linkedInToken, message);
                    if (!string.IsNullOrEmpty(pageWallPostId))
                    {
                        linkedInPost = await _ls.GetCompanyPostAsync(linkedInAuth, linkedInToken);
                    }
                }
                return linkedInPost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
