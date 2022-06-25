using SocialSharing.Model.Twitter;
using System.Threading.Tasks;
using System.Collections.Generic;
using SocialSharing.Model;
using System;
using CoreTweet;
using Newtonsoft.Json;
using System.Dynamic;

namespace SocialSharing.Service.Twitter
{
    public class TwitterService : ITwitterService
    {
        public TwitterService()
        {

        }

        public Task<MvTwitterAuth> AuthorizeUriAsync(MvTwitterAuth twitterAuth)
        {
            try
            {
                OAuth.OAuthSession session = OAuth.Authorize(twitterAuth?.APIKey, twitterAuth?.APIKeySecret, twitterAuth?.RedirectUri);
                //OAuth.OAuthSession session = await OAuth.AuthorizeAsync(twitterAuth?.APIKey, twitterAuth?.APIKeySecret, twitterAuth?.RedirectUri);
                twitterAuth.AuthorizeUrl = session.AuthorizeUri.AbsoluteUri;
                twitterAuth.RequestToken = session.RequestToken;
                twitterAuth.RequestTokenSecret = session.RequestTokenSecret;
                return Task.FromResult(twitterAuth);
                //return twitterToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<Tokens> AccessTokenAsync(MvTwitterAuth twitterAuth)
        {
            try
            {
                OAuth.OAuthSession session = new OAuth.OAuthSession()
                {
                    ConsumerKey = twitterAuth.APIKey,
                    ConsumerSecret = twitterAuth.APIKeySecret,
                    RequestToken = twitterAuth.RequestToken,
                    RequestTokenSecret = twitterAuth.RequestTokenSecret
                };
                Tokens tokens = session.GetTokens(twitterAuth.OauthVerifier);
                return Task.FromResult(tokens);
                //Tokens response = await OAuth.GetTokensAsync(session, twitterAuth.PinCode); 
                //return twitterToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> PostTweetAsync(MvTwitterAuth twitterAuth, Tokens tokens, MvSocialSharing message)
        {
            try
            {
                dynamic parameters = new ExpandoObject();
                parameters.status = message?.Text;
                parameters.status = parameters.status + "\r\n" + message.Link;
                // attachment_url => takes only twitter link
                //parameters.attachment_url = "https://yawahang.blogspot.com/2018/12/symmetric-key-certificate-password.html";

                if (message.ImageData != null)
                {
                    MediaUploadResult image = await tokens.Media.UploadAsync(message.ImageData);
                    parameters.media_ids = new long[] { image.MediaId };
                }

                dynamic result = await tokens.Statuses.UpdateAsync(parameters);
                result = JsonConvert.DeserializeObject(result?.ToString());
                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MvTweet>> GetTweetsAsync(MvTwitterAuth twitterAuth, Tokens tokens)
        {
            try
            {
                List<MvTweet> twitterTweet = new List<MvTweet>();
                foreach (var status in await tokens.Statuses.HomeTimelineAsync(count => 10))
                {
                    twitterTweet.Add(new MvTweet { Id = status.Id, Text = status.Text, UserName = status.User.Name });
                }
                return twitterTweet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
