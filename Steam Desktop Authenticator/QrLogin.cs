using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProtoBuf;
using SteamAuth;
using SteamKit2.Internal;
using ZXing;
using ZXing.Common;

namespace Steam_Desktop_Authenticator
{
    class QrLoginRequest
    {
        public int Version;
        public ulong ClientId;
    }

    // Approves a Steam login QR code the way the mobile app does when it scans one
    static class QrLogin
    {
        private static readonly HttpClient http = new HttpClient();
        private static readonly Regex qrPattern = new Regex(@"s\.team/q/(\d+)/(\d+)", RegexOptions.IgnoreCase);

        public static QrLoginRequest FindOnScreen()
        {
            var reader = new BarcodeReaderGeneric
            {
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                }
            };

            foreach (var screen in Screen.AllScreens)
            {
                var bounds = screen.Bounds;
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb))
                {
                    using (var g = Graphics.FromImage(bitmap))
                        g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);

                    var results = reader.DecodeMultiple(Luminance(bitmap));
                    if (results == null) continue;
                    foreach (var result in results)
                    {
                        var match = qrPattern.Match(result.Text ?? "");
                        if (match.Success)
                            return new QrLoginRequest { Version = int.Parse(match.Groups[1].Value), ClientId = ulong.Parse(match.Groups[2].Value) };
                    }
                }
            }
            return null;
        }

        private static LuminanceSource Luminance(Bitmap bitmap)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                byte[] bytes = new byte[data.Stride * data.Height];
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                return new RGBLuminanceSource(bytes, bitmap.Width, bitmap.Height, RGBLuminanceSource.BitmapFormat.BGR32);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        public static async Task<CAuthentication_GetAuthSessionInfo_Response> GetInfoAsync(SteamGuardAccount account, ulong clientId)
        {
            var request = new CAuthentication_GetAuthSessionInfo_Request { client_id = clientId };
            return await CallAsync<CAuthentication_GetAuthSessionInfo_Response>(account, "GetAuthSessionInfo", request);
        }

        public static async Task ApproveAsync(SteamGuardAccount account, QrLoginRequest login, bool confirm)
        {
            var request = new CAuthentication_UpdateAuthSessionWithMobileConfirmation_Request
            {
                version = login.Version,
                client_id = login.ClientId,
                steamid = account.Session.SteamID,
                signature = Sign(account, login.Version, login.ClientId),
                confirm = confirm,
                persistence = ESessionPersistence.k_ESessionPersistence_Persistent
            };
            await CallAsync<CAuthentication_UpdateAuthSessionWithMobileConfirmation_Response>(account, "UpdateAuthSessionWithMobileConfirmation", request);
        }

        // HMAC over version, client id and steam id, little endian, keyed with the shared secret
        private static byte[] Sign(SteamGuardAccount account, int version, ulong clientId)
        {
            byte[] data = new byte[2 + 8 + 8];
            BitConverter.GetBytes((ushort)version).CopyTo(data, 0);
            BitConverter.GetBytes(clientId).CopyTo(data, 2);
            BitConverter.GetBytes(account.Session.SteamID).CopyTo(data, 10);

            using (var hmac = new HMACSHA256(Convert.FromBase64String(account.SharedSecret)))
                return hmac.ComputeHash(data);
        }

        private static async Task<TResponse> CallAsync<TResponse>(SteamGuardAccount account, string method, object request)
        {
            if (account.Session.IsAccessTokenExpired())
                await account.Session.RefreshAccessToken();

            byte[] body;
            using (var stream = new MemoryStream())
            {
                Serializer.NonGeneric.Serialize(stream, request);
                body = stream.ToArray();
            }

            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "access_token", account.Session.AccessToken },
                { "input_protobuf_encoded", Convert.ToBase64String(body) }
            });

            using (var response = await http.PostAsync("https://api.steampowered.com/IAuthenticationService/" + method + "/v1/", form))
            {
                string result = response.Headers.TryGetValues("x-eresult", out var values) ? string.Join("", values) : "1";
                if (result != "1")
                    throw new Exception("Steam returned " + ResultName(result) + " (" + ResultMessage(response) + ")");
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                    return Serializer.Deserialize<TResponse>(stream);
            }
        }

        private static string ResultName(string code)
        {
            int number;
            if (int.TryParse(code, out number) && Enum.IsDefined(typeof(SteamKit2.EResult), number))
                return ((SteamKit2.EResult)number).ToString();
            return "error " + code;
        }

        private static string ResultMessage(HttpResponseMessage response)
        {
            return response.Headers.TryGetValues("x-error_message", out var values) ? string.Join("", values) : "no details";
        }

        public static string Describe(CAuthentication_GetAuthSessionInfo_Response info)
        {
            if (info == null) return "an unknown device";

            var parts = new List<string>();
            if (!string.IsNullOrEmpty(info.device_friendly_name)) parts.Add(info.device_friendly_name);
            parts.Add(Platform(info.platform_type));

            var where = new List<string>();
            if (!string.IsNullOrEmpty(info.city)) where.Add(info.city);
            if (!string.IsNullOrEmpty(info.state)) where.Add(info.state);
            if (!string.IsNullOrEmpty(info.country)) where.Add(info.country);
            string location = where.Count > 0 ? string.Join(", ", where) : "an unknown location";
            if (!string.IsNullOrEmpty(info.ip)) location += " (" + info.ip + ")";

            return string.Join(" on ", parts) + " from " + location;
        }

        private static string Platform(EAuthTokenPlatformType type)
        {
            switch (type)
            {
                case EAuthTokenPlatformType.k_EAuthTokenPlatformType_SteamClient: return "Steam client";
                case EAuthTokenPlatformType.k_EAuthTokenPlatformType_WebBrowser: return "web browser";
                case EAuthTokenPlatformType.k_EAuthTokenPlatformType_MobileApp: return "mobile app";
                default: return "unknown platform";
            }
        }
    }
}
