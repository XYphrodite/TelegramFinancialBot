using Microsoft.EntityFrameworkCore;
using myTestTelegramBot.Models;
using System.Configuration;

namespace myTestTelegramBot.Data
{
    public class ApplicationContext: DbContext
    {
        private string _connectionString;


        public DbSet<TransactionModel> Transactions { get; set; }
        public DbSet<UserModel> Users { get; set; }

        public ApplicationContext()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
