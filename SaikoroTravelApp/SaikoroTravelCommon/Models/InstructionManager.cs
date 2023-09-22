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
    /// <summary>
    /// <see cref="Instruction"/> を管理する機能を提供します。
    /// </summary>
    public class InstructionManager
    {
        private readonly List<Instruction> instructions = new List<Instruction>();
        private ITimer? timer;

        public Users? User { get; private set; }

        /// <summary>
        /// 現在ロードされている <see cref="Instruction"/> のリストを取得します。
        /// </summary>
        public IReadOnlyList<Instruction> Instructions => instructions;

        /// <summary>
        /// ID から <see cref="Instruction"/> を取得します。存在しない場合は例外がスローされます。
        /// </summary>
        /// <param name="id">検索する ID。</param>
        /// <returns></returns>
        public Instruction FindById(string id)
        {
            return instructions.Single(x => x.Id == id);
        }

        /// <summary>
        /// 埋め込みリソースと保存された状態から <see cref="Instruction"/> をロードします。
        /// </summary>
        /// <param name="accesser">状態を保存しているストアへのアクセサ。</param>
        /// <returns></returns>
        public void Load(Users? user, IKVPStore accesser, ITimer timer)
        {
            this.timer = timer;
            User = user;
            LoadResource(user, accesser);

            foreach (Instruction instruction in instructions.Where(x => x.UserInfo.HasPermission(user)))
            {
                UpdateInstructionFromTimer(timer.Now, instruction);
                RegistorInstructionToUpdateTimer(timer, instruction);
            }
        }

        public void ForceUpdate()
        {
            foreach (Instruction instruction in instructions.Where(x => x.UserInfo.HasPermission(User)))
            {
                UpdateInstructionFromTimer(timer.Now, instruction);
            }
        }

        private const string asmName = "SaikoroTravelCommon";
        private const string tableName = "Instruction_State";

        private void LoadResource(Users? user, IKVPStore accesser)
        {
            var asm = Assembly.GetExecutingAssembly();
            using Stream stream = asm.GetManifestResourceStream($"{asmName}.Resources.Instruction.json");
            Instruction[] instructions = InstructionParser.Parse(stream, accesser, tableName);
            AssertIdIsUnique(instructions);
            this.instructions.AddRange(instructions);
            AttachTimerToRoutes(instructions.Where(x => x.UserInfo.HasPermission(user)));
        }

        private void AssertIdIsUnique(IEnumerable<Instruction> instructions)
        {
            var hash = new HashSet<string>();
            foreach (Instruction instruction in instructions)
            {
                if (hash.Contains(instruction.Id))
                {
                    throw new FormatException($"ID '{instruction.Id}' が重複しています");
                }
            }
        }

        private void AttachTimerToRoutes(IEnumerable<Instruction> instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                instruction.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(Instruction.InstructionState))
                    {
                        UpdateInstructionFromTimer(timer!.Now, instruction);
                        RegistorInstructionToUpdateTimer(timer, instruction);
                        StateChanged?.Invoke(sender, e);
                    }
                };
            }
        }

        private static void UpdateInstructionFromTimer(DateTime nowTime, Instruction instruction)
        {
            if (instruction.InstructionState == RouteState.WaitingForActivation)
            {
                if (instruction.DesiredNotificationTime.AddMinutes(-1) <= nowTime)
                {
                    instruction.SetState(RouteState.Activated);
                }
            }
        }

        private static void RegistorInstructionToUpdateTimer(ITimer timer, Instruction instruction)
        {
            if (instruction.InstructionState == RouteState.WaitingForActivation)
            {
                timer.AddCallback(instruction.DesiredNotificationTime.AddMinutes(-1), time =>
                {
                    if (instruction.InstructionState == RouteState.WaitingForActivation)
                    {
                        instruction.SetState(RouteState.Activated);
                    }
                });
            }
            if (instruction.InstructionState == RouteState.WaitingForActivation || instruction.InstructionState == RouteState.Activated)
            {
                timer.AddInstructionNotification(instruction);
            }
        }

        /// <summary>
        /// <see cref="Instruction"/> の状態が変化したときに発生するイベント。
        /// </summary>
        public event EventHandler<EventArgs>? StateChanged;
    }
}
