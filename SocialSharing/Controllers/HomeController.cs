using SocialSharing.Models;
using System.Web.Mvc;
using Facebook;
using System.Net;
using System.IO;
using System;
using System.Configuration;
using Newtonsoft.Json;
using System.Dynamic;
using SocialSharing.Helpers;
using Spring.Social.OAuth1;
using Spring.Social.Twitter.Connect;
using Spring.Social.Twitter.Api;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SocialSharing.Controllers
{
    public class HomeController : Controller
    {
        private MvSocialSharingModel socialSharing = new MvSocialSharingModel { };
        private readonly MvFacebookAuth facebookAuth = new MvFacebookAuth { };
        public MvFacebookResponse facebookResponse = new MvFacebookResponse { };

        private readonly MvTwitterAuth twitterAuth = new MvTwitterAuth { };
        public MvTwitterResponse twitterResponse = new MvTwitterResponse { };
        private IOAuth1ServiceProvider<ITwitter> twitterProvider;
        private IOAuth1Operations twitterOauthOperations;
        private readonly OAuthToken twitterRequestToken;
        private ITwitter twitterClient;

        public MvLinkedInResponse linkedInResponse = new MvLinkedInResponse { };

        public HomeController()
        {
            facebookAuth.ClientId = ConfigurationManager.AppSettings["FacebookClientId"];
            facebookAuth.ClientSecret = ConfigurationManager.AppSettings["FacebookClientSecret"];
            facebookAuth.Scope = ConfigurationManager.AppSettings["FacebookScope"];
            facebookAuth.PageId = ConfigurationManager.AppSettings["FacebookPageId"];
            facebookAuth.RedirectUri = ConfigurationManager.AppSettings["FacebookRedirectUri"];
            facebookAuth.AccessTokenUri = ConfigurationManager.AppSettings["FacebookAccessTokenUri"];
            facebookAuth.AuthorizeUri = ConfigurationManager.AppSettings["FacebookAuthorizeUri"];
            facebookAuth.PageAccessTokenUri = ConfigurationManager.AppSettings["FacebookPageAccessTokenUri"];

            twitterAuth.APIKey = ConfigurationManager.AppSettings["TwitterAPIKey"];
            twitterAuth.APIKeySecret = ConfigurationManager.AppSettings["TwitterAPIKeySecret"];
            twitterAuth.BearerToken = ConfigurationManager.AppSettings["TwitterBearerToken"];
            twitterAuth.RedirectUri = ConfigurationManager.AppSettings["TwitterRedirectUri"];
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SocialSharing()
        {
            return await Task.Run<ActionResult>(async () =>
            {
                socialSharing = (MvSocialSharingModel)(Session["socialSharing"] ?? new MvSocialSharingModel { });
                bool isShared = await ShareToSocialMediaAsync();
                if (isShared)
                {
                    return View();
                }
                else
                {
                    return View("Error");
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SocialShareForm(MvSocialSharingModel socialSharing, string shareTo)
        {
            Session["socialSharing"] = socialSharing;
            Session["shareTo"] = shareTo;
            ViewBag.test = "test success";
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("Index");
            //}

            if (shareTo.Equals("Share To Facebook"))
            {
                FacebookAuth();
            }
            else if (shareTo.Equals("Share To Twitter"))
            {
                TwitterAuth();
            }
            else if (shareTo.Equals("Share To LinkedIn"))
            {
                LinkedInAuth();
            }
            else
            {
                return View("Error");
            }

            return View("SocialSharing");
        }

        public async Task<bool> ShareToSocialMediaAsync()
        {
            if (Session["shareTo"].Equals("Share To Facebook"))
            {
                if (!string.IsNullOrEmpty(Request["code"]?.ToString()))
                {
                    await FacebookAccessTokenAsync();
                    if (!string.IsNullOrEmpty(facebookResponse?.AccessToken))
                    {
                        await FacebookGetPageAccessTokenAsync();
                        if (!string.IsNullOrEmpty(facebookResponse?.PageAccessToken))
                        {
                            await FacebookPostPageWallFeedAsync(socialSharing);
                            if (!string.IsNullOrEmpty(facebookResponse?.NewPageWallPostId))
                            {
                                await FacebookGetPageWallFeedAsync();
                                if (facebookResponse?.PagePost != null || facebookResponse?.PagePost.Count > 0)
                                {
                                    ViewBag.facebookResponse = facebookResponse;
                                    return true;
                                }
                                else
                                {
                                    Response.Write("Error: No Response => FacebookGetPageWallFeedAsync()");
                                    return false;
                                }
                            }
                        }
                        return false;
                    }
                    return false;
                }
                else
                {
                    Response.Write("Error: No code => ShareToSocialMediaAsync()");
                    return false;
                }
            }
            else if (Session["shareTo"].Equals("Share To Twitter"))
            {
                if (!string.IsNullOrEmpty(Request["oauth_token"]?.ToString()))
                {
                    await TwitterClientAsync();
                    if (!string.IsNullOrEmpty(twitterResponse?.AccessToken))
                    {
                        await TwitterPostTweetAsync();
                        if (!string.IsNullOrEmpty(twitterResponse?.NewTweetId))
                        {
                            await TwitterGetTweetsAsync();
                            if (twitterResponse?.Tweets != null || twitterResponse?.Tweets.Count > 0)
                            {
                                ViewBag.twitterResponse = twitterResponse;
                                return true;
                            }
                            else
                            {
                                Response.Write("Error: No Response => TwitterGetTweetsAsync()");
                                return false;
                            }
                        }
                        return false;
                    }
                    return false;
                }
                else
                {
                    Response.Write("Error: No oauth_token (Share To Twitter) => SocialSharing()");
                    return false;
                }
            }
            else if (Session["shareTo"].Equals("Share To LinkedIn"))
            {
                ViewBag.linkedInResponse = linkedInResponse;
                return true;
            }

            return false;
        }

        #region Facebook 
        public void FacebookAuth()
        {
            try
            {
                string authorizeUrl = string.Format(facebookAuth.AuthorizeUri, facebookAuth.ClientId, facebookAuth.RedirectUri, facebookAuth.Scope);
                Response.Redirect(string.Format(authorizeUrl, facebookAuth.ClientId, facebookAuth.RedirectUri, facebookAuth.Scope), false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task FacebookAccessTokenAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    string accessTokenUri = string.Format(facebookAuth.AccessTokenUri, facebookAuth.ClientId, facebookAuth.RedirectUri, facebookAuth.Scope, Request["code"].ToString(), facebookAuth.ClientSecret);

                    HttpWebRequest request = WebRequest.Create(accessTokenUri) as HttpWebRequest;
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        string tokenResponse = reader.ReadToEnd();

                        if (!string.IsNullOrEmpty(tokenResponse?.ToString()))
                        {
                            MvFacebookTokenResponse facebookTokenResponse = JsonConvert.DeserializeObject<MvFacebookTokenResponse>(tokenResponse);

                            facebookResponse = (MvFacebookResponse)(Session["facebookResponse"] ?? new MvFacebookResponse { });
                            facebookResponse.AccessToken = facebookTokenResponse.access_token;
                        }
                        else
                        {
                            Response.Write("Error: No Response => FacebookAccessTokenAsync()");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task FacebookGetPageAccessTokenAsync()
        {
            try
            {
                FacebookClient fbClient = new FacebookClient(facebookResponse?.AccessToken);
                string appsecret_proof = FacebookHelper.GenerateFacebookSecretProof(facebookResponse?.AccessToken, facebookAuth?.ClientSecret); // generate appsecret_proof with user access token

                string pageAccessTokenUrl = facebookAuth?.PageAccessTokenUri.Replace("PageId", facebookAuth?.PageId).Replace("UserAccessToken", facebookResponse?.AccessToken);
                pageAccessTokenUrl = string.Format(pageAccessTokenUrl + "&appsecret_proof={0}", appsecret_proof);
                await fbClient.GetTaskAsync(pageAccessTokenUrl).ContinueWith((response) =>
                {
                    if (!string.IsNullOrEmpty(response?.Result?.ToString()))
                    {
                        MvFacebookPageTokenResponse pageTokenResponse = JsonConvert.DeserializeObject<MvFacebookPageTokenResponse>(response?.Result.ToString());
                        facebookResponse.PageAccessToken = pageTokenResponse?.access_token;
                    }
                    else
                    {
                        Response.Write("Error: No Response => FacebookGetPageAccessTokenAsync()");
                    }
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task FacebookPostPageWallFeedAsync(MvSocialSharingModel message)
        {
            try
            {
                FacebookClient fbClient = new FacebookClient(facebookResponse?.PageAccessToken);
                string appsecret_proof = FacebookHelper.GenerateFacebookSecretProof(facebookResponse?.PageAccessToken, facebookAuth?.ClientSecret); // generate appsecret_proof with user access token

                string pathUrl = string.Format(facebookAuth?.PageId + "/feed?appsecret_proof={0}", appsecret_proof);

                dynamic parameters = new ExpandoObject();
                parameters.message = message?.Description + " => This is Test";
                parameters.caption = "This is caption";
                parameters.attribution = "This is attribution";

                // Link Start
                parameters.picture = "https://blogger.googleusercontent.com/img/a/AVvXsEi8BoZAo5dxXnOqbfj-bH6QnCi-ODva1ee-3WdKnLSv4tMSYCLCoIuQMkE-3jbqO8WkJl-THj9J8rGXX3g4G9rKKIDrt-j6oouF1sGzXOWKJiFqWU0yeIJbEXMUL0PUYSwh8agSK-PviAiX36oscD3FWGmhhxc3PtCbrbVYBzlIxn9SZwaNzkFxjoUi=w702-h395";
                parameters.name = "This is picture name";
                parameters.caption = "This is picture caption";
                parameters.description = "This is description";
                parameters.link = "https://yawahang.blogspot.com/2018/12/symmetric-key-certificate-password.html";
                // Link End

                dynamic result = await fbClient.PostTaskAsync(pathUrl, parameters);
                result = JsonConvert.DeserializeObject(result?.ToString());
                facebookResponse.NewPageWallPostId = result["id"];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task FacebookGetPageWallFeedAsync()
        {
            try
            {
                FacebookClient fbClient = new FacebookClient(facebookResponse?.PageAccessToken);
                string appsecret_proof = FacebookHelper.GenerateFacebookSecretProof(facebookResponse?.PageAccessToken, facebookAuth?.ClientSecret); // generate appsecret_proof with user access token

                string pathUrl = string.Format(facebookAuth?.PageId + "/posts?appsecret_proof={0}", appsecret_proof);

                dynamic parameters = new ExpandoObject();
                parameters.fields = "id,message,picture";

                dynamic result = await fbClient.GetTaskAsync(pathUrl, parameters);

                MvFacebookPageFeedResponse feed = JsonConvert.DeserializeObject<MvFacebookPageFeedResponse>(result.ToString());
                facebookResponse.PagePost = feed.data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Facebook

        #region Twitter 
        public void TwitterAuth()
        {
            try
            {
                twitterProvider = new TwitterServiceProvider(twitterAuth?.APIKey, twitterAuth?.APIKeySecret);
                twitterOauthOperations = twitterProvider.OAuthOperations;
                _ = twitterOauthOperations.FetchRequestTokenAsync(twitterAuth.RedirectUri, null).ContinueWith((response) =>
                  {
                      if (!string.IsNullOrEmpty(response?.Result?.ToString()))
                      {
                          twitterResponse.AccessToken = response?.Result?.Value;
                          twitterResponse.AccessTokenSecret = response?.Result?.Secret;
                          string authorizeUrl = twitterOauthOperations.BuildAuthorizeUrl(response?.Result?.Value, null);
                          Response.Redirect(authorizeUrl, false);
                      }
                      else
                      {
                          Response.Write("Error: No Response => TwitterOAuthToken()");
                      }
                  });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task TwitterClientAsync()
        {
            try
            {
                // upon receiving the callback from the provider   
                string oauthVerifier = "";
                dynamic requestToken = new
                {
                    Value = twitterResponse.AccessToken,
                    Secret = twitterResponse.AccessTokenSecret,
                };

                await twitterOauthOperations.ExchangeForAccessTokenAsync(new AuthorizedRequestToken(requestToken, oauthVerifier), null).ContinueWith((response) =>
                {
                    if (!string.IsNullOrEmpty(response?.Result?.ToString()))
                    {
                        twitterClient = twitterProvider.GetApi(response?.Result?.Value, response?.Result?.Secret);
                    }
                    else
                    {
                        Response.Write("Error: No Response => TwitterAccessToken()");
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task TwitterPostTweetAsync()
        {
            try
            {
                string statusText = "This is a test message that has been published by the Twitter C# SDK. " + DateTime.UtcNow.Ticks.ToString();
                await twitterClient.TimelineOperations.UpdateStatusAsync(statusText).ContinueWith((response) =>
              {
                  if (!string.IsNullOrEmpty(response?.Result?.ToString()))
                  {
                      twitterResponse.Tweets.Add(response.Result);
                      Session["twitterResponse"] = twitterResponse;
                  }
                  else
                  {
                      Response.Write("Error: No Response => TwitterPostTweet()");
                  }
              });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task TwitterGetTweetsAsync()
        {
            try
            {
                dynamic result = await twitterClient.TimelineOperations.GetHomeTimelineAsync();

                IList<Tweet> tweets = JsonConvert.DeserializeObject<IList<Tweet>>(result.ToString());
                twitterResponse.Tweets = tweets;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Twitter

        #region LinkedIn 
        public void LinkedInAuth()
        {
            try
            {

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void LinkedInAccessToken()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LinkedInPost()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion LinkedIn
    }
}