using Microsoft.EntityFrameworkCore;
using myTestTelegramBot.Data;
using myTestTelegramBot.Models;

namespace myTestTelegramBot.Services
{
    public class Repository
    {
        private readonly ApplicationContext _context;

        public Repository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task Add(TransactionModel model)
        {
            if (TryGetUser(model.User.UserId, out var user))
            {
                model.UserId = user.Id;
                model.User = null;
            }
            await _context.Transactions.AddAsync(model);
            await _context.SaveChangesAsync();

        }

        //private static async Task Add(UserModel user)
        //{
        //    using (ApplicationContext context = new ApplicationContext())
        //    {
        //        await context.Users.AddAsync(user);
        //        await context.SaveChangesAsync();
        //    }
        //}

        public bool TryGetUser(long userId, out UserModel user)
        {
            user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            return user != null;
        }

        public async Task<UserModel> GetUser(long id) =>
            await _context.Users
            .Include(u => u.Transactions)
            .FirstOrDefaultAsync(u => u.UserId == id);

    }
}
