using myTestTelegramBot.Data;
using myTestTelegramBot.Models;

namespace myTestTelegramBot.Services
{
    public class Repository
    {
        private readonly ApplicationContext context;

        public Repository()
        {
            this.context = new ApplicationContext();
        }
        public static async Task Add(TransactionModel model)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                context.Transactions.Add(model);
                await context.SaveChangesAsync();
            }

        }

    }
}
