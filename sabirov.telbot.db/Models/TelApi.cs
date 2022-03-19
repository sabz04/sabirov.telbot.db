using System;
using System.Collections.Generic;
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
        private static async void Client_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            using (TelUserContext db = new TelUserContext())
            {
                var msg = e.Message;
                var user = db.telUsers.FirstOrDefault(x => x.Name == msg.Chat.Username);
                if (user == null)
                    await SendMessage("Здравствуйте! Меня зовут Алексия! Рады познакомиться, давайте произведем регистрацию в системе", msg.Chat.Id);
                switch (msg.Text)
                {
                    case "Войти":
                        await DBOperations.AddUser(msg.Chat.Username, msg.Chat.Id);
                        break;
                    case "Инфо":
                        var cUser =  await DBOperations.GetUser(msg.Chat.Username, msg.Chat.Id);
                        if(cUser != null)
                            await SendMessage($"Ник:{cUser.Name}\nНомер чата:{cUser.ChatId}", msg.Chat.Id);
                        break;
                }
            }
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
