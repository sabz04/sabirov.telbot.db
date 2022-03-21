using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public static List<string> userL = new List<string>();
        public static string name = "", photo = "";
        private static TelUser telUs = null;

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
                var user = await DBOperations.GetUser( msg.Chat.Id);
                switch (msg.Text)
                {
                    case "Регистрация":
                        if (user==null)
                        {
                            client.OnMessage += Client_OnMessage1;
                            client.OnMessage -= Client_OnMessage;
                            telUs = new TelUser();
                            await SendMessage("Введите ник: ", e.Message.Chat.Id);
                            return;
                        }
                        await DBOperations.AddUser(userL[1], int.Parse(userL[0]), userL[2]);
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
                        name = "";
                        photo = "";
                        userL.Clear();
                        userL = null;
                        break;
                    default:
                        await SendMessage("Не знаю такой команды...", msg.Chat.Id);
                        break;
                    
                }
            }
        }

        private static async void Client_OnMessage1(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            
            
            if(telUs.Name == null)
            {
                telUs.Name = e.Message.Text;
                await SendMessage("Ссылку на фото: ", e.Message.Chat.Id);
                return;
            }
               
            if (telUs.Photo == null)
            {
                var test = client.GetFileAsync(e.Message.Photo[e.Message.Photo.Count() - 1].FileId);
                var download_url = $"https://api.telegram.org/file/bot<{token}>/" + test.Result.FilePath;
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(download_url), @"c:\temp\NewCompanyPicure.png");

                    byte[] imageBytes =client.DownloadData(download_url);
                    telUs.Photo = imageBytes;
                }

                userL.Add(photo);
            }
            client.OnMessage -= Client_OnMessage1;
            client.OnMessage += Client_OnMessage;
            e.Message.Text = "Регистрация";
            Client_OnMessage(sender, e);
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
