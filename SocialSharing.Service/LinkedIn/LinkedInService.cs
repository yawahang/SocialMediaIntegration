using Newtonsoft.Json;
using SocialSharing.Model;
using SocialSharing.Model.LinkedIn;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SocialSharing.Service.LinkedIn
{
    public class LinkedInService : ILinkedInService
    {
        public LinkedInService()
        {

        }

        public async Task<MvLinkedInToken> LinkedInAccessTokenAsync(MvLinkedInAuth linkedInAuth, MvLinkedInToken linkedInToken, string code)
        {
            try
            {
                string accessTokenUri = string.Format(linkedInAuth.AccessTokenUri, code, linkedInAuth.RedirectUri, linkedInAuth.ClientId, linkedInAuth.ClientSecret);

                HttpWebRequest request = WebRequest.Create(accessTokenUri) as HttpWebRequest;
                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string tokenResponse = reader.ReadToEnd();

                    if (!string.IsNullOrEmpty(tokenResponse?.ToString()))
                    {
                        MvLinkedInTokenResponse linkedInTokenResponse = JsonConvert.DeserializeObject<MvLinkedInTokenResponse>(tokenResponse);
                        linkedInToken.AccessToken = linkedInTokenResponse.access_token;
                    }

                    return linkedInToken;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> CompanyPostAsync(MvLinkedInAuth linkedInAuth, MvLinkedInToken linkedInToken, MvSocialSharing message)
        {
            try
            {
                List<MvLinkedInPost> linkedInPost = new List<MvLinkedInPost>();
                string sharePostUri = "https://api.linkedin.com/v2/shares";
                HttpClient client = new HttpClient();

                LinkedInPostRequest body = new LinkedInPostRequest { }; 
                body.content.title = " => Test LinkedIn App API C#";

                List<LinkedInPostThumbnail> postThumbnails = new List<LinkedInPostThumbnail>
                {
                    new LinkedInPostThumbnail
                    {
                        resolvedUrl = "https://www.example.com/image.jpg"
                    }
                };
                LinkedInPostContentEntity postContent = new LinkedInPostContentEntity
                {
                    entityLocation = "https://www.example.com/content.html",
                    thumbnails = postThumbnails
                };
                body.content.contentEntities.Add(postContent);
                //body.distribution.linkedInDistributionTarget = { };
                //body.owner = "urn:li:organization:324_kGGaLE";
                body.owner = "urn:li:person:324_kGGaLE";
                body.subject = "Test Share Subject";
                body.text.text = message.Text + " => Test LinkedIn App API C#";

                FormUrlEncodedContent content = new FormUrlEncodedContent(JsonConvert.DeserializeObject<Dictionary<string, string>>(body.ToString()));
                HttpResponseMessage response = await client.PostAsync(sharePostUri, content);

                string responseString = await response.Content.ReadAsStringAsync();
                dynamic result = null;
                if (!string.IsNullOrEmpty(responseString))
                {
                    result = JsonConvert.DeserializeObject(responseString);
                }

                return result["id"];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<List<MvLinkedInPost>> GetCompanyPostAsync(MvLinkedInAuth linkedInAuth, MvLinkedInToken linkedInToken)
        {
            throw new NotImplementedException();
        }

    }
}