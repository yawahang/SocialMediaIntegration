using Spring.Social.Twitter.Api;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace SocialSharing.Models
{
    public class MvSocialSharingModel
    {
        [Required(ErrorMessage = "Title is required")]
        [MinLength(5, ErrorMessage = "Title must have atleast five characters")]
        [MaxLength(200, ErrorMessage = "Title should not exceed 200 characters")]
        public string Title { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        [MinLength(5, ErrorMessage = "Description must have atleast five characters")]
        [MaxLength(500, ErrorMessage = "Description should not exceed 500 characters")]
        public string Description { get; set; }
    }

    #region Facebook
    public class MvFacebookAuth
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string PageId { get; set; }
        public string RedirectUri { get; set; }
        public string AccessTokenUri { get; set; }
        public string AuthorizeUri { get; set; }
        public string PageAccessTokenUri { get; set; }
    }
    public class MvFacebookResponse
    {
        public string AccessToken { get; set; }
        public string PageAccessToken { get; set; }
        public string NewWallPostId { get; set; }
        public string NewPageWallPostId { get; set; }
        public List<MvFacebookPagePost> PagePost { get; set; }
    }

    public class MvFacebookPagePost
    {
        public string id { get; set; }
        public string message { get; set; }
    }

    public class MvFacebookPageFeedResponse
    {
        public List<MvFacebookPagePost> data { get; set; }
        public dynamic paging { get; set; }
    }

    public class MvFacebookTokenResponse
    {
        //[ModelBinder(Name = "access_token"])
        //[JsonPropertyName("access_token")]
        //public string AccessToken { get; set; }
        //[JsonPropertyName("token_type")]
        //public string TokenType { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
    }

    public class MvFacebookPageTokenResponse
    {
        //[JsonPropertyName("id")]
        //public string Id { get; set; } // page Id
        //[JsonPropertyName("access_token")]
        //public string AccessToken { get; set; }
        public string id { get; set; }
        public string access_token { get; set; }
    }
    #endregion Facebook

    #region Twitter
    public class MvTwitterAuth
    {
        public string APIKey { get; set; }
        public string APIKeySecret { get; set; }
        public string BearerToken { get; set; }
        public string RedirectUri { get; set; }
    }

    public class MvTwitterResponse
    {
        [Display(Name = "Twitter Access Token")]
        public string AccessToken { get; set; }
        [Display(Name = "Twitter Access Token Secret")]
        public string AccessTokenSecret { get; set; }
        [Display(Name = "Twitter Tweets")]
        public IList<Tweet> Tweets { get; set; }
        public string NewTweetId { get; set; }
    }
    #endregion Twitter

    #region LinkedIn
    public class MvLinkedInResponse
    {
        [Display(Name = "LinkedIn Access Token")]
        public string AccessToken { get; set; }
        [Display(Name = "LinkedIn Post")]
        public List<MvLinkedInInPost> LinkedInPost { get; set; }
    }

    public class MvLinkedInInPost
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }
    public class MvLinkedInAuth
    {
        public string RedirectUri { get; set; }
    }
    #endregion LinkedIn 
}