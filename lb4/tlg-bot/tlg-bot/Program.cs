using System.Threading.Tasks;
using System;
using tlg_bot;

class Program
{
    static async Task Main(string[] args)
    {
        var botToken = "";
        var bot = new BotService(botToken);
        await bot.StartAsync();
        Console.ReadLine();
    }
}
