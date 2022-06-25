using System.Threading.Tasks;

namespace SocialSharing.Service.Facebook
{
    public class FacebookService : IFacebookService
    {
        private readonly string AppId;
        private readonly string ApiKey;
        private readonly string AppSecret;

        public FacebookService()
        {

        }

        public Task<string> GetAccessToken(string code)
        {
            //if(code == null)
            //{

            //}
            //MyWebRequest myRequest = new WebRequest("https://graph.facebook.com/oauth/access_token", "GET", "client_id=" + this.ApplicationID + "&client_secret=" + this.ApplicationSecret + "&code=" + code + "&redirect_uri=http:%2F%2Flocalhost:5176%2F");

            //string accessToken = myRequest.GetResponse().Split('&')[0];
            //accessToken = accessToken.Split('=')[1];

            //return accessToken;
            return Task.FromResult("");
        }
    }
}