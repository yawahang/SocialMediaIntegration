using System.Collections.Generic;

namespace SocialSharing.Model.Facebook
{
    public class MvFacebookAuth
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthorizeUri { get; set; }
        public string Scope { get; set; }
        public string AccessTokenUri { get; set; }
        public string PageAccessTokenUri { get; set; }
        public string RedirectUri { get; set; }
        public string PageId { get; set; }
    }

    public class MvFacebookToken
    {
        public string AccessToken { get; set; }
        public string PageAccessToken { get; set; }
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
}
