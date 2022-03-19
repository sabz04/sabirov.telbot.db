using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sabirov.telbot.db.Models
{
    public class DBOperations
    {
        public static async Task AddUser(string username, long chatid)
        {
            using(TelUserContext db = new TelUserContext())
            {
                var user = db.telUsers.FirstOrDefault(x => x.Name == username);
                if (user != null) {
                    await TelApi.SendMessage("Такой пользователь уже существует!", chatid);
                    return;
                }
                db.telUsers.Add(new TelUser()
                {
                    Name = username,
                    ChatId = chatid
                });
                await TelApi.SendMessage("Регистрация успешна!", chatid);
                db.SaveChanges();
            }
        }
        public static async Task GetUser(string username, long chatid)
        {
            using (TelUserContext db = new TelUserContext())
            {
                var user = db.telUsers.FirstOrDefault(x => x.Name == username);
                if (user == null)
                {
                    await TelApi.SendMessage("Пройдите регистрацию! Вас нет в моей базе!", chatid);
                    return;
                }
                db.telUsers.Add(new TelUser()
                {
                    Name = username,
                    ChatId = chatid
                });
                await TelApi.SendMessage("Регистрация успешна!", chatid);
                db.SaveChanges();
            }
        }
    }
}
