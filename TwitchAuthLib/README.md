# Twitch Auth Library
Simple library to assist with getting and refreshing Twitch user and app access tokens.

Only supports the "code" and "client_credentials" grant types (and also refresh_token of course).

## Usage
```csharp
using TwitchAuthLib;

namespace MyApp
{
	public class MyTwitchAuth
	{
		private readonly TwitchAuth _twitchAuth;
		private GenerateSignInUrlResponse _genResponse;

		public MyTwitchAuth()
		{
			_twitchAuth = new TwitchAuth("myClientId", "myClientSecret", "http://localhost/my-signin-redirect");
		}

		public RedirectUserToTwitch()
		{
			var genResponse = _twitchAuth.GenerateSignInUrl("my space-seperated scopes here");
			_genResponse = genResponse;

			// Redirect user to url _genResponse.Url
		}

		/// <summary>
		/// This is the endpoint that the user is redirected to after they have signed in with Twitch.
		/// The function parameters are URL query parameters send back from the request to your redirect URI.
		/// </summary>
		public async Task GetAccessToken(string code, string state, string scope, string error, string error_description)
		{
			var twitchResp = new TwitchSignInRedirectResponse()
			{
				Code = code,
				State = state,
				Scope = scope,
				Error = error,
				ErrorDescription = error_description
			};

			var tokenResp = await _twitchAuth.GetTokenPair(_genResponse, twitchResp);

			// Save the tokenResp.AccessToken and tokenResp.RefreshToken somewhere
			// Also keep track of twitchResp.ExpiresIn (or twitchResp.ExpiresAt) to know when to refresh the token.
			// When it is time to renew the token, send tokenResp to the `RefreshToken` method, as seen below.

			// REMEMBER!
			// You are responsible for watching the expiry time and requesting to refresh the token when it expires.
		}

		public async Task RefreshToken(TwitchToken myToken) 
		{
			var tokenResp = await _twitchAuth.RefreshToken(myToken);

			// tokenResp will be a new instance of TwitchToken with your new token, as well
			// as the refresh token and new ExpiresIn and ExpiresAt values.
		}

		public async Task GetAppToken()
		{
			var appToken = await _twitchAuth.GetAppToken();

			// appToken is an instance o TwitchToken with the app token.
			// As with the user token, save the appToken.AccessToken, appToken.ExpiresIn, and appToken.ExpiresAt somewhere.
		}
	}
}
```