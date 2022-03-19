using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot;

namespace sabirov.telbot.db.Models
{
    public class TelApi
    {
        private static TelegramBotClient client;
        private static string token = "5232226113:AAFSHMs4MCDMOmVGMDL1wGP6iZNjMNtIMvM";
        
        public TelApi()
        {
            client = new TelegramBotClient(token);
            client.OnMessage += Client_OnMessage;
        }
        public static bool IsReceiving()
        {
            return client.IsReceiving;
        }
        private static async void Client_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            using (TelUserContext db = new TelUserContext())
            {
                var msg = e.Message;
                var user = await DBOperations.GetUser(msg.Chat.Username, msg.Chat.Id);
                switch (msg.Text)
                {
                    case "Регистрация":
                        await DBOperations.AddUser(msg.Chat.Username, msg.Chat.Id, "https://www.un.org/sites/un2.un.org/files/chat.png");
                        break;
                    case "Инфо":
                        if (user == null)
                            return;
                          await SendMessage($"Ник:{user.Name}\nНомер чата:{user.ChatId}", msg.Chat.Id);
                          await SendPhoto(user.ChatId, DBOperations.ConvertPhotoStream(user.Photo));
                        break;
                    case "Удалить":
                        if (user == null)
                            return;
                        await DBOperations.RemoveUser(user.ChatId);
                        break;
                    default:
                        await SendMessage("Не знаю такой команды...", msg.Chat.Id);
                        break;
                    
                }
            }
        }
        public static async Task SendPhoto(long chatid, Stream photo)
        {
            await client.SendPhotoAsync(chatid, photo);
            
        }
        public static async Task SendMessage(string message, long userid)
        {
            await client.SendTextMessageAsync(userid, message);
        }

        public static void BotActive(bool flag)
        {
            if(flag)
                client.StartReceiving();
            else
                client.StopReceiving();
        }
    }
}
