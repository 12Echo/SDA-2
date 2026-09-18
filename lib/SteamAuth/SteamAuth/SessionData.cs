using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SteamAuth
{
    public class SessionData
    {
        public ulong SteamID { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string SessionID { get; set; }

        /// <summary>
        /// Refresh token for a Steam client session, only needed for a persistent connection to Steam. Optional.
        /// </summary>
        public string ClientRefreshToken { get; set; }

        public async Task RefreshAccessToken()
        {
            if (string.IsNullOrEmpty(this.RefreshToken))
                throw new Exception("Refresh token is empty");

            if (IsTokenExpired(this.RefreshToken))
                throw new Exception("Refresh token is expired");

            string responseStr;
            try
            {
                var postData = new NameValueCollection();
                postData.Add("refresh_token", this.RefreshToken);
                postData.Add("steamid", this.SteamID.ToString());
                responseStr = await SteamWeb.POSTRequest("https://api.steampowered.com/IAuthenticationService/GenerateAccessTokenForApp/v1/", null, postData);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to refresh token: " + ex.Message, ex);
            }

            var response = JsonConvert.DeserializeObject<GenerateAccessTokenForAppResponse>(responseStr);
            if (string.IsNullOrEmpty(response?.Response?.AccessToken))
                throw new Exception("Steam did not return a new access token");
            this.AccessToken = response.Response.AccessToken;
        }

        /// <summary>
        /// True when a RefreshAccessToken failure means Steam rejected the refresh token, rather than a network problem.
        /// </summary>
        public static bool IsTokenRejected(Exception ex)
        {
            var web = ex.InnerException as WebException;
            if (web == null)
                return ex.InnerException == null && ex.Message.StartsWith("Steam did not return");
            var http = web.Response as HttpWebResponse;
            if (http == null)
                return false;
            int status = (int)http.StatusCode;
            return status == 400 || status == 401 || status == 403;
        }

        public bool IsAccessTokenExpired()
        {
            if (string.IsNullOrEmpty(this.AccessToken))
                return true;

            return IsTokenExpired(this.AccessToken);
        }

        public bool IsRefreshTokenExpired()
        {
            if (string.IsNullOrEmpty(this.RefreshToken))
                return true;

            return IsTokenExpired(this.RefreshToken);
        }

        /// <summary>
        /// When the refresh token stops working, or null if there is no usable token.
        /// </summary>
        public DateTimeOffset? GetRefreshTokenExpiry()
        {
            if (string.IsNullOrEmpty(this.RefreshToken))
                return null;

            try
            {
                return DateTimeOffset.FromUnixTimeSeconds(GetTokenExpiry(this.RefreshToken));
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool IsTokenExpired(string token)
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() > GetTokenExpiry(token);
        }

        private long GetTokenExpiry(string token)
        {
            var tokenComponents = token.Split('.');
            // Fix up base64url to normal base64
            var base64 = tokenComponents[1].Replace('-', '+').Replace('_', '/');

            if (base64.Length % 4 != 0)
            {
                base64 += new string('=', 4 - base64.Length % 4);
            }

            var payloadBytes = Convert.FromBase64String(base64);
            var jwt = JsonConvert.DeserializeObject<SteamAccessToken>(System.Text.Encoding.UTF8.GetString(payloadBytes));
            return jwt.exp;
        }

        public CookieContainer GetCookies()
        {
            if (this.SessionID == null)
                this.SessionID = GenerateSessionID();

            var cookies = new CookieContainer();
            cookies.Add(new Cookie("steamLoginSecure", this.GetSteamLoginSecure(), "/", "steamcommunity.com"));
            cookies.Add(new Cookie("sessionid", this.SessionID, "/", "steamcommunity.com"));
            cookies.Add(new Cookie("mobileClient", "android", "/", "steamcommunity.com"));
            cookies.Add(new Cookie("mobileClientVersion", "777777 3.6.1", "/", "steamcommunity.com"));
            return cookies;
        }

        private string GetSteamLoginSecure()
        {
            return this.SteamID.ToString() + "%7C%7C" + this.AccessToken;
        }

        private static string GenerateSessionID()
        {
            return GetRandomHexNumber(32);
        }

        private static string GetRandomHexNumber(int digits)
        {
            Random random = new Random();
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }

        private class SteamAccessToken
        {
            public long exp { get; set; }
        }

        private class GenerateAccessTokenForAppResponse
        {
            [JsonProperty("response")]
            public GenerateAccessTokenForAppResponseResponse Response;
        }

        private class GenerateAccessTokenForAppResponseResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
        }
    }
}
