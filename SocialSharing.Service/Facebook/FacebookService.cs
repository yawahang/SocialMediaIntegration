using Facebook;
using Newtonsoft.Json;
using SocialSharing.Model;
using SocialSharing.Model.Facebook;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocialSharing.Service.Facebook
{
    public class FacebookService : IFacebookService
    {
        public FacebookService()
        {

        }

        public string GenerateSecretProof(string accessToken, string appSecret)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(appSecret);
                byte[] messageBytes = Encoding.UTF8.GetBytes(accessToken);
                HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);
                byte[] hash = hmacsha256.ComputeHash(messageBytes);
                StringBuilder sbHash = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                {
                    _ = sbHash.Append(hash[i].ToString("x2"));
                }

                return sbHash.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MvFacebookToken> AccessTokenAsync(MvFacebookAuth facebookAuth, MvFacebookToken facebookToken, string code)
        {
            try
            {
                string accessTokenUri = string.Format(facebookAuth.AccessTokenUri, facebookAuth.ClientId, facebookAuth.RedirectUri, facebookAuth.Scope, code, facebookAuth.ClientSecret);

                HttpWebRequest request = WebRequest.Create(accessTokenUri) as HttpWebRequest;
                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string tokenResponse = reader.ReadToEnd();

                    if (!string.IsNullOrEmpty(tokenResponse?.ToString()))
                    {
                        MvFacebookTokenResponse facebookTokenResponse = JsonConvert.DeserializeObject<MvFacebookTokenResponse>(tokenResponse);
                        facebookToken.AccessToken = facebookTokenResponse.access_token;
                    }

                    return facebookToken;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MvFacebookToken> GetPageAccessTokenAsync(MvFacebookAuth facebookAuth, MvFacebookToken facebookToken)
        {
            try
            {
                FacebookClient fbClient = new FacebookClient(facebookToken?.AccessToken);
                // generate appsecret_proof with user access token
                string appsecret_proof = this.GenerateSecretProof(facebookToken?.AccessToken, facebookAuth?.ClientSecret);

                string pageAccessTokenUrl = facebookAuth?.PageAccessTokenUri.Replace("PageId", facebookAuth?.PageId).Replace("UserAccessToken", facebookToken?.AccessToken);
                pageAccessTokenUrl = string.Format(pageAccessTokenUrl + "&appsecret_proof={0}", appsecret_proof);
                dynamic response = await fbClient.GetTaskAsync(pageAccessTokenUrl);
                if (!string.IsNullOrEmpty(response?.ToString()))
                {
                    MvFacebookPageTokenResponse pageTokenResponse = JsonConvert.DeserializeObject<MvFacebookPageTokenResponse>(response?.ToString());
                    facebookToken.PageAccessToken = pageTokenResponse?.access_token;
                }

                return facebookToken;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<string> PostPageWallFeedAsync(MvFacebookAuth facebookAuth, MvFacebookToken facebookToken, MvSocialSharing message)
        {
            try
            {
                FacebookClient fbClient = new FacebookClient(facebookToken?.PageAccessToken);
                // generate appsecret_proof with user access token
                string appsecret_proof = this.GenerateSecretProof(facebookToken?.PageAccessToken, facebookAuth?.ClientSecret);

                string pathUrl = string.Format(facebookAuth?.PageId + "/feed?appsecret_proof={0}", appsecret_proof);

                dynamic parameters = new ExpandoObject();
                parameters.message = message?.Text;

                if (!string.IsNullOrEmpty(message.PictureLink))
                {
                    parameters.picture = message.PictureLink;
                    parameters.name = message.PictureName;
                    parameters.caption = message.PictureCaption;
                    parameters.description = message.PictureDescription;
                }

                if (!string.IsNullOrEmpty(message.Link))
                {
                    parameters.link = message.Link;
                }

                dynamic result = await fbClient.PostTaskAsync(pathUrl, parameters);
                result = JsonConvert.DeserializeObject(result?.ToString());
                return result["id"];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MvFacebookPagePost>> GetPageWallFeedAsync(MvFacebookAuth facebookAuth, MvFacebookToken facebookToken)
        {
            try
            {
                FacebookClient fbClient = new FacebookClient(facebookToken?.PageAccessToken);
                // generate appsecret_proof with user access token
                string appsecret_proof = GenerateSecretProof(facebookToken?.PageAccessToken, facebookAuth?.ClientSecret);

                string pathUrl = string.Format(facebookAuth?.PageId + "/posts?appsecret_proof={0}", appsecret_proof);

                dynamic parameters = new ExpandoObject();
                parameters.fields = "id,message,picture";

                dynamic result = await fbClient.GetTaskAsync(pathUrl, parameters);

                MvFacebookPageFeedResponse feed = JsonConvert.DeserializeObject<MvFacebookPageFeedResponse>(result.ToString());
                return feed?.data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}