using SaikoroTravelCommon.IO;
using SaikoroTravelCommon.ResourceLoaders;
using SaikoroTravelCommon.Time;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SaikoroTravelCommon.Models
{
    public class LotteryManager
    {
        private readonly List<Lottery> lotteries = new List<Lottery>();

        public IReadOnlyList<Lottery> Lotteries => lotteries;

        public Users? User { get; private set; }

        public LotteryManager()
        {

        }

        public Lottery FindById(string id)
        {
            return lotteries.Single(x => x.Id == id);
        }

        public void Load(Users? user, IKVPStore accesser, ITimer timer)
        {
            this.timer = timer;
            User = user;
            LoadResource(user, accesser);
            foreach (Lottery lottery in lotteries.Where(x => x.UserInfo.HasPermission(user)))
            {
                UpdateLotteryFromTimer(timer.Now, lottery);
                RegistorLotteryForUpdateTimer(timer, lottery);
            }
        }

        public void ForceUpdateNow()
        {
            foreach (Lottery lottery in lotteries.Where(x => x.UserInfo.HasPermission(User)))
            {
                UpdateLotteryFromTimer(timer.Now, lottery);
            }
        }

        private ITimer? timer;

        private const string asmName = "SaikoroTravelCommon";
        private const string stateTableName = "Lottery_State";

        private void LoadResource(Users? user, IKVPStore accesser)
        {
            var asm = Assembly.GetExecutingAssembly();
            using Stream stream = asm.GetManifestResourceStream($"{asmName}.Resources.Lottery.json");
            Lottery[] lotteries = LotteryParser.Parse(stream, accesser, stateTableName);
            AssertIdIsUnique(lotteries);
            this.lotteries.AddRange(lotteries);
            AttachTimerToRoutes(lotteries.Where(x => x.UserInfo.HasPermission(user)));
        }

        private void AssertIdIsUnique(IEnumerable<Lottery> lotteries)
        {
            var hash = new HashSet<string>();
            foreach (Lottery lottery in lotteries)
            {
                if (hash.Contains(lottery.Id))
                {
                    throw new FormatException($"ID '{lottery.Id}' が重複しています");
                }
            }
        }

        private void AttachTimerToRoutes(IEnumerable<Lottery> lotteries)
        {
            foreach (Lottery lottery in lotteries)
            {
                lottery.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(Lottery.LotteryState))
                    {
                        UpdateLotteryFromTimer(timer!.Now, lottery);
                        RegistorLotteryForUpdateTimer(timer, lottery);
                        StateChanged?.Invoke(sender, e);
                    }
                };
            }
        }

        private static void UpdateLotteryFromTimer(DateTime nowTime, Lottery lottery)
        {
            if (lottery.LotteryState == RouteState.WaitingForActivation)
            {
                if (lottery.DesiredNotificationTime.AddMinutes(-1) <= nowTime)
                {
                    lottery.SetLotteryState(RouteState.Activated);
                }
            }
        }

        private static void RegistorLotteryForUpdateTimer(ITimer timer, Lottery lottery)
        {
            if (lottery.LotteryState == RouteState.WaitingForActivation)
            {
                timer.AddCallback(lottery.DesiredNotificationTime.AddMinutes(-1), time =>
                {
                    if (lottery.LotteryState == RouteState.WaitingForActivation)
                    {
                        lottery.SetLotteryState(RouteState.Activated);
                    }
                });
            }
            if (lottery.LotteryState == RouteState.WaitingForActivation || lottery.LotteryState == RouteState.Activated)
            {
                timer.AddLotteryNotification(lottery);
            }
        }

        public event EventHandler<EventArgs>? StateChanged;
    }
}
