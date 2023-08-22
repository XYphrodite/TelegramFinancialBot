using Microsoft.EntityFrameworkCore;
using myTestTelegramBot.Data;
using myTestTelegramBot.Models;

namespace myTestTelegramBot.Services
{
    public class Repository
    {
        public static async Task Add(TransactionModel model)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                if(TryGetUser(model.User.UserId, out var user))
                    model.User = user;
                context.Transactions.Add(model);
                await context.SaveChangesAsync();
            }

        }
        public static bool TryGetUser(long userId, out UserModel user)
        {
            using (ApplicationContext context = new ApplicationContext())
                user = context.Users.FirstOrDefault(u => u.UserId == userId);
            return user != null;
        }

    }
}
