using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

class Program
{
    private DiscordSocketClient _client;
    private IConfiguration _config;

    static async Task Main(string[] args) => await new Program().RunBotAsync();

    public async Task RunBotAsync()
    {
        Console.WriteLine("Bot démarré...");

        // Chargement de la configuration
        _config = LoadConfiguration();

        // Configuration du client Discord avec les intents nécessaires
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

        await Task.Delay(-1); // Garde le bot en vie
    }

    private async Task CommandHandler(SocketMessage message)
    {
        // Vérifie que c'est un message utilisateur et pas un embed ou autre
        if (message is not SocketUserMessage userMessage) return;

        Console.WriteLine($"Message reçu : \"{userMessage.Content}\" de {userMessage.Author.Username}");

        // Ignore les messages des bots
        if (userMessage.Author.IsBot) return;

        if (userMessage.Content == "!ping")
        {
            await userMessage.Channel.SendMessageAsync("Pong !");
        }
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }

    private IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        return builder.Build();
    }
}
