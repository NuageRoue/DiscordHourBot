using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

class Program
{
    private DiscordSocketClient _client;

    static async Task Main(string[] args) => await new Program().RunBotAsync();

    public async Task RunBotAsync()
    {
        Console.WriteLine("Bot démarré...");

        

        // configuration:
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);
        _client.Log += Log;
        _client.MessageReceived += CommandHandler;

        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");


        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1); // keep alive
    }

    private async Task CommandHandler(SocketMessage message)
    {
        if (message is not SocketUserMessage userMessage) return;

        Console.WriteLine($"Message reçu : \"{userMessage.Content}\" de {userMessage.Author.Username}");

        if (userMessage.Author.IsBot) return;

        if (userMessage.Content == "!taiwan")
        {
            await userMessage.Channel.SendMessageAsync("À Taïwan, il est: " + GetTaiwanTime());
        }
        if (userMessage.Content == "!france")
        {
            await userMessage.Channel.SendMessageAsync("En France, il est: " + GetFranceTime());
        }
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }

    string GetTaiwanTime()
    {
        TimeZoneInfo taiwanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");

        DateTime utcNow = DateTime.UtcNow;
        DateTime taiwanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, taiwanTimeZone);

        return taiwanTime.ToString("HH:mm");
    }

    string GetFranceTime()
    {
        TimeZoneInfo franceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

        DateTime utcNow = DateTime.UtcNow;
        DateTime franceTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, franceTimeZone);

        return franceTime.ToString("HH:mm");
    }

}
