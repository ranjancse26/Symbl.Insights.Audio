using SymblAISharp.Authentication;

namespace Symbl.Insights.Audio
{
    public class SymblAuth
    {
        private string appId;
        private string appSecret;
        public SymblAuth(string appId, string appSecret)
        {
            this.appId = appId;
            this.appSecret = appSecret;
        }

        public AuthResponse GetAuthToken()
        {
            AuthenticationApi authentication = new AuthenticationApi();

            var authResponse = authentication.GetAuthToken(
                new AuthRequest
                {
                    type = "application",
                    appId = appId,
                    appSecret = appSecret
                });

            return authResponse;
        }
    }
}
