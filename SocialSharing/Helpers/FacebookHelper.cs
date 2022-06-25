using Facebook;
using System.Security.Cryptography;
using System.Text;

namespace SocialSharing.Helpers
{
    /// <summary>
    /// Facebook Helper
    /// </summary>
    public static class FacebookHelper
    {
        /// <summary>
        /// Generate a facebook secret proof (works with facebook APIs v2.4)
        /// <seealso cref="http://stackoverflow.com/questions/20572523/c-sharp-help-required-to-create-facebook-appsecret-proof-hmacsha256"/>
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string GenerateFacebookSecretProof(string accessToken, string appSecret)
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
    }
}