using SocialSharing.Model.Facebook;
using SocialSharing.Model.LinkedIn;
using SocialSharing.Model.Twitter;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SocialSharing.Model
{
    public class MvSocialSharing
    {
        [Display(Name = "Enter Something To Share")]
        [Required(ErrorMessage = "Text is required")]
        public string Text { get; set; }
        [Display(Name = "Picture Link (Facebook)")]
        public string PictureLink { get; set; }
        [Display(Name = "Picture Name (Facebook)")]
        public string PictureName { get; set; }
        [Display(Name = "Picture Caption (Facebook)")]
        public string PictureCaption { get; set; }
        [Display(Name = "Picture Description (Facebook)")]
        public string PictureDescription { get; set; }
        [Display(Name = "Link")]
        public string Link { get; set; }
        [Display(Name = "Select Image (For Twitter)")]
        public HttpPostedFileBase Image { get; set; }
        public IEnumerable<byte> ImageData { get; set; }
        public string ShareTo { get; set; }
    }

    public class MvSocialSharingResponse
    {
        public List<MvFacebookPagePost> FacebookPagePost { get; set; }
        public List<MvTweet> TwitterTweet { get; set; }
        public List<MvLinkedInPost> LinkedInPost { get; set; }
    }
}
