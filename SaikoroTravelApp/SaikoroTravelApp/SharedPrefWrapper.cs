using Android.Content;

using SaikoroTravelCommon.IO;

namespace SaikoroTravelApp
{
    /// <summary>
    /// Androidプラットフォームにおける、状態ファイル入出力機能を提供します。
    /// </summary>
    public class SharedPrefWrapper : IKVPStore
    {
        private readonly Context context;

        /// <summary>
        /// <see cref="SharedPrefWrapper"/> クラスのインスタンスを初期化します。
        /// </summary>
        public SharedPrefWrapper(Context context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public bool ExistsTable(string table)
        {
            ISharedPreferences pref = context.GetSharedPreferences(table, FileCreationMode.Private);
            return pref.Contains(table);
        }

        /// <inheritdoc />
        public void DeleteTable(string table)
        {
            ISharedPreferences pref = context.GetSharedPreferences(table, FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            _ = editor.Clear();
            editor.Apply();
            _ = editor.Commit();
        }

        public bool ContainsKey(string table, string key)
        {
            ISharedPreferences pref = context.GetSharedPreferences(table, FileCreationMode.Private);
            return pref.Contains(key);
        }

        public string Get(string table, string key)
        {
            ISharedPreferences pref = context.GetSharedPreferences(table, FileCreationMode.Private);
            return pref.GetString(key, null);
        }

        public void Set(string table, string key, string value)
        {
            ISharedPreferences pref = context.GetSharedPreferences(table, FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            _ = editor.PutString(key, value);
            editor.Apply();
            _ = editor.Commit();
        }
    }
}