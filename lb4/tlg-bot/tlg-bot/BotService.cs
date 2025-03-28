using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;

namespace tlg_bot
{
    public class BotService
    {
        private readonly TelegramBotClient _bot;
        private readonly MongoService _mongo;

        public BotService(string token)
        {
            _bot = new TelegramBotClient(token);
            _mongo = new MongoService();
        }

        public async Task StartAsync()
        {
            var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

            _bot.StartReceiving(OnUpdate, OnError, receiverOptions, cts.Token);

            var me = await _bot.GetMeAsync();
            Console.WriteLine($"Бот запущено: @{me.Username}");
        }

        private async Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message) return;
            var message = update.Message;
            if (message?.Text == null) return;

            if (message.Text == "/start")
            {
                await _mongo.AddUniqueUser(message.Chat.Id);
                await botClient.SendTextMessageAsync(message.Chat.Id, "Вітаю! Оберіть опцію:", replyMarkup: MainMenu());
                return;
            }

            switch (message.Text)
            {
                case "Статистика":
                    var stats = await _mongo.GetStats();
                    var mostPopular = await _mongo.GetMostPopularAuditorium();

                    var messageText = $"👥 Унікальних користувачів: {stats.TotalUsers}\n" +
                                      $"🔍 Загальна кількість пошуків: {stats.SearchCount}\n";

                    if (mostPopular != null)
                    {
                        messageText += $"🔥 Найпопулярніша аудиторія: {mostPopular.Number} ({mostPopular.SearchHits} пошуків)";
                    }

                    await botClient.SendTextMessageAsync(message.Chat.Id, messageText, replyMarkup: GetBackMenu("menu"));
                    break;

                case "Знайти аудиторію":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть спосіб пошуку:", replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new[] { new KeyboardButton("🔎 За номером"), new KeyboardButton("📚 За предметом") },
                        new[] { new KeyboardButton("⬅️ Назад до меню") }
                    })
                    { ResizeKeyboard = true });
                    break;

                case "🔎 За номером":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть номер аудиторії:", replyMarkup: GetBackMenu("search"));
                    break;

                case "📚 За предметом":
                    var subjects = await _mongo.GetSubjects();
                    var keyboard = subjects.Select(s => new[] { new KeyboardButton(s) }).ToList();
                    keyboard.Add(new[] { new KeyboardButton("⬅️ Назад до пошуку") });

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть предмет:", replyMarkup: new ReplyKeyboardMarkup(keyboard) { ResizeKeyboard = true });
                    break;

                case "⬅️ Назад до меню":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "🔙 Повертаємось в головне меню", replyMarkup: MainMenu());
                    break;

                case "⬅️ Назад до пошуку":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть спосіб пошуку:", replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new[] { new KeyboardButton("🔎 За номером"), new KeyboardButton("📚 За предметом") },
                        new[] { new KeyboardButton("⬅️ Назад до меню") }
                    })
                    { ResizeKeyboard = true });
                    break;

                default:
                    var room = await _mongo.FindByNumber(message.Text) ?? await _mongo.FindBySubject(message.Text);
                    if (room != null)
                    {
                        await _mongo.IncrementSearchCount();
                        await _mongo.IncrementAuditoriumSearch(room.Number);

                        await botClient.SendTextMessageAsync(message.Chat.Id,
                            $"🏫 Аудиторія {room.Number}\n📚 Предмет: {room.Subject}\n🗺️ Як дістатися: {room.Description}",
                            replyMarkup: GetBackMenu("menu"));
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "❌ Аудиторію не знайдено.", replyMarkup: GetBackMenu("search"));
                    }
                    break;
            }
        }

        private Task OnError(ITelegramBotClient botClient, Exception ex, CancellationToken token)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
            return Task.CompletedTask;
        }

        private ReplyKeyboardMarkup MainMenu() =>
            new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Статистика"), new KeyboardButton("Знайти аудиторію") }
            })
            { ResizeKeyboard = true };

        private ReplyKeyboardMarkup GetBackMenu(string context = "menu")
        {
            string label;

            if (context == "menu")
                label = "⬅️ Назад до меню";
            else if (context == "search")
                label = "⬅️ Назад до пошуку";
            else
                label = "⬅️ Назад";

            return new ReplyKeyboardMarkup(new[]
            {
        new[] { new KeyboardButton(label) }
    })
            { ResizeKeyboard = true };
        }
    }
}
