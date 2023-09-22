using System.Collections.Generic;
using System.Linq;

namespace SaikoroTravelCommon.Models
{
    /// <summary>
    /// ユーザー権限情報を表現します。
    /// </summary>
    public class UserInfo
    {
        private readonly bool toAll;

        private readonly Users[] users;

        /// <summary>
        /// <see cref="UserInfo"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="toAll"></param>
        /// <param name="users"></param>
        public UserInfo(bool toAll, IEnumerable<Users> users)
        {
            this.toAll = toAll;
            this.users = users.ToArray();
        }

        /// <summary>
        /// ユーザー名を表す文字列から <see cref="UserInfo"/> を取得します。
        /// </summary>
        /// <param name="userStrings"></param>
        /// <returns></returns>
        public static UserInfo FromString(IEnumerable<string> userStrings)
        {
            return userStrings.Contains("all")
                ? new UserInfo(true,
                    userStrings.Where(x => x != "all").Select(x => ModelHelper.GetUser(x)))
                : new UserInfo(false, userStrings.Select(x => ModelHelper.GetUser(x)));
        }

        /// <summary>
        /// 指定のユーザーに権限があるかどうかを表す値を取得します。
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public bool HasPermission(Users? users)
        {
            return !(users is Users notNull) || toAll || this.users.Contains(notNull);
        }
    }
}