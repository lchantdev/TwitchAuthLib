using RestSharp;
using System.Text;
using TwitchAuthLib.Exceptions;
using TwitchAuthLib.Helpers;
using TwitchAuthLib.Models;

namespace TwitchAuthLib
{
    /// <summary>
    /// Handle Twitch OAuth2 authentication.
    /// </summary>
    /// <param name="clientId">Twitch Application Client ID</param>
    /// <param name="clientSecret">Twitch Application Client Secret</param>
    /// /// <param name="redirectUrl">URL that the user gets redirected back to after sign-in.</param>
    public class TwitchAuth(string clientId, string clientSecret, string redirectUrl)
    {
        private readonly string _clientId = clientId;
        private readonly string _clientSecret = clientSecret;
        private readonly string _redirectUrl = redirectUrl;

        private readonly RestClient _httpClient = new(Constants.TwitchAuthBaseUrl);

        /// <summary>
        /// Generates a Twitch OAuth URI to redirect a user to.
        /// <br/>
        /// The user gets redirected back to your specified URL,
        /// where you validate the scope and then pass the 
        /// response to <see cref="GetTokenPair(TwitchSignInRedirectResponse)"/>.
        /// </summary>
        /// <param name="scopes">Scopes you provided.</param>
        /// <returns></returns>
        public GenerateSignInUrlResponse GenerateSignInUrl(string scopes)
        {
            var state = KeyGenerator.GetUniqueKey(15);

            var uri = new StringBuilder(Constants.TwitchAuthBaseUrl);
            uri.Append($"?client_id={_clientId}");
            uri.Append($"&redirect_uri={Uri.EscapeDataString(_redirectUrl)}");
            uri.Append($"&response_type={Constants.UserGrantType}");
            uri.Append($"&scope={Uri.EscapeDataString(scopes)}");
            uri.Append($"&state={Uri.EscapeDataString(state)}");

            return new GenerateSignInUrlResponse()
            {
                Url = uri.ToString(),
                Scopes = scopes,
                State = state,
                RedirectUri = _redirectUrl
            };
        }

        /// <summary>
        /// Take the redirect response from Twitch and exchange it for an
        /// authentication token.
        /// </summary>
        /// <param name="generateResponse"></param>
        /// <param name="twitchResponse"></param>
        /// <returns></returns>
        /// <exception cref="TwitchSignInRedirectException"></exception>
        public async Task<TwitchToken> GetTokenPair(GenerateSignInUrlResponse generateResponse, TwitchSignInRedirectResponse twitchResponse)
        {
            if(!string.IsNullOrEmpty(twitchResponse.Error))
                throw new TwitchSignInRedirectException(twitchResponse.Error, twitchResponse.ErrorDescription ?? "");

            if (generateResponse.State != twitchResponse.State)
                throw new TwitchSignInRedirectException("INVALID_STATE", "The state returned from Twitch does not match the state we generated locally.");

            if(string.IsNullOrEmpty(twitchResponse.Code))
                throw new TwitchSignInRedirectException("NO_CODE", "No valid code was returned from Twitch.");

            var request = new RestRequest("token", Method.Post);
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
            request.AddParameter("code", twitchResponse.Code);
            request.AddParameter("grant_type", Constants.UserReturnGrantType);
            request.AddParameter("redirect_uri", generateResponse.RedirectUri);

            var response = await _httpClient.PostAsync<TwitchTokenResponse>(request) ??
                throw new TwitchSignInRedirectException("INVALID_RESPONSE", "The response from Twitch was invalid (empty).");

            var tokResp = new TwitchToken()
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                TokenType = TwitchTokenType.User,
                ExpiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresIn),
                ExpiresIn = response.ExpiresIn
            };
            
            return tokResp;
        }

        /// <summary>
        /// Refreshes an existing TwitchToken and returns the new token.
        /// </summary>
        /// <param name="existingToken"></param>
        /// <returns></returns>
        /// <exception cref="TwitchTokenRefreshException"></exception>
        public async Task<TwitchToken> RefreshToken(TwitchToken existingToken)
        {
            if (string.IsNullOrEmpty(existingToken.RefreshToken))
                throw new TwitchTokenRefreshException("The existingToken parameter needs to contain a valid refresh token value.");

            var request = new RestRequest("token", Method.Post);
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
            request.AddParameter("refresh_token", existingToken.RefreshToken);
            request.AddParameter("grant_type", Constants.RefreshGrantType);

            var response = await _httpClient.PostAsync<TwitchTokenResponse>(request) ??
                throw new TwitchTokenRefreshException("The response from Twitch was invalid (empty).");

            var tokResp = new TwitchToken()
                {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                TokenType = existingToken.TokenType,
                ExpiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresIn),
                ExpiresIn = response.ExpiresIn
            };

            return tokResp;
        }

        /// <summary>
        /// Generates and returns an App Token for the current application.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="TwitchAppTokenException">Thrown if the response from Twitch is invalid.</exception>
        public async Task<TwitchToken> GetAppToken()
        {
            var request = new RestRequest("token", Method.Post);
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
            request.AddParameter("grant_type", Constants.AppGrantType);

            var response = await _httpClient.PostAsync<TwitchTokenResponse>(request) ??
                throw new TwitchAppTokenException("The response from Twitch was invalid (empty).");

            var tokResp = new TwitchToken()
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                TokenType = TwitchTokenType.App,
                ExpiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresIn),
                ExpiresIn = response.ExpiresIn
            };

            return tokResp;           
        }
    }

}
