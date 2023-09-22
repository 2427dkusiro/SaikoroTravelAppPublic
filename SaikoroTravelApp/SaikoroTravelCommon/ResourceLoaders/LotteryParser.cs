using SaikoroTravelCommon.IO;
using SaikoroTravelCommon.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SaikoroTravelCommon.ResourceLoaders
{
    public static class LotteryParser
    {
        public static Lottery[] Parse(Stream stream, IKVPStore kVPStateStoreAccesser, string stateTableName)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var jsonDocument = JsonDocument.Parse(stream);

            DateTime refTime = ParseRoot(jsonDocument.RootElement);
            JsonElement data = jsonDocument.RootElement.GetProperty("data");
            var len = data.GetArrayLength();

            var lotteries = new Lottery[len];
            for (var i = 0; i < len; i++)
            {
                JsonElement obj = data[i];
                lotteries[i] = ParseLotteryObject(obj, refTime, kVPStateStoreAccesser, stateTableName);
            }

            return lotteries;
        }

        private static DateTime ParseRoot(JsonElement root)
        {
            JsonElement referenceTime = root.GetProperty("referenceTime");
            return referenceTime.GetDateTime();
        }

        private static Lottery ParseLotteryObject(JsonElement jsonElement, DateTime refTime, IKVPStore kVPStore, string stateTableName)
        {
            var id = jsonElement.GetPropertyAs<string>("id");
            var title = jsonElement.GetPropertyAs<string>("title");
            var day = jsonElement.GetPropertyAs<int>("day");
            DateTime desiredNotificationTime = jsonElement.GetProperty("desiredNotificationTime").GetTime(refTime, day, "desiredNotificationTime");
            DateTime limitTime = jsonElement.GetProperty("limitTime").GetTime(refTime, day, "limitTime");

            RouteState state = ModelHelper.GetState(jsonElement.GetPropertyAs<string>("defaultState"));
            if (kVPStore.ContainsKey(stateTableName, id))
            {
                state = ModelHelper.GetState(kVPStore.Get(stateTableName, id));
            }

            var userInfo = UserInfo.FromString(jsonElement.GetProperty("users").ToArray<string>());
            IEnumerable<LotteryOption> contents = jsonElement.GetProperty("content").AsEnumerable().Select(x => ParseLotteryOption(x)).SelectMany(x => x);

            return contents.Select(x => x.Number).Distinct().OrderBy(x => x).SequenceEqual(Enumerable.Range(1, 6))
                ? new Lottery(id, title, day, desiredNotificationTime, limitTime, state, userInfo, contents.OrderBy(x => x.Number), kVPStore, stateTableName)
                : throw new FormatException("選択肢は出目を重複無く尽くしてはいません");
        }

        private static IEnumerable<LotteryOption> ParseLotteryOption(JsonElement jsonElement)
        {
            var number = jsonElement.GetProperty("number").ToArray<int>();
            var selectable = jsonElement.GetPropertyAs<bool>("selectable");
            var title = jsonElement.GetProperty("title").ToArray<string>();
            var subTitle = jsonElement.GetProperty("subTitle").ToArray<string>();

            InstructionUpdateAction[] actions = jsonElement.TryGetProperty("instructionUpdates", out JsonElement updates)
                ? updates.AsEnumerable().Select(x => ParseInstructionUpdate(x)).ToArray()
                : Array.Empty<InstructionUpdateAction>();
            if (number.Length == title.Length && title.Length == subTitle.Length)
            {
                for (var i = 0; i < number.Length; i++)
                {
                    yield return new LotteryOption(number[i], selectable, title[i], subTitle[i], actions);
                }
            }
        }

        private static InstructionUpdateAction ParseInstructionUpdate(JsonElement jsonElement)
        {
            var id = jsonElement.GetPropertyAs<string>("instructionId");
            var field = jsonElement.GetPropertyAs<string>("field");
            var value = jsonElement.GetPropertyAs<string>("value");

            return field != "state" ? throw new FormatException() : (InstructionUpdateAction)new InstructionStateUpdateAction(id, ModelHelper.GetState(value));
        }
    }
}
