using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace RivneDating_TeleBot
{
    class MyTeleBot
    {
        static readonly TelegramBotClient Bot = new TelegramBotClient( /* token */ );
        static readonly User user = new User();

        static public void Main()
        {
            var BotInfo = Bot.GetMeAsync().Result;
            Console.WriteLine("My id: {0} \nMy name: {1}", BotInfo.Id, BotInfo.Username);


            Bot.OnMessage += Bot_OnMessage;
            Bot.StartReceiving();
            Thread.Sleep(int.MaxValue);

            Console.ReadLine();
        }

        // Обработчик события получения сообщения
        // В зависимости от команды выполняет тот или иной метод
        static public async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            user.IDPath = @"users\id.txt";
            string path = $@"users\{e.Message.Chat.Id}.txt";

            try
            {
                if (e.Message.Text == null)
                    Do_Nothing();

                else
                {
                    if (!File.Exists(path))
                    {
                        using var stream = new FileStream(path, FileMode.Create);

                        File.AppendAllText(user.IDPath, $"{e.Message.Chat.Username}: {e.Message.Chat.Id} ");

                        await Bot.SendTextMessageAsync(text: /*text*/,
                                                       chatId: e.Message.Chat.Id);

                        Console.WriteLine($"New {e.Message.Chat.Id} file created.");
                    }

                    if (e.Message.Text.StartsWith("/myposts"))
                    {
                        await MyPostsAsync(path, e);
                    }

                    if (e.Message.Text.StartsWith("/newpost"))
                    {
                        await NewPostAsync(e.Message.Text, path, e);
                    }

                    if (e.Message.Text.StartsWith("/getusers") & ((e.Message.Chat.Id == /* admin_id */))
                        await Bot.SendTextMessageAsync(text: File.ReadAllText(user.IDPath), chatId: e.Message.Chat.Id);
                }
            }

            catch (Telegram.Bot.Exceptions.ApiRequestException)
            {
                Do_Nothing();
            }

            catch (IOException)
            {
                await Task.Delay(300);
            }

            catch (NullReferenceException)
            {
                Do_Nothing();
            }
        }

        static public async Task NewPostAsync(string messageText, string filePath, MessageEventArgs e)
        {
            string[] text;
            string newText = "";

            await Task.Run(() =>
            {
                if (messageText == "/newpost")
                    Do_Nothing();

                text = messageText.Split();
                for (int i = 1; i < text.Length; i++)
                {
                    newText += $"{text[i]} ";
                }

                File.AppendAllLinesAsync(filePath, new[] { newText });
            });

            await Bot.SendTextMessageAsync(text: newText, chatId: /* admin_id */);
            await Bot.SendTextMessageAsync(text: /*text*/, chatId: e.Message.Chat.Id);

            Console.WriteLine($"Got message from {e.Message.Chat.Id} and sent to admins.");
        }

        static public async Task MyPostsAsync(string filePath, MessageEventArgs e)
        {
            foreach (var line in File.ReadLines(filePath))
                await Bot.SendTextMessageAsync(chatId: e.Message.Chat.Id, text: line);

            Console.WriteLine($"Sent all posts to {e.Message.Chat.Id} successfully.");
        }

        static void Do_Nothing() { }
    }
}