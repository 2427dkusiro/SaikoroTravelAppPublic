using Android.Content;

using SaikoroTravelCommon.Models;

using System;

namespace SaikoroTravelApp
{
    internal class ConfigManager
    {
        private readonly Context context;

        public ConfigManager(Context context)
        {
            this.context = context;
        }

        private const string tableKey = "UserInfo";
        private const string userKey = "user";

        public bool TryGetUser(out Users user)
        {
            ISharedPreferences pref = context.GetSharedPreferences(tableKey, FileCreationMode.Private);
            if (pref is null)
            {
                user = default;
                return false;
            }
            var userPref = pref.GetInt(userKey, -1);
            if (userPref == -1)
            {
                user = default;
                return false;
            }
            user = (Users)userPref;
            return true;
        }

        public Users User
        {
            get => TryGetUser(out Users user) ? user : throw new InvalidOperationException("ユーザー情報が設定されていません");
            set
            {
                ISharedPreferences pref = context.GetSharedPreferences(tableKey, FileCreationMode.Private);
                if (pref is null)
                {
                    throw new InvalidOperationException("SharedPreferncesテーブルの取得に失敗しました");
                }
                ISharedPreferencesEditor editor = pref.Edit();
                _ = editor.PutInt(userKey, (int)value);
                editor.Apply();
            }
        }

        /// <summary>
        /// デバッグ用
        /// </summary>
        public void DeleteUser()
        {
            ISharedPreferences pref = context.GetSharedPreferences(tableKey, FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            _ = editor.Remove(userKey);
            _ = editor.Commit();
        }
    }
}