using System;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace SteamAuth
{
    /// <summary>
    /// Class to help align system time with the Steam server time. Not super advanced; probably not taking some things into account that it should.
    /// Necessary to generate up-to-date codes. In general, this will have an error of less than a second, assuming Steam is operational.
    /// </summary>
    public class TimeAligner
    {
        private static bool _aligned = false;
        private static int _timeDifference = 0;
        private static DateTime _retryAt = DateTime.MinValue;

        /// <summary>
        /// True once the offset to Steam's clock is known.
        /// </summary>
        public static bool Aligned
        {
            get { return _aligned; }
        }

        public static long GetSteamTime()
        {
            if (!TimeAligner._aligned)
            {
                TimeAligner.AlignTime();
            }
            return Util.GetSystemUnixTime() + _timeDifference;
        }

        public static async Task<long> GetSteamTimeAsync()
        {
            if (!TimeAligner._aligned)
            {
                await TimeAligner.AlignTimeAsync();
            }
            return Util.GetSystemUnixTime() + _timeDifference;
        }

        public static void AlignTime()
        {
            if (DateTime.UtcNow < _retryAt) return;
            long currentTime = Util.GetSystemUnixTime();
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                try
                {
                    string response = client.UploadString(APIEndpoints.TWO_FACTOR_TIME_QUERY, "steamid=0");
                    Apply(response, currentTime);
                }
                catch (Exception)
                {
                    Failed();
                }
            }
        }

        public static async Task AlignTimeAsync()
        {
            if (DateTime.UtcNow < _retryAt) return;
            long currentTime = Util.GetSystemUnixTime();
            WebClient client = new WebClient();
            try
            {
                client.Encoding = Encoding.UTF8;
                string response = await client.UploadStringTaskAsync(new Uri(APIEndpoints.TWO_FACTOR_TIME_QUERY), "steamid=0");
                Apply(response, currentTime);
            }
            catch (Exception)
            {
                Failed();
            }
        }

        // Half the round trip is Steam's answer travelling back, the offset is measured from the middle of the request
        private static void Apply(string response, long requestTime)
        {
            TimeQuery query = JsonConvert.DeserializeObject<TimeQuery>(response);
            if (query?.Response == null) throw new InvalidOperationException("No server time in the response");
            long now = Util.GetSystemUnixTime();
            TimeAligner._timeDifference = (int)(query.Response.ServerTime - (requestTime + now) / 2);
            TimeAligner._aligned = true;
            _retryAt = DateTime.MinValue;
        }

        // Without this an offline machine would ask Steam for the time on every code
        private static void Failed()
        {
            _retryAt = DateTime.UtcNow.AddSeconds(30);
        }

        internal class TimeQuery
        {
            [JsonProperty("response")]
            internal TimeQueryResponse Response { get; set; }

            internal class TimeQueryResponse
            {
                [JsonProperty("server_time")]
                public long ServerTime { get; set; }
            }

        }
    }
}
