using SaikoroTravelCommon.IO;
using SaikoroTravelCommon.Models;

using System;
using System.IO;
using System.Text.Json;

namespace SaikoroTravelCommon.ResourceLoaders
{
    /// <summary>
    /// 行程定義の構文解析機能を提供します。
    /// </summary>
    public static class RouteParser
    {
        /// <summary>
        /// 保存済み状態を考慮して、行程定義ファイルを解析します。
        /// </summary>
        /// <param name="stream">行程定義ファイル。</param>
        /// <param name="kVPStore">保存済み状態へのアクセサ。</param>
        /// <param name="stateTableKey">行程の状態を保存するテーブルのキー。</param>
        /// <param name="messageTableKey">行程のメッセージの状態を保存するテーブルのキー。</param>
        /// <param name="destinationVisibleTableKey">行程の目的地の可視性を保存するテーブルのキー。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Route[] Parse(Stream stream, IKVPStore kVPStore, string stateTableKey, string messageTableKey, string destinationVisibleTableKey)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            var jsonDocument = JsonDocument.Parse(stream);

            DateTime refTime = ParseRoot(jsonDocument.RootElement);
            JsonElement data = jsonDocument.RootElement.GetProperty("data");
            var len = data.GetArrayLength();

            var routes = new Route[len];
            for (var i = 0; i < len; i++)
            {
                JsonElement obj = data[i];
                routes[i] = ParseRouteObject(obj, refTime, kVPStore, stateTableKey, messageTableKey, destinationVisibleTableKey);
            }
            return routes;
        }

        private static DateTime ParseRoot(JsonElement root)
        {
            JsonElement referenceTime = root.GetProperty("referenceTime");
            return referenceTime.GetDateTime();
        }

        private static Route ParseRouteObject(JsonElement jsonElement, DateTime referenceTime, IKVPStore kVPStore, string stateTableKey, string messageTableKey, string destinationVisibleTableKey)
        {
            var id = jsonElement.GetPropertyAs<string>("id");
            var displayName = jsonElement.GetPropertyAs<string>("displayName");
            var day = jsonElement.GetPropertyAs<int>("day");
            var departureName = jsonElement.GetPropertyAs<string>("departureName");
            DateTime departureTime = jsonElement.GetProperty("departureTime").GetTime(referenceTime, day, "departureTime");

            var destinationName = jsonElement.GetPropertyAsOrDefault<string>("destinationName");

            DateTime? destinationTime = null;
            if (jsonElement.TryGetProperty("destinationTime", out JsonElement destTime))
            {
                destinationTime = destTime.GetTime(referenceTime, day, "destinationTime");
                if (departureTime > destinationTime)
                {
                    destinationTime = ((DateTime)destinationTime).AddDays(1);
                }
            }

            var trainName = jsonElement.GetPropertyAsOrDefault<string>("trainName");
            var trainDetail = jsonElement.GetPropertyAsOrDefault<string>("trainDetail");

            var price = jsonElement.GetPropertyAsOrNull<int>("price");

            var messageStrings = jsonElement.GetProperty("message").ToArray<string>();
            var messageVisibles = jsonElement.GetProperty("messageVisible").ToArray<bool>();
            if (messageStrings.Length != messageVisibles.Length)
            {
                throw new FormatException();
            }

            var result = new RouteMessage[messageStrings.Length];
            for (var i = 0; i < messageStrings.Length; i++)
            {
                var key = string.Format(messageTableKey, i + 1);
                if (kVPStore.ContainsKey(key, id))
                {
                    var value = bool.Parse(kVPStore.Get(key, id));
                    result[i] = new RouteMessage(i + 1, messageStrings[i], value);
                }
                else
                {
                    result[i] = new RouteMessage(i + 1, messageStrings[i], messageVisibles[i]);
                }
            }

            var destinationVisible = jsonElement.GetPropertyAs<bool>("destinationVisible");
            if (kVPStore.ContainsKey(destinationVisibleTableKey, id))
            {
                destinationVisible = bool.Parse(kVPStore.Get(destinationVisibleTableKey, id));
            }

            var defaultStateString = jsonElement.GetPropertyAs<string>("defaultState");
            if (kVPStore.ContainsKey(stateTableKey, id))
            {
                defaultStateString = kVPStore.Get(stateTableKey, id);
            }

            RouteState routeState = ModelHelper.GetState(defaultStateString);

            var userInfo = UserInfo.FromString(jsonElement.GetProperty("users").ToArray<string>());

            return new Route(id, displayName, day, departureName, departureTime, destinationName, destinationTime
                , destinationVisible, trainName, trainDetail, price, result, routeState, userInfo, kVPStore, stateTableKey, messageTableKey, destinationVisibleTableKey);
        }
    }
}