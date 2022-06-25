using System.Web.Mvc;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using SocialSharing.Model;
using SocialSharing.Model.Facebook;
using SocialSharing.Model.Twitter;
using SocialSharing.Model.LinkedIn;
using SocialSharing.Service.SocialSharing;
using System;

namespace SocialSharing.Controllers
{
    public class HomeController : Controller
    {
        private MvSocialSharing socialSharing = new MvSocialSharing { };

        private readonly ISocialSharingService _ss;
        private MvFacebookAuth facebookAuth = new MvFacebookAuth { };
        public MvFacebookToken facebookToken = new MvFacebookToken { };
        public List<MvFacebookPagePost> facebookPagePost = new List<MvFacebookPagePost>();

        private MvTwitterAuth twitterAuth = new MvTwitterAuth { };
        public MvTwitterToken twitterToken = new MvTwitterToken { };
        public List<MvTweet> twitterTweet = new List<MvTweet>();

        private MvLinkedInAuth linkedInAuth = new MvLinkedInAuth { };
        public MvLinkedInToken linkedInToken = new MvLinkedInToken { };
        public List<MvLinkedInPost> linkedInPost = new List<MvLinkedInPost>();

        public HomeController(ISocialSharingService ss)
        {
            _ss = ss;

            // Facebook Config
            facebookAuth.ClientId = ConfigurationManager.AppSettings["FacebookClientId"];
            facebookAuth.ClientSecret = ConfigurationManager.AppSettings["FacebookClientSecret"];
            facebookAuth.Scope = ConfigurationManager.AppSettings["FacebookScope"];
            facebookAuth.PageId = ConfigurationManager.AppSettings["FacebookPageId"];
            facebookAuth.RedirectUri = ConfigurationManager.AppSettings["FacebookRedirectUri"];
            facebookAuth.AccessTokenUri = ConfigurationManager.AppSettings["FacebookAccessTokenUri"];
            facebookAuth.AuthorizeUri = ConfigurationManager.AppSettings["FacebookAuthorizeUri"];
            facebookAuth.PageAccessTokenUri = ConfigurationManager.AppSettings["FacebookPageAccessTokenUri"];
            // Twitter Config
            twitterAuth.APIKey = ConfigurationManager.AppSettings["TwitterAPIKey"];
            twitterAuth.APIKeySecret = ConfigurationManager.AppSettings["TwitterAPIKeySecret"];
            twitterAuth.BearerToken = ConfigurationManager.AppSettings["TwitterBearerToken"];
            twitterAuth.RedirectUri = ConfigurationManager.AppSettings["TwitterRedirectUri"];
            twitterToken.AccessToken = ConfigurationManager.AppSettings["TwitterAccessToken"];
            twitterToken.AccessTokenSecret = ConfigurationManager.AppSettings["TwitterAccessTokenSecret"];
            // LinkedIn Config
            linkedInAuth.ClientId = ConfigurationManager.AppSettings["LinkedInClientId"];
            linkedInAuth.ClientSecret = ConfigurationManager.AppSettings["LinkedInClienSecret"];
            linkedInAuth.AuthorizeUri = ConfigurationManager.AppSettings["LinkedInAuthorizeUri"];
            linkedInAuth.State = ConfigurationManager.AppSettings["LinkedInState"];
            linkedInAuth.Scope = ConfigurationManager.AppSettings["LinkedInScope"];
            linkedInAuth.AccessTokenUri = ConfigurationManager.AppSettings["LinkedInAccessTokenUri"];
            linkedInAuth.RedirectUri = ConfigurationManager.AppSettings["LinkedInRedirectUri"];
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SocialSharing()
        {
            socialSharing = (MvSocialSharing)(Session["socialSharing"] ?? new MvSocialSharing { });
            bool isShared = await ShareToSocialMediaAsync();
            if (isShared)
            {
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult SocialShareForm(MvSocialSharing socialSharing)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
                // Image Share 
                if (socialSharing.Image != null && socialSharing.Image.ContentLength > 0)
                {
                    //byte[] fileInBytes = new byte[socialSharing.Image.ContentLength];
                    //using (BinaryReader reader = new BinaryReader(socialSharing.Image.InputStream))
                    //{
                    //    fileInBytes = reader.ReadBytes(socialSharing.Image.ContentLength);
                    //}
                    //string fileAsString = Convert.ToBase64String(fileInBytes);
                    byte[] image = new byte[socialSharing.Image.ContentLength];
                    socialSharing.Image.InputStream.Read(image, 0, image.Length);
                    socialSharing.ImageData = image;
                }
                string shareTo = socialSharing.ShareTo;
                Session["socialSharing"] = socialSharing;
                Session["shareTo"] = shareTo;

                string authorizeUrl;
                if (shareTo.Equals("Share To Facebook"))
                {
                    authorizeUrl = string.Format(facebookAuth.AuthorizeUri, facebookAuth.ClientId, facebookAuth.RedirectUri, facebookAuth.Scope);
                    Response.Redirect(string.Format(authorizeUrl, facebookAuth.ClientId, facebookAuth.RedirectUri, facebookAuth.Scope), false);
                }
                else if (shareTo.Equals("Share To Twitter"))
                {
                    twitterAuth = _ss.TwitterAuthorizeUrlAsync(twitterAuth).Result;
                    if (!string.IsNullOrEmpty(twitterAuth.AuthorizeUrl))
                    {
                        Session["requestToken"] = twitterAuth.RequestToken;
                        Session["requestTokenSecret"] = twitterAuth.RequestTokenSecret;
                        Response.Redirect(twitterAuth.AuthorizeUrl, false);
                    }
                    else
                    {
                        Response.Write("Error: No Response => TwitterAuthorizeUriAsync()");
                    }
                }
                else if (shareTo.Equals("Share To LinkedIn"))
                {
                    authorizeUrl = string.Format(linkedInAuth.AuthorizeUri, linkedInAuth.ClientId, linkedInAuth.RedirectUri, linkedInAuth.State, linkedInAuth.Scope);
                    Response.Redirect(authorizeUrl, false);
                }
                else
                {
                    return View("Error");
                }

                return View("SocialSharing");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ShareToSocialMediaAsync()
        {
            try
            {
                string shareTo = Session["shareTo"]?.ToString();
                string verifier;
                if (shareTo.Equals("Share To Facebook"))
                {
                    verifier = Request["code"]?.ToString();
                    if (!string.IsNullOrEmpty(verifier))
                    {
                        facebookPagePost = await _ss.FacebookPageShareAsync(facebookAuth, verifier, socialSharing);
                        if (facebookPagePost != null || facebookPagePost.Count > 0)
                        {
                            ViewBag.facebookPagePost = facebookPagePost;
                            return true;
                        }
                        else
                        {
                            Response.Write("Error: No Response => FacebookGetPageWallFeedAsync()");
                            return false;
                        }
                    }
                    else
                    {
                        Response.Write("Error: No verifier => ShareToSocialMediaAsync()");
                        return false;
                    }
                }
                else if (shareTo.Equals("Share To Twitter"))
                {
                    verifier = Request["oauth_verifier"]?.ToString();

                    if (!string.IsNullOrEmpty(verifier))
                    {
                        twitterAuth.OauthVerifier = verifier;
                        twitterAuth.RequestToken = Session["requestToken"]?.ToString();
                        twitterAuth.RequestTokenSecret = Session["requestTokenSecret"]?.ToString();
                        twitterTweet = await _ss.TwitterShareAsync(twitterAuth, socialSharing);
                        if (twitterTweet != null || twitterTweet.Count > 0)
                        {
                            ViewBag.twitterTweet = twitterTweet;
                            return true;
                        }
                        else
                        {
                            Response.Write("Error: No Response => ShareToTwitterAsync()");
                            return false;
                        }
                    }
                    else
                    {
                        Response.Write("Error: No verifier => ShareToSocialMediaAsync()");
                        return false;
                    }
                }
                else if (shareTo.Equals("Share To LinkedIn"))
                {
                    verifier = Request["code"]?.ToString();
                    if (!string.IsNullOrEmpty(verifier))
                    {
                        linkedInPost = await _ss.LinkedInShareAsync(linkedInAuth, verifier, socialSharing);
                        if (linkedInPost != null || linkedInPost.Count > 0)
                        {
                            ViewBag.linkedInPost = linkedInPost;
                            return true;
                        }
                        else
                        {
                            Response.Write("Error: No Response => LinkedInShareAsync()");
                            return false;
                        }
                    }
                    else
                    {
                        Response.Write("Error: No verifier => ShareToSocialMediaAsync()");
                        return false;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}