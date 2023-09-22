using Newtonsoft.Json;

using SaikoroTravelCommon.HttpModels;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Context = Android.Content.Context;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SaikoroTravelApp.Backends
{
    internal static class APIService
    {
        private const string url = @"https://saikorotravelbackend20220827101245.azurewebsites.net/";

        private static readonly HttpClient httpClient;

        static APIService()
        {
            httpClient = new HttpClient();
            //httpClient.Timeout = new TimeSpan(0, 0, 3);
        }

        public static async Task<LotteryResponse> RequestLotteryOpen(Context context, LotteryRequest lotteryRequest)
        {
            //debug
            return null;

            var target = url + "api/openlottery";
            var json = JsonSerializer.Serialize(lotteryRequest);
            var content = new StringContent(json);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            DependencyProvider dependencyProvider = DependencyProvider.Default;
            var logger = Logger.GetInstance(context, dependencyProvider.RequestTimer(context), lotteryRequest.User);

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(target, content);

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    LotteryResponse obj = JsonConvert.DeserializeObject<LotteryResponse>(body);
                    return obj;
                }
                else
                {
                    logger.Warn("HTTPエラー", response.ToString());
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("HTTP POSTが例外を発生させました", ex);
                return null;
            }
        }

        public static async Task<UserRegisterResponse> PostUserToken(Context context, UserRegisterRequest request)
        {
            var target = url + "api/user";
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            DependencyProvider dependencyProvider = DependencyProvider.Default;
            var logger = Logger.GetInstance(context, dependencyProvider.RequestTimer(context), request.User);

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(target, content);

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<UserRegisterResponse>(body);
                }
                else
                {
                    logger.Warn("HTTPエラー", response.ToString());
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("HTTP POSTが例外を発生させました", ex);
                return null;
            }
        }

        public static async Task<bool> PostLog(Log log)
        {
            // debug
            return true;

            var target = url + "api/log";
            var json = JsonSerializer.Serialize(log);
            var content = new StringContent(json);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(target, content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}