using System;

namespace SaikoroTravelCommon.Models
{
    /// <summary>
    /// 列挙型と文字列の変換機能を提供します。
    /// </summary>
    public static class ModelHelper
    {
        /// <summary>
        /// ユーザー名を表す文字列から、定義済みユーザーを取得します。
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Users GetUser(string userName)
        {
            return userName switch
            {
                "user1" => Users.User1,
                "user2" => Users.User2,
                "user3" => Users.User3,
                "user4" => Users.User4,
                _ => throw new NotSupportedException()
            };
        }

        public static string GetJsonValue(this Users user)
        {
            return user switch
            {
                Users.User1 => "user1",
                Users.User2 => "user2",
                Users.User3 => "user3",
                Users.User4 => "user4",
                _ => throw new NotSupportedException()
            };
        }

        public static string GetName(this Users user)
        {
            return user switch
            {
                Users.User1 => "ユーザー1",
                Users.User2 => "ユーザー2",
                Users.User3 => "ユーザー3",
                Users.User4 => "ユーザー4",
                _ => throw new NotSupportedException()
            };
        }

        /// <summary>
        /// 状態を表す文字列から、状態値を取得します。
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static RouteState GetState(string stateName)
        {
            return stateName switch
            {
                "waitingForActivation" => RouteState.WaitingForActivation,
                "unsettled" => RouteState.Unsettled,
                "abandoned" => RouteState.Abandoned,
                "activated" => RouteState.Activated,
                "executed" => RouteState.Executed,
                _ => throw new NotSupportedException()
            };
        }

        public static string GetJsonValue(this RouteState routeState)
        {
            return routeState switch
            {
                RouteState.WaitingForActivation => "waitingForActivation",
                RouteState.Unsettled => "unsettled",
                RouteState.Abandoned => "abandoned",
                RouteState.Activated => "activated",
                RouteState.Executed => "executed",
                _ => throw new NotSupportedException()
            };
        }
    }
}
