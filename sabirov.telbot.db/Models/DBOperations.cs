using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sabirov.telbot.db.Models
{
    public class DBOperations
    {
        public static async Task AddUser(string username, long chatid, byte[] photo)
        {
          
            using (TelUserContext db = new TelUserContext())
            {
                var user = db.telUsers.FirstOrDefault(x => x.ChatId == chatid);
                if (user != null) {
                    await TelApi.SendMessage("Вы уже присутствуете в моей базе!", chatid);
                    return;
                }
                db.telUsers.Add(new TelUser()
                {
                    Name = username,
                    ChatId = chatid,
                    Photo = photo
                });
                await TelApi.SendMessage("Регистрация успешна!", chatid);
                db.SaveChanges();
            }
        }
        public static async Task RemoveUser(long chatid)
        {
            using (TelUserContext db = new TelUserContext())
            {
                var user = db.telUsers.FirstOrDefault(x => x.ChatId == chatid);
                if (user == null)
                {
                    await TelApi.SendMessage("Вас нет в моей базе", chatid);
                    return;
                }
                db.telUsers.Remove(user);
                await TelApi.SendMessage("Удаление успешно!", chatid);
                db.SaveChanges();
            }
        }
        public static async Task<TelUser> GetUser(long chatid)
        {
            using (TelUserContext db = new TelUserContext())
            {
                var user = db.telUsers.FirstOrDefault(x => x.ChatId == chatid);
                if (user == null)
                {
                    //await TelApi.SendMessage("Пройдите регистрацию! Вас нет в моей базе!", chatid);
                    return null;
                }
                db.SaveChanges();
                return user;
            }
        } 
        //public static async Task<TelUser> EditUser(string username, long chatid)
        //{
           
        //}
        public static Stream ConvertPhotoStream(byte[] file)
        {
            Stream stream = new MemoryStream(file);
            return stream;
        }
    }
}
