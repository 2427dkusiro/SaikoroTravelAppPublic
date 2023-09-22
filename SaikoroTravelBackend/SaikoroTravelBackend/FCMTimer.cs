namespace SaikoroTravelBackend;

public class FCMTimer
{
    public static async Task CheckLoop()
    {
        var client = new HttpClient();

        while (true)
        {
            var content = new StringContent("internal");
            _ = await client.PostAsync("https://saikorotravelbackend20220827101245.azurewebsites.net/api/debug", content);
            await Task.Delay(new TimeSpan(0, 1, 0));
        }
    }
}
