using Discord;
using Discord.WebSocket;

using SaikoroTravelCommon.HttpModels;
using SaikoroTravelCommon.Models;

using System.Text;
using System.Text.Json;

namespace SaikoroTravelBackend;

public static class LogHandle
{
    public static async Task Handle(Log log)
    {
        SaikoroTravelCommon.HttpModels.LogLevel type = log.LogLevel;
        var stringBuilder = new StringBuilder();

        if (type == SaikoroTravelCommon.HttpModels.LogLevel.Event_Instruction_Opened)
        {
            var id = log.AdditionalObject;
            Instruction instruction = DependencyProvider.Default.RequestInstructionManager().FindById(id);
            _ = stringBuilder.AppendLine($"{log.User.GetName()}が指示を開封しました！");
            _ = stringBuilder.AppendLine("```");
            _ = stringBuilder.AppendLine(instruction.Content);
            _ = stringBuilder.AppendLine("```");
            await DiscordService.SendPublic(stringBuilder.ToString());
        }
        else if (type == SaikoroTravelCommon.HttpModels.LogLevel.Event_Lottery_Opened)
        {
            LotteryReport? add = JsonSerializer.Deserialize<LotteryReport>(log.AdditionalObject!);
            Lottery lottery = DependencyProvider.Default.RequestLotteryManager().FindById(add.Id);
            _ = stringBuilder.AppendLine($"{log.User.GetName()}が抽選で{add.Number}を出しました");
            _ = stringBuilder.AppendLine("```");
            foreach (LotteryOption opt in lottery.LotteryOptions)
            {
                _ = stringBuilder.AppendLine($"{opt.Number}: {opt.SubTitle} {opt.Title}");
            }
            _ = stringBuilder.AppendLine("```");
            await DiscordService.SendPublic(stringBuilder.ToString());
        }
        else if (type == SaikoroTravelCommon.HttpModels.LogLevel.Report_Location)
        {
            LocationReport? add = JsonSerializer.Deserialize<LocationReport>(log.AdditionalObject!);
            _ = stringBuilder.AppendLine("==位置情報==");
            _ = stringBuilder.AppendLine($"ユーザー:{log.User} 日時:{log.TimeStamp}");
            _ = stringBuilder.AppendLine($"緯度:{add.Latitude},経度:{add.Longitude},高度:{add.Altitude}");
            _ = stringBuilder.AppendLine($"郵便番号:{add.PostCode}");
            _ = stringBuilder.AppendLine($"逆Geocoding:{add.Name}");
            _ = stringBuilder.AppendLine($"https://www.google.co.jp/maps/@{add.Latitude},{add.Longitude},15z");
        }
        else
        {
            _ = stringBuilder.AppendLine("==ログを受け取りました==");
            _ = stringBuilder.AppendLine($"[{log.LogLevel}] {log.Message}");
            _ = stringBuilder.AppendLine($"ユーザー:{log.User} 日時:{log.TimeStamp}");
            _ = stringBuilder.AppendLine($"発生箇所:{log.FilePath}::{log.Writer}(line {log.WriterLine})");
            _ = stringBuilder.AppendLine("```json");
            _ = stringBuilder.AppendLine($"{log.AdditionalObject}");
            _ = stringBuilder.AppendLine("```");
        }
        await DiscordService.SendPrivate(stringBuilder.ToString());
    }
}

public static class DiscordService
{
    private static DiscordSocketClient? client;
    private static SocketTextChannel? textPublic;
    private static SocketTextChannel? textPrivate;

    public static readonly string Mention = "<@&{this_bot_id}>";

    public static async Task Init()
    {
        const string token = @"{your_discord_token}";
        client = new DiscordSocketClient();
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        while (!client.Guilds.Any(x => x?.Name == "真ホ場"))
        {
            await Task.Delay(100);
        }

        SocketGuild guild = client.Guilds.First(x => x?.Name == "真ホ場");
        textPublic = guild.TextChannels.First(x => x.Name == "自動_進捗");
        textPrivate = guild.TextChannels.First(x => x.Name == "自動_管理ログ");
    }

    public static async Task SendPublic(string message)
    {
        _ = await textPublic!.SendMessageAsync(message);
    }

    public static async Task SendPrivate(string message)
    {
        _ = await textPrivate!.SendMessageAsync(message);
    }
}
