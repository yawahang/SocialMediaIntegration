using System.Collections.Generic;

namespace SocialSharing.Model.LinkedIn
{
    public class MvLinkedInAuth
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthorizeUri { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
        public string AccessTokenUri { get; set; }
        public string RedirectUri { get; set; }
    }

    public class MvLinkedInToken
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }

    public class MvLinkedInTokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public class MvLinkedInPost
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }

    // LinkedInPostRequest 
    public class LinkedInPostRequest
    {
        public LinkedInPostContent content { get; set; }
        public LinkedInPostDistribution distribution { get; set; }
        public string owner { get; set; }
        public string subject { get; set; }
        public LinkedInPostText text { get; set; }
    }

    public class LinkedInPostContent
    {
        public List<LinkedInPostContentEntity> contentEntities { get; set; }
        public string title { get; set; }
    }

    public class LinkedInPostThumbnail
    {
        public string resolvedUrl { get; set; }
    }

    public class LinkedInPostContentEntity
    {
        public string entityLocation { get; set; }
        public List<LinkedInPostThumbnail> thumbnails { get; set; }
    }

    public class LinkedInDistributionTarget
    {
    }

    public class LinkedInPostDistribution
    {
        public LinkedInDistributionTarget linkedInDistributionTarget { get; set; }
    }

    public class LinkedInPostText
    {
        public string text { get; set; }
    }
    // LinkedInPostRequest 
}
