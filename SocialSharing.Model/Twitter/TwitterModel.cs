using System;

namespace SocialSharing.Model.Twitter
{
    public class MvTwitterAuth
    {
        public string APIKey { get; set; }
        public string APIKeySecret { get; set; }
        public string BearerToken { get; set; }
        public string RedirectUri { get; set; }
        public string OauthVerifier { get; set; } // PinCode or OauthToken 
        public string AuthorizeUrl { get; set; } // redirect url received from twitter Authorize (OAuth.Authorize)
        public string RequestToken { get; set; }
        public string RequestTokenSecret { get; set; }
    }

    public class MvTwitterToken
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }

    public class MvTweet
    {
        // Gets or sets the tweet ID.
        public long Id { get; set; }
        // Gets or sets the tweet message.
        public string Text { get; set; }
        // Gets or sets the tweet created date.
        public DateTime? CreatedAt { get; set; }
        // Gets or sets the tweet author's screen name.
        public string FromUser { get; set; }
        // Gets or sets the tweet author's profile image URL.
        public string ProfileImageUrl { get; set; }
        // Gets or sets the user ID when replying to a user.
        public long? ToUserId { get; set; }
        // Gets or sets the tweet ID when replying to a tweet.
        public long? InReplyToStatusId { get; set; }
        // Gets or sets the tweet author's ID.
        public long FromUserId { get; set; }
        // Gets or sets the tweet's language code. May be null.
        public string LanguageCode { get; set; }
        // Gets or sets the source from where the tweet was send.
        public string Source { get; set; }
        // Gets or sets the username who posted the tweet
        public string UserName { get; set; }
    }

}
