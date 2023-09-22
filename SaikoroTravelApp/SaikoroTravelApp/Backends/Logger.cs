using Android.Content;

using SaikoroTravelCommon.HttpModels;
using SaikoroTravelCommon.Models;
using SaikoroTravelCommon.Time;

using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace SaikoroTravelApp.Backends
{
    internal class Logger
    {
        private SharedPrefWrapper store;

        private ITimer timer;

        private Users users;
        private const string tableKey = "Log";

        protected Logger()
        {

        }

        private static Logger instance;

        private static int currnetId;

        public static Logger GetInstance(Context context, ITimer timer, Users users)
        {
            if (instance is null)
            {
                var store = new SharedPrefWrapper(context);
                for (var i = 1; ; i++)
                {
                    if (store.ContainsKey(tableKey, i.ToString()))
                    {
                        currnetId = i;
                    }
                    else
                    {
                        break;
                    }
                }

                var logger = new Logger
                {
                    store = store,
                    timer = timer,
                    users = users
                };
                instance = logger;
            }
            return instance;
        }

        public void Info(string message, object obj = null, [CallerFilePath] string file = null, [CallerMemberName] string caller = null, [CallerLineNumber] int number = 0)
        {
            string add = null;
            if (obj != null)
            {
                add = JsonSerializer.Serialize(obj);
            }
            var id = ++currnetId;

            var log = new Log()
            {
                Id = id,
                Message = message,
                LogLevel = LogLevel.Info,
                User = users,
                TimeStamp = timer.Now,
                FilePath = file,
                Writer = caller,
                WriterLine = number,
                AdditionalObject = add,
                IsPosted = false
            };
            var str = JsonSerializer.Serialize(log);
            store.Set(tableKey, id.ToString(), str);
        }

        public void Warn(string message, object obj = null, [CallerFilePath] string file = null, [CallerMemberName] string caller = null, [CallerLineNumber] int number = 0)
        {
            string add = null;
            if (obj != null)
            {
                add = JsonSerializer.Serialize(obj);
            }
            var id = ++currnetId;

            var log = new Log()
            {
                Id = id,
                Message = message,
                LogLevel = LogLevel.Warning,
                User = users,
                TimeStamp = timer.Now,
                FilePath = file,
                Writer = caller,
                WriterLine = number,
                AdditionalObject = add,
                IsPosted = false
            };
            var str = JsonSerializer.Serialize(log);
            store.Set(tableKey, id.ToString(), str);
        }

        public void Error(string message, object obj = null, [CallerFilePath] string file = null, [CallerMemberName] string caller = null, [CallerLineNumber] int number = 0)
        {
            string add = null;
            if (obj != null)
            {
                add = obj is Exception ? obj.ToString() : JsonSerializer.Serialize(obj);
            }
            var id = ++currnetId;

            var log = new Log()
            {
                Id = id,
                Message = message,
                LogLevel = LogLevel.Error,
                User = users,
                TimeStamp = timer.Now,
                FilePath = file,
                Writer = caller,
                WriterLine = number,
                AdditionalObject = add,
                IsPosted = false
            };
            var str = JsonSerializer.Serialize(log);
            store.Set(tableKey, id.ToString(), str);
        }

        public async Task ReportInstructionTimerWaked(Instruction instruction)
        {
            var add = instruction.Id;
            var id = ++currnetId;

            var log = new Log()
            {
                Id = id,
                Message = "指示通知タイマーが発火しました",
                LogLevel = LogLevel.Report_Instruction_Timer,
                User = users,
                TimeStamp = timer.Now,
                FilePath = null,
                Writer = null,
                WriterLine = 0,
                AdditionalObject = add,
                IsPosted = false
            };
            var str = JsonSerializer.Serialize(log);
            store.Set(tableKey, id.ToString(), str);
            await PostLog();
        }

        public async Task ReportLotteryTimerWaked(Lottery lottery)
        {
            var add = lottery.Id;
            var id = ++currnetId;

            var log = new Log()
            {
                Id = id,
                Message = "抽選通知タイマーが発火しました",
                LogLevel = LogLevel.Report_Lottery_Timer,
                User = users,
                TimeStamp = timer.Now,
                FilePath = null,
                Writer = null,
                WriterLine = 0,
                AdditionalObject = add,
                IsPosted = false
            };
            var str = JsonSerializer.Serialize(log);
            store.Set(tableKey, id.ToString(), str);
            await PostLog();
        }

        public async Task ReportInstructionOpened(Instruction instruction)
        {
            var add = instruction.Id;
            var id = ++currnetId;

            var log = new Log()
            {
                Id = id,
                Message = "指示が開封されました",
                LogLevel = LogLevel.Event_Instruction_Opened,
                User = users,
                TimeStamp = timer.Now,
                FilePath = null,
                Writer = null,
                WriterLine = 0,
                AdditionalObject = add,
                IsPosted = false
            };
            store.Set(tableKey, id.ToString(), JsonSerializer.Serialize(log));
            await PostLog();
        }

        public async Task ReportLotteryOpened(Lottery lottery, int number)
        {
            var report = new LotteryReport()
            {
                Id = lottery.Id,
                Number = number
            };

            var add = JsonSerializer.Serialize(report);
            var id = ++currnetId;

            var log = new Log()
            {
                Id = id,
                Message = "抽選が実行されました",
                LogLevel = LogLevel.Event_Lottery_Opened,
                User = users,
                TimeStamp = timer.Now,
                FilePath = null,
                Writer = null,
                WriterLine = 0,
                AdditionalObject = add,
                IsPosted = false
            };
            store.Set(tableKey, id.ToString(), JsonSerializer.Serialize(log));
            await PostLog();
        }

        public async Task ReportLocation(LocationReport locationReport)
        {
            var add = JsonSerializer.Serialize(locationReport);
            var id = ++currnetId;

            var log = new Log()
            {
                Id = id,
                Message = "位置が特定されました",
                LogLevel = LogLevel.Report_Location,
                User = users,
                TimeStamp = timer.Now,
                FilePath = null,
                Writer = null,
                WriterLine = 0,
                AdditionalObject = add,
                IsPosted = false
            };
            store.Set(tableKey, id.ToString(), JsonSerializer.Serialize(log));
            await PostLog();
        }

        public async Task PostLog()
        {
            for (var i = 0; i <= currnetId; i++)
            {
                if (store.ContainsKey(tableKey, i.ToString()))
                {
                    Log log = JsonSerializer.Deserialize<Log>(store.Get(tableKey, i.ToString()));
                    if (!log.IsPosted)
                    {
                        var res = await APIService.PostLog(log);
                        if (res)
                        {
                            log.IsPosted = true;
                            store.Set(tableKey, i.ToString(), JsonSerializer.Serialize(log));
                            store.Set(tableKey, i.ToString(), JsonSerializer.Serialize(log));
                        }
                    }
                }
            }
        }
    }
}