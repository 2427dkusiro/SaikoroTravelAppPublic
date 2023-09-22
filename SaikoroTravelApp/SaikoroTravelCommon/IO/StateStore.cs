/*
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SaikoroTravelCommon.IO
{
    /// <summary>
    /// Json ファイルの読み書きによって状態の保存を提供する <see cref="IKVPStateStore"/> を表現します。
    /// </summary>
    public class JsonStateStore : IKVPStateStore
    {
        private IStateFileIOProvider stateFileIOProvider;
        private JsonStateFile jsonStateFile;

#pragma warning disable CS8618
        protected JsonStateStore()
        {

        }
#pragma warning restore CS8618

        /// <summary>
        /// ファイルをロードして <see cref="JsonStateStore"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="iOProvider">Json ファイルへの IO を提供するオブジェクト。</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<JsonStateStore> OpenAsync(IStateFileIOProvider iOProvider)
        {
            if (iOProvider.Exists())
            {
                using Stream stream = await iOProvider.OpenRead();
                JsonStateFile? saved = default;
                try
                {
                    saved = await JsonSerializer.DeserializeAsync<JsonStateFile>(stream);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw;
                }
                if (saved is null)
                {
                    throw new InvalidOperationException();
                }
                var instance = new JsonStateStore()
                {
                    stateFileIOProvider = iOProvider,
                    jsonStateFile = saved
                };
                return instance;
            }
            else
            {
                var file = new JsonStateFile()
                {
                    Version = 0,
                    Data = new Dictionary<string, Dictionary<string, string>>()
                };
                using Stream stream = await iOProvider.OpenWrite();
                try
                {
                    // using JsonSerializer.SerializeAsync(object,stream) causes runtime exception!
                    var str = JsonSerializer.Serialize(file);
                    using var streamWriter = new StreamWriter(stream);
                    await streamWriter.WriteAsync(str);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw;
                }
                var instance = new JsonStateStore()
                {
                    stateFileIOProvider = iOProvider,
                    jsonStateFile = file
                };
                return instance;
            }
        }

        /// <inheritdoc />
        public ValueTask<string> Get(string tableKey, string key)
        {
            return new ValueTask<string>(jsonStateFile.Data![tableKey][key]);
        }

        /// <inheritdoc />
        public ValueTask<bool> ContainsKey(string tableKey, string key)
        {
            return new ValueTask<bool>(jsonStateFile.Data!.ContainsKey(tableKey) && jsonStateFile.Data![tableKey].ContainsKey(key));
        }

        /// <inheritdoc />
        public async ValueTask Set(string tableKey, string key, string value)
        {
            if (!jsonStateFile.Data!.ContainsKey(tableKey))
            {
                jsonStateFile.Data!.Add(tableKey, new Dictionary<string, string>());
            }
            if (!jsonStateFile.Data![tableKey].ContainsKey(key))
            {
                jsonStateFile.Data![tableKey].Add(key, value);
            }
            else
            {
                jsonStateFile.Data![tableKey][key] = value;
            }
            jsonStateFile.Version++;
            using Stream? stream = await stateFileIOProvider.OpenWrite();

            try
            {
                var str = JsonSerializer.Serialize(jsonStateFile);
                using var streamWriter = new StreamWriter(stream);
                await streamWriter.WriteAsync(str);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }
    }

    /// <summary>
    /// <see cref="JsonStateStore"/> が保存する Json ファイルの形式を表現します。
    /// </summary>
    public class JsonStateFile
    {
        /// <summary>
        /// バージョン番号を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 内容を変更するたびにこのプロパティをインクリメントします。
        /// </remarks>
        public int Version { get; set; }

        /// <summary>
        /// データを取得または設定します。
        /// </summary>
        /// <remarks>
        /// テーブル名、キーの順に引きます。
        /// </remarks>
        public Dictionary<string, Dictionary<string, string>>? Data { get; set; }
    }
}
*/