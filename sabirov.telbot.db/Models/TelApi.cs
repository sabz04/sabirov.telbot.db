using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private static ITelegramBotClient client;
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
                        await SendMessage("Вы уже зарегистрированы.", e.Message.Chat.Id);
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
                        telUs = null;
                        break;
                    default:
                        await SendMessage("Не знаю такой команды...", msg.Chat.Id);
                        break;
                    
                }
            }
        }

        private static async void Client_OnMessage1(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            
            if (telUs.Name == null)
            {
                telUs.ChatId = e.Message.Chat.Id;
                if (e.Message.Text == null) { await SendMessage("Извините, это не подходит, введите заново.", e.Message.Chat.Id); return; }
                telUs.Name = e.Message.Text;
                await SendMessage("Оправьте фото: ", e.Message.Chat.Id);
                return;
            }
            if (telUs.Photo == null)
            {
                if (e.Message.Photo == null) { await SendMessage("Извините, это не подходит, отправьте заново.", e.Message.Chat.Id); return; }
                var test = await client.GetFileAsync(e.Message.Photo[e.Message.Photo.Count() - 1].FileId);

                MemoryStream str = new MemoryStream();
                await client.DownloadFileAsync(test.FilePath, str);
                MemoryStream img = await RoundCorners(Image.FromStream(str),200);
                
                telUs.Photo = img.ToArray();
                
            }
            await DBOperations.AddUser(telUs.Name, telUs.ChatId, telUs.Photo);
            client.OnMessage -= Client_OnMessage1;
            client.OnMessage += Client_OnMessage;
            
        }
        public static async Task<MemoryStream> RoundCorners(Image StartImage, int CornerRadius)
        {
            CornerRadius *= 2;
            Bitmap RoundedImage = new Bitmap(StartImage.Width, StartImage.Height);
            using (Graphics g = Graphics.FromImage(RoundedImage))
            {
                g.Clear(Color.Transparent);
                
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Brush brush = new TextureBrush(StartImage);
                GraphicsPath gp = new GraphicsPath();
                gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                gp.AddArc(0, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                g.FillPath(brush, gp);
                MemoryStream ms = new MemoryStream();
                RoundedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms;
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
