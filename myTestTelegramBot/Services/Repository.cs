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
                if (TryGetUser(model.User.UserId, out var user))
                {
                    model.UserId = user.Id;
                    model.User = null;
                }
                await context.Transactions.AddAsync(model);
                await context.SaveChangesAsync();
            }

        }

        //private static async Task Add(UserModel user)
        //{
        //    using (ApplicationContext context = new ApplicationContext())
        //    {
        //        await context.Users.AddAsync(user);
        //        await context.SaveChangesAsync();
        //    }
        //}

        public static bool TryGetUser(long userId, out UserModel user)
        {
            using (ApplicationContext context = new ApplicationContext())
                user = context.Users.FirstOrDefault(u => u.UserId == userId);
            return user != null;
        }

        public static async Task<UserModel> GetUser(Guid id)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
        }

    }
}
