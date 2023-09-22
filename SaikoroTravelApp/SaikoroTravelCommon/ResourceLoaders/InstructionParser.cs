using SaikoroTravelCommon.IO;
using SaikoroTravelCommon.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SaikoroTravelCommon.ResourceLoaders
{
    internal class InstructionParser
    {
        public static Instruction[] Parse(Stream stream, IKVPStore accesser, string tableName)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            var jsonDocument = JsonDocument.Parse(stream);

            DateTime refTime = ParseRoot(jsonDocument.RootElement);
            JsonElement data = jsonDocument.RootElement.GetProperty("data");
            var len = data.GetArrayLength();

            var instructions = new Instruction[len];
            for (var i = 0; i < len; i++)
            {
                JsonElement obj = data[i];
                instructions[i] = ParseInstructionObject(obj, refTime, accesser, tableName);
            }
            return instructions;
        }

        private static DateTime ParseRoot(JsonElement root)
        {
            JsonElement referenceTime = root.GetProperty("referenceTime");
            return referenceTime.GetDateTime();
        }

        private static Instruction ParseInstructionObject(JsonElement element, DateTime referenceTime, IKVPStore kVPStore, string tableName)
        {
            var id = element.GetPropertyAs<string>("id");
            var day = element.GetPropertyAs<int>("day");
            DateTime desiredNotificationTime = element.GetProperty("desiredNotificationTime").GetTime(referenceTime, day, "desiredNotificationTime");
            DateTime limitTime = element.GetProperty("limitTime").GetTime(referenceTime, day, "limitTime");
            var content = element.GetPropertyAs<string>("content");
            RouteState defaultState = ModelHelper.GetState(element.GetPropertyAs<string>("defaultState"));
            var user = UserInfo.FromString(element.GetProperty("users").ToArray<string>());
            IEnumerable<RouteUpdateAction>? routeUpdates = element.GetProperty("routeUpdates").AsEnumerable()
                .Select(x => GetRouteUpdateAction(x));

            if (kVPStore.ContainsKey(tableName, id))
            {
                defaultState = ModelHelper.GetState(kVPStore.Get(tableName, id));
            }

            return new Instruction(id, day, desiredNotificationTime, limitTime, content, kVPStore, tableName, defaultState, user, routeUpdates);
        }

        private static RouteUpdateAction GetRouteUpdateAction(JsonElement element)
        {
            var routeId = element.GetPropertyAs<string>("routeId");


            var field = element.GetPropertyAs<string>("field");
            switch (field)
            {
                case "destinationVisible":
                    var value1 = element.GetPropertyAs<bool>("value");
                    return new RouteDestinationVisibleUpdateAction(routeId, value1);
                case "message1":
                    var value2 = element.GetPropertyAs<bool>("value");
                    return new RouteMessageVisibleUpdateAction(routeId, value2, 1);
                case "message2":
                    var value3 = element.GetPropertyAs<bool>("value");
                    return new RouteMessageVisibleUpdateAction(routeId, value3, 2);
                case "message3":
                    var value4 = element.GetPropertyAs<bool>("value");
                    return new RouteMessageVisibleUpdateAction(routeId, value4, 3);
                case "state":
                    RouteState value5 = ModelHelper.GetState(element.GetPropertyAs<string>("value"));
                    return new RouteStateUpdateAction(routeId, value5);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
