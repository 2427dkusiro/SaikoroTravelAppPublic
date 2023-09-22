namespace SaikoroTravelCommon.IO
{
    /// <summary>
    /// アプリケーションの状態を保存する key-value-pair ストレージへのアクセスを提供することを約束します。
    /// </summary>
    public interface IKVPStore
    {
        bool ExistsTable(string table);

        void DeleteTable(string table);

        /// <summary>
        /// キーが存在するかどうか調べます。
        /// </summary>
        /// <param name="tableKey">一意のテーブル名。</param>
        /// <param name="key">一意のキー。</param>
        /// <returns></returns>
        bool ContainsKey(string table, string key);

        /// <summary>
        /// 指定のキーに関連付けられた値を取得します。
        /// </summary>
        /// <param name="tableKey">一意のテーブル名。</param>
        /// <param name="key">一意のキー。</param>
        /// <returns></returns>
        public string Get(string table, string key);

        /// <summary>
        /// 指定のキーに値を設定します。
        /// </summary>
        /// <param name="tableKey">一意のテーブル名。</param>
        /// <param name="key">一意のキー。</param>
        /// <param name="value">設定する値。</param>
        /// <returns></returns>
        public void Set(string table, string key, string value);
    }
}
