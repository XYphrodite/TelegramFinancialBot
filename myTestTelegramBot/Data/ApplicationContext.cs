using Microsoft.EntityFrameworkCore;
using myTestTelegramBot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTestTelegramBot.Data
{
    public class ApplicationContext: DbContext
    {
        private string _connectionString;


        public DbSet<TransactionModel> Transactions { get; set; }

        public ApplicationContext()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            Database.EnsureCreated();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
